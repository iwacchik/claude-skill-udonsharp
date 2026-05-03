# Event Execution Order

Udon / Unity 組み込みイベントの実行順序。

## 明文化されている項目

- **`Start()` は各スクリプトで 1 回だけ実行**
- **`Update()` は毎フレーム 1 回**
- **`FixedUpdate()` は物理ステップごと**
- **`LateUpdate()` は全 Update 後**

## 重要な注意（公式）

> Unity と VRChat のアップデートにより実行順序は変更される可能性がある。
> すべてのイベントが列挙されているわけではない。
> 状況によって異なる実行順序もある（オブジェクトのオーナーかどうか、ワールド参加タイミング等）。

→ **正確な順序に依存する設計は避ける**。依存する場合は明示的にイベント間で順序を制御する（フラグ・状態機械で）。

## 明示的に制御する手段

### `[DefaultExecutionOrder(int)]`

Update / LateUpdate / FixedUpdate の相対実行順を整数で制御：

```csharp
[DefaultExecutionOrder(-100)]  // 他の Update より早く
public class EarlyUpdater : UdonSharpBehaviour { ... }
```

負値＝早く実行、正値＝遅く実行、0＝デフォルト。

### イベント間の依存は状態で

```csharp
private bool _initialized;

private void Start() { _initialized = true; }

private void Update()
{
    if (!_initialized) return;
    // ...
}
```

## 公式図について

公式ページの実行順序は SVG 図として提供されている。テキスト化されていないため、**細かな順序が必要な場面は公式ページの図を直接確認すること**：

- https://creators.vrchat.com/worlds/udon/event-execution-order

## 設計原則

- **イベント順序に依存しないコード設計を優先**
- どうしても順序が必要なら：
  - `[DefaultExecutionOrder]` で明示指定
  - 状態フラグで「前段が終わったか」を自分で管理
  - `SendCustomEventDelayedFrames(1)` で 1 フレーム遅延させて確実に後段で動く
