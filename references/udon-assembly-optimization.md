# Udon Assembly 最適化(実測知見)

> **注意:このファイルは公式ドキュメント由来ではない。** 実プロジェクトでのアセンブリ解析・実測ベンチマークによる独自知見であり、MAINTENANCE.md の doc 追随バッチの**対象外**(手動管理)。検証環境:VRChat SDK 3.10.5-beta / Unity 2022.3.22f1(2026-08 計測)。

## 実行モデル:命令数(特に EXTERN)がコストを決める

Udon Assembly はスタックベースのインタプリタ VM で実行され、1 命令ごとの fetch-dispatch 固定費が支配的。UdonSharp ではメソッド呼び出し・プロパティアクセス(`.x`、`.eulerAngles`、`.Append(",")` 等)の 1 つ 1 つが EXTERN 命令になる。

- **JIT/IL 最適化は一切効かない**:インライン化・定数畳み込み・ループ不変式ホイストなし。`for (i < _data.Length)` の `Length` は**毎反復 EXTERN で再評価**される(実測)
- **中間状態が実体を持つ**:式の途中結果はヒープスロットに具現化され、値型はボックス化される
- **C# の GC 最適化常識が通用しない**:「アロケーション削減のために呼び出しを増やす」(例:Append 連鎖)は、削った分以上のボックス化+解釈コストを生む逆効果
- 実測の目安:空ループ 1 反復 約 308ns、null チェック 1 回 約 590〜690ns

## 文字列整形は補間文字列に集約する

毎フレームの文字列整形は、細かい `Append` / `ToString` の連鎖でなく補間文字列に集約して呼び出し回数を減らす。

- 実測:Vector3 ×2 の整形が Append 連鎖 26 EXTERN → 補間 1 発 5 EXTERN(プログラム総命令数 232 → 127)
- **穴 3 個以下**なら `String.Format(s, o, o, o)` の 1 EXTERN に集約される。**4 個以上**は `object[]` 経由になり要素代入が各 1 EXTERN で効果が薄れる
- 書式は穴の中で指定する(`{value:F2}`)。明示的な `ToString("F2")` の EXTERN を省ける
- `Vector3.ToString("F1")` は括弧込みの `(x, y, z)` を 1 EXTERN で返す(`.x/.y/.z` の個別整形は component アクセスだけで 3 EXTERN 掛かる)
- `StringBuilder` はフィールドで確保して再利用する。`Append(StringBuilder)` が Udon に公開されており、`ToString()` を挟まず中間 string なしで直接連結できる

## 自作ループより公開済み公式メソッド(ネイティブ実行)を優先する

自作 `for` は全反復が Udon 命令として解釈されるが、公開済みメソッドは EXTERN 1 発の向こう側でネイティブ実行される。

- 実測(`int[100]` の線形検索):自作 for = 1 反復 約 23 命令・5 EXTERN × N。`Array.IndexOf` = N によらずメソッド全体 11 命令・1 EXTERN
- `Array.Find` はデリゲート(Predicate)が Udon 非対応のため使えない。使えるのは `IndexOf` / `LastIndexOf` / `BinarySearch`、string 系メソッド、DataList 系など
- **ループを書く前に「1 EXTERN で済む公開 API がないか」を探す**のを習慣にする

## null 判定の使い分け

| 対象 | 書き方 | 理由 |
|------|--------|------|
| UnityEngine.Object 派生(Component / GameObject / アセット) | `if (obj)` / `if (!obj)` | 最速。疑似 null(Destroy 済み)対応 |
| `VRCPlayerApi` | `Utilities.IsValid(player)` | 退室後の無効化検知はこれが唯一 |
| `new` した非 Unity 型(StringBuilder 等) | `== null` | implicit なし。参照比較で十分 |
| `object` 型スロットに Unity Object が入りうる場合 | `Utilities.IsValid(obj)` | `== null` は参照比較のため疑似 null を見逃す |

- 実測:UnityEngine.Object の `== null` は `op_Equality` + 一時スロット COPY×2 で 28 命令。`if` 単独は `op_Implicit` で 22 命令(1 チェック正味 594ns vs 672ns)
- `!` は分岐反転に最適化され、否定の EXTERN は生成されない
- 暗黙 bool 変換と `==` オーバーロードは UnityEngine.Object 基底の定義なので、派生型すべてで疑似 null を判別できる(ただし `Destroy()` の反映はフレーム末)
- `Utilities.IsValid` の速度は `== null` と統計的にほぼ同等(2〜3% 遅い傾向)。`object` 型スロットで選ぶ理由は速度でなく正しさ

## 検証手法

- **公開 API の確認**:`VRC.Udon.Editor.UdonEditorManager.Instance.GetNodeDefinitions()` を名前で検索する
- **アセンブリ検証**:`UdonEditorManager.Instance.DisassembleProgram(programAsset.SerializedProgramAsset.RetrieveProgram())` で逆アセンブルし、総命令数と EXTERN 数を数える
- **ベンチマーク**:単発の ms 比較は CPU タイミングノイズで信頼できない。1 万回ループ × 数十リピートを `Stopwatch.GetTimestamp()`(Udon 公開済み)で計測し、空ループのベースラインを差し引いて中央値・最小値で評価する
