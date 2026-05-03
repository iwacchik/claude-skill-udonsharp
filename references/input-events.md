# Input Events

VR / Desktop 両プラットフォームで同じイベントが発火する統一入力 API。

## 共通引数：`UdonInputEventArgs`

全入力イベントは `UdonInputEventArgs args` を受け取る：

| メンバー | 型 | 内容 |
|---------|-----|------|
| `eventType` | `UdonInputEventType` | `BUTTON` または `AXIS` |
| `boolValue` | bool | ボタン押下判定 |
| `floatValue` | float | 軸値（-1.0〜1.0） |
| `handType` | `HandType` | `LEFT` / `RIGHT` |

## ボタン系（bool、押下=true / 離し=false）

```csharp
public override void InputJump(bool value, UdonInputEventArgs args) { }
public override void InputUse(bool value, UdonInputEventArgs args) { }
public override void InputGrab(bool value, UdonInputEventArgs args) { }
public override void InputDrop(bool value, UdonInputEventArgs args) { }
```

| イベント | Desktop | VR |
|---------|---------|-----|
| `InputJump` | Space | フェイスボタン |
| `InputUse` | 左クリック | トリガー |
| `InputGrab` | 左クリック | グリップ |
| `InputDrop` | 右クリック | グリップ操作（機種依存） |

## 軸系（float、-1.0〜1.0）

```csharp
public override void InputMoveHorizontal(float value, UdonInputEventArgs args) { }
public override void InputMoveVertical(float value, UdonInputEventArgs args) { }
public override void InputLookHorizontal(float value, UdonInputEventArgs args) { }
public override void InputLookVertical(float value, UdonInputEventArgs args) { }
```

| イベント | Desktop | VR |
|---------|---------|-----|
| `InputMoveHorizontal` | A/D | 左スティック X |
| `InputMoveVertical` | W/S | 左スティック Y |
| `InputLookHorizontal` | マウス X | 右スティック X |
| `InputLookVertical` | マウス Y | 右スティック Y |

## 入力方式切替

```csharp
public override void OnInputMethodChanged(VRCInputMethod newMethod) { }
```

`VRCInputMethod`：Keyboard / Mouse / Controller / Vive / Oculus / Generic 等。

## 制限

- **VRChat メニュー（Main / Quick / Text Input）表示中は入力イベント発火しない**
- 入力は**ローカルプレイヤーのみ**に発火（他プレイヤーの入力は届かない）

ネットワーク越しに入力相当の動作を反映したい場合は、`SendCustomNetworkEvent` と組み合わせる。

## サンプル：ジャンプでカウント

```csharp
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class JumpCounter : UdonSharpBehaviour
{
    [UdonSynced] private int _count;

    public override void InputJump(bool value, UdonInputEventArgs args)
    {
        if (!value) return;   // 押下時のみ
        if (!Networking.IsOwner(gameObject))
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        _count++;
        RequestSerialization();
    }
}
```
