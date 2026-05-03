# Sync Variables（`[UdonSynced]`）

## 基本

```csharp
[UdonSynced] private string _score;
```

`[UdonSynced]` が付いたフィールドはネットワーク上で共有される。**オーナーのみが値を書き換えられる**。

## 2 つの同期モード

### Continuous

- オーナーが値を変更すると**自動で定期送信**される
- 頻繁に変わる値（位置、進行度など）向け
- 1 serialization あたり **~200 bytes** 上限

### Manual

- オーナーが **`RequestSerialization()`** を明示的に呼んで送信する
- スコアボードなど正確性が重要な値向け
- 1 serialization あたり **~280,496 bytes** 上限
- データサイズに応じて送信レートが制限される（連続で呼び放題だが実送信は Udon が間引く）

```csharp
[UdonSynced] private int _score;

public void AddScore()
{
    if (!Networking.IsOwner(gameObject)) return;
    _score++;
    RequestSerialization();
}
```

## 同期可能な型

| 型 | Size |
|----|------|
| `bool` | 1 byte |
| `sbyte` / `byte` | 1 byte |
| `short` / `ushort` | 2 bytes |
| `int` / `uint` | 4 bytes |
| `long` / `ulong` | 8 bytes |
| `float` | 4 bytes |
| `double` | 8 bytes |
| `Vector2` | 8 bytes |
| `Vector3` | 12 bytes |
| `Vector4` / `Quaternion` | 16 bytes |
| `Color` | 16 bytes |
| `Color32` | 4 bytes |
| `char` | 2 bytes |
| `string` | 2 bytes / char |
| `VRCUrl` | 2 bytes / char |

配列（`int[]`, `Vector3[]` 等）も同期可能。**scene object 参照は非対応**。

## 配列の必須初期化

**同期対象の配列が未初期化（null）だと、その UdonBehaviour 全体の同期が機能しない**。

```csharp
[UdonSynced] private int[] _scores = new int[0];
[UdonSynced] private Vector3[] _positions;

private void Start()
{
    if (_positions == null) _positions = new Vector3[0];
}
```

## 同期の粒度（差分同期ではない）

- 1 回の `RequestSerialization` で、その UB の**全 `[UdonSynced]` フィールド最終値**が 1 セットで送信される
- 連続で呼んでも**直前の最終値 1 セットのみ**送信（中間値は捨てられる）
- 1 UB 内の個別フィールドだけを選んで送ることはできない
- 同一 GameObject 上の複数 UB も**まとめて送信**される

## シリアライズ関連イベント

```
[Owner 側]
  RequestSerialization()           ← フラグ ON
           ↓（次の sync Tick 待機）
  OnPreSerialization()             ← 送信直前。ここで [UdonSynced] の最終値を確定できる
           ↓
  （ネットワーク送信）
           ↓
  OnPostSerialization(result)      ← 送信直後。result.success / byteCount 取得可

[非 Owner 側]
  （ネットワーク受信）
           ↓
  OnDeserialization()              ← 受信後に反映
```

### `SerializationResult`

```csharp
namespace VRC.Udon.Common
{
    public struct SerializationResult
    {
        public bool success;
        public int byteCount;
    }
}
```

### 失敗時の再送（VRChat は常時接続前提、再送上限なしで OK）

```csharp
public override void OnPostSerialization(VRC.Udon.Common.SerializationResult result)
{
    if (!result.success)
    {
        SendCustomEventDelayedSeconds(nameof(Resend), 0.5f);
    }
}

public void Resend()
{
    if (Networking.IsOwner(gameObject))
        RequestSerialization();
}
```

## Late Joiner（後から参加したプレイヤー）

**VRChat は新規参加者に `[UdonSynced]` 変数の最新値を自動配布する**（`OnDeserialization` 経由）。一方、**ネットワークイベント（`SendCustomNetworkEvent`）は late joiner に届かない**。

### 設計原則

- **状態は `[UdonSynced]` 変数に持たせる**
- **イベントは「その場限りのアクション」にのみ使う**（音、演出、一度きりの視覚効果）
- 状態変更をイベントで通知する設計は late joiner で崩れる

### パターン：Interact でイベント → オーナーが変数更新

```csharp
[UdonSynced] private bool _isDoorOpen;

public override void Interact()
{
    if (!Networking.IsOwner(gameObject))
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    _isDoorOpen = !_isDoorOpen;
    RequestSerialization();
    Apply();
}

public override void OnDeserialization()
{
    Apply();  // late joiner を含む全受信側で呼ばれる
}

private void Apply()
{
    door.SetActive(!_isDoorOpen);
}
```
