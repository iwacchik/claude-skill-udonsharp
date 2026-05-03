# Network ID Utility

ネットワーク同期対象 GameObject に割り当てられる **Network ID** の管理ツール。

## Network ID とは

- GameObject に割り当てられる**識別用の数値**
- ネットワーク上で「どのオブジェクトがどれか」を決定する
- **クロスプラットフォームワールド**で意味を持つ：プレイヤーは PC 版 / Android 版で技術的には異なるビルドを動かしているため、**両ビルド間で同じオブジェクトを指す**リンクが必要

### 具体例

> Beach ball（ID=1）と Ice cream cone（ID=2）があるとき、ID が食い違うと、一方のプレイヤーはビーチボールを蹴っているつもりでも、他方には**アイスクリームを蹴っている**ように見える。

通常は意識不要だが、クロスプラットフォーム対応時や、シーンを別プロジェクトへ移す時に問題になる。

## Network ID Import and Export Utility

場所：Unity Editor の **`VRChat SDK > Utilities > Network ID Import and Export Utility`**

### 主な操作

| ボタン | 用途 |
|-------|------|
| **Regenerate Scene IDs** | シーンの全同期対象に Network ID を再割り当て（初回・リセット時） |
| **Export** | 現在シーンの Network ID をファイルに保存 |
| **Import** | 別シーンから Export したファイルを読み込み、このシーンに適用 |
| **Scan For Conflicts** | ID の衝突を検出 |
| **Auto Scan**（チェック） | 編集中に継続的にスキャン |
| **Accept All** | 検出結果を一括適用（全部マッチする場合） |

### Export ファイル形式

**シーン階層パス**で各 Network ID を記録：

```json
{
  "10": "/CubePickup",
  "11": "/Prefabs/SyncedPen"
}
```

### Import の前提条件

- **各ネットワーク対象オブジェクトのヒエラルキーパスが両シーンで一致している必要**がある
- 非ネットワーク対象オブジェクトのパス差異は問題ない（ただし同期対象との衝突は不可）
- Import 前に既存 ID を**Clear してから**行うのが安全（衝突回避）

## コンフリクト解決

**Scan For Conflicts** を実行すると、次のような食い違いが検出される：

### ケース 1：ファイルに ID があるがシーンに該当オブジェクトがない

- **Ignore**：そのまま無視（該当オブジェクトがこのシーンに不要なら OK）
- **別オブジェクトを選択**：パスが違うだけなら該当するオブジェクトを指定して割り当て

解決すれば通常の accept 候補に移行する。

### ケース 2：シーン内オブジェクトに ID はあるが、ファイルの内容と食い違う

- 既存 ID を持つシーンに別ファイルを上から import したときに起こる
- **Clear IDs → Import** で回避するのが基本
- 「既存 ID をできるだけ壊さず修復したい」特殊ケースのために、部分的な適用オプションもある

## 運用ガイド

### 通常ケース

1. 作成したシーンで `Regenerate Scene IDs` → `Export`（バックアップとして保存）
2. 別プロジェクト・別シーンへ持っていくときは、先方で `Clear IDs`（既存があれば） → `Import`
3. `Scan For Conflicts` で検証 → `Accept All`

### クロスプラットフォーム（PC / Android）

- PC 用シーンと Android 用シーンで、**同期対象のパスが完全に一致**していることを確認
- 片方で Export → 片方で Import することで、同一の Network ID を付与する
- これを怠ると、PC・Android 間でオブジェクトの対応関係がずれる

### トラブル時

- 同期が「なぜか別オブジェクトに反映される」「late joiner で位置がおかしい」等の症状は Network ID 不整合を疑う
- Utility で `Scan For Conflicts` をまず実行
