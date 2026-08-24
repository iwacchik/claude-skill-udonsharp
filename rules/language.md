# UdonSharp 言語ルール

UdonSharp は C# を Udon Assembly にコンパイルする。以下の属性・enum・制約を**コード生成前に必ず確認**する。

## 動作要件

- Unity **2019.4.31f1**
- VRCSDK3 + Udon
- 導入：VCC（VRChat Creator Companion）経由が推奨

## 使える属性

### Unity 標準属性（すべて有効）

| 属性 | 用途 |
|------|------|
| `[Header("...")]` | Inspector にヘッダー表示 |
| `[HideInInspector]` | Inspector 非表示 |
| `[NonSerialized]` | シリアライズ対象外 |
| `[SerializeField]` | private フィールドをシリアライズ |
| `[Space]` | Inspector スペース |
| `[Tooltip("...")]` | ツールチップ |
| `[ColorUsage(...)]` | カラーピッカー設定 |
| `[GradientUsage(...)]` | グラデーション設定 |
| `[TextArea]` | 複数行テキスト入力 |

### UdonSharp 固有属性

| 属性 | 用途 |
|------|------|
| `[UdonSynced]` | フィールドをネットワーク同期 |
| `[UdonSynced(UdonSyncMode.Linear)]` | 補間モード指定付き同期 |
| `[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]` | クラスの同期モード強制 |
| `[DefaultExecutionOrder(int)]` | Update / LateUpdate / FixedUpdate の実行順制御（負値対応） |
| `[RecursiveMethod]` | メソッドの再帰呼び出しを安全化 |
| `[FieldChangeCallback(nameof(Prop))]` | 値変化時にプロパティ setter を呼ぶ |

## `UdonSyncMode` enum

フィールド単位の補間モード。`[UdonSynced(UdonSyncMode.X)]` で指定。

| 値 | 挙動 |
|----|------|
| `NotSynced` | 同期しない |
| `None` | 補間なし（デフォルト） |
| `Linear` | `Lerp` による線形補間 |
| `Smooth` | 滑らかな補間 |

## `BehaviourSyncMode` enum

クラス単位の同期モード。`[UdonBehaviourSyncMode(BehaviourSyncMode.X)]` で指定。

| 値 | 挙動 |
|----|------|
| `Any`（デフォルト） | Inspector でユーザーが選択可能 |
| `None` | 同期変数禁止、UI 非表示、`SendCustomNetworkEvent` 無効 |
| `Continuous` | 高頻度自動更新（信頼性より頻度優先） |
| `Manual` | 低頻度・手動更新（`RequestSerialization()` で信頼性確保） |
| `NoVariableSync` | 同期変数禁止だが Manual/Continuous の NetworkEvent は使える |

## `FieldChangeCallback` の厳守ルール

```csharp
[UdonSynced, FieldChangeCallback(nameof(Score))]
private int _score;

public int Score
{
    get => _score;
    set { _score = value; /* 副作用 */ }
}
```

**重要な制約：**

- **`SetProgramVariable` またはネットワーク同期による値更新時のみ**、setter が発動する
- **同一 UdonBehaviour 内で `_score = ...` のようにバッキングフィールドへ直接代入しても setter は発動しない**
- **コンパイル時に直接代入は検出されてコンパイルエラーになる**
- 値を更新するときは必ずプロパティ経由（`Score = ...`）で書く

## コンパイル時検証

- 禁止構文や `FieldChangeCallback` バッキングフィールド直接代入は**コンパイルエラー**で検出される
- エラーは Unity Console に表示される

## 条件コンパイル — Assembly Version Defines（SDK 3.10.5+）

- asmdef の **Version Defines** で定義したシンボルを UdonSharp スクリプトの `#if` で使える
- 例：asmdef に Resource `com.vrchat.worlds`・Expression `3.10`・Define `SDK_310_OR_NEWER` を定義 → `#if SDK_310_OR_NEWER` で SDK バージョン依存コードをコンパイル時に切替できる

## 1 スクリプト = 1 UdonSharpProgramAsset

- 各 `.cs` は**1 つの `UdonSharpProgramAsset`（`.asset`）にのみ接続**される
- デプロイ後に割り当て変更すると**予測不能な動作**を招く
- リネーム時は `.cs` と `class` 名を同時に変更する（UdonSharp v1.0+ の要件）
