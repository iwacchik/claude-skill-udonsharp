# Avatar Events

## `OnAvatarChanged(VRCPlayerApi player)`

プレイヤーのアバター読込完了時に発火。

```csharp
public override void OnAvatarChanged(VRCPlayerApi player)
{
    // player がアバター切替を完了
}
```

## `OnAvatarEyeHeightChanged(VRCPlayerApi player, float previousEyeHeight)`

視線高（メートル）が変更されたときに発火：
- アバター切替による変化
- アバタースケーリングシステムによる変化

`previousEyeHeight` は変更前の値（メートル）。

## 発火タイミングの違い

### ローカルプレイヤー

- 永続化された視線高については**1 回だけ発火**
- 予測可能でシンプル

### リモートプレイヤー

- **複数回発火する可能性**（同期の中間値が次々届くため）
- `OnAvatarChanged` が完了する**前に** `OnAvatarEyeHeightChanged` が来ることもある
- 順序逆転はしないが、タイミングは不定

## 初回ロード時の注意

**初回入室時、`previousEyeHeight` の初期値は `0` になることがある**。これは「実際の変更」ではなく「初回配信」を示す目印として使える。

## 用途

- アバター切替を検知してログやエフェクト
- 視線高に応じた UI スケール調整
- スケール制限内かの検証・補正
