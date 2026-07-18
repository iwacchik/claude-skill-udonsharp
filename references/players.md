# VRCPlayerApi（Player API）

各プレイヤーは 1 つの `VRCPlayerApi` オブジェクトで表される。ワールドは `OnPlayerJoined` / `OnPlayerLeft` を `UdonSharpBehaviour` で受け取れる。

## プレイヤー参照の取得

```csharp
VRCPlayerApi me = Networking.LocalPlayer;   // ローカルプレイヤー

public override void OnPlayerJoined(VRCPlayerApi player) { }
public override void OnPlayerLeft(VRCPlayerApi player) { }
```

### 全プレイヤー取得（`GetPlayers`）

**毎フレーム呼ぶ場合は配列を再利用**（毎回 new すると GC 負荷）：

```csharp
// 毎回 new（低頻度イベント向け）
VRCPlayerApi[] all = VRCPlayerApi.GetPlayers();

// 再利用版（推奨）
private VRCPlayerApi[] _players;
private void Start() { _players = new VRCPlayerApi[100]; }

private void Update()
{
    int count = VRCPlayerApi.GetPlayerCount();
    VRCPlayerApi.GetPlayers(_players);
    for (int i = 0; i < count; i++) { /* _players[i] */ }
}
```

### ID 指定での取得

```csharp
int id = player.playerId;
VRCPlayerApi p = VRCPlayerApi.GetPlayerById(id);
```

## 必ずやること：`IsValid` 検証

退出後の参照は無効化される：

```csharp
if (Utilities.IsValid(player))
{
    string name = player.displayName;
}
```

## 基本プロパティ

| プロパティ | 型 | 内容 |
|-----------|-----|------|
| `displayName` | string | 表示名 |
| `isLocal` | bool | ローカルプレイヤーか |
| `isMaster` | bool | インスタンスマスターか |
| `isInstanceOwner` | bool | インスタンスオーナーか |
| `IsUserInVR()` | bool | VR デバイス使用中か |
| `IsValid` | bool | 参照有効か（`Utilities.IsValid` 経由推奨） |
| `isSuspended` | bool | デバイスサスペンド中か |
| `isVRCPlus` | bool | アクティブな VRC+ サブスクリプション保有か（SDK 3.10.3+） |
| `playerId` | int | キャッシュされた ID |

## プレイヤータグ（軽量 key-value）

```csharp
player.SetPlayerTag("team", "red");
string team = player.GetPlayerTag("team");
player.ClearPlayerTags();
```

**注意：`GetPlayersWithTag` は現状機能しない**（公式注記）。自前ループして `GetPlayerTag` で比較する。

## 位置・回転・テレポート

```csharp
Vector3 pos = player.GetPosition();
Quaternion rot = player.GetRotation();

Vector3 vel = player.GetVelocity();
player.SetVelocity(new Vector3(0, 5, 0));

bool grounded = player.IsPlayerGrounded();

// テレポート（ローカルプレイヤーのみ）
player.TeleportTo(position, rotation);
```

### `TeleportTo` の制約

- **ローカルプレイヤーのみ**（他プレイヤーを直接移動できない）
- Station に座っていると **ブロックされる**
- **`OnDeserialization` 中に呼ばない**（コリジョン問題の原因）
- リモートプレイヤーの補間 Lerp オプションあり

## ボーン情報

```csharp
Vector3 headPos = player.GetBonePosition(HumanBodyBones.Head);
Quaternion rightHand = player.GetBoneRotation(HumanBodyBones.RightHand);
```

アバターによって**存在しないボーンもある**ので、期待する位置に必ずあるとは限らない。

## トラッキングデータ（推奨）

ボーンよりトラッキングデータが頭・手のデータ取得には優先：

```csharp
var td = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
// td.position, td.rotation
```

`TrackingDataType`：`Head`, `LeftHand`, `RightHand`, `Origin`, `AvatarRoot` など。

- ローカル VR プレイヤー：ヘッドセット / トラッカーのデータ
- リモートプレイヤー：ボーンから推定

## 移動パラメータ（ローカルプレイヤー限定）

```csharp
player.SetWalkSpeed(2f);       // デフォルト 2、推奨 0-5
player.SetRunSpeed(4f);        // デフォルト 4、推奨 0-10
player.SetStrafeSpeed(2f);     // デフォルト 2、推奨 0-5
player.SetJumpImpulse(3f);     // デフォルト 0、範囲 0-10
player.SetGravityStrength(1f); // デフォルト 1、範囲 0-10
player.Immobilize(true);       // 位置固定（VR 視点は動ける）
```

推奨：Walk < Run、Strafe = Walk を目安に。

**リモートプレイヤーには適用できない**。

## アバタースケーリング

### モード切替

```csharp
player.SetManualAvatarScalingAllowed(true);   // プレイヤー自身で制御
// false でワールド主導
```

### ワールドからスケール指定

```csharp
player.SetAvatarEyeHeightByMeters(1.6f);
player.SetAvatarEyeHeightByMultiplier(1.2f);  // 元プリセットからの倍率
```

### 制限設定（プレイヤー制御時の上下限）

```csharp
player.SetAvatarEyeHeightMinimumByMeters(0.2f);   // 最小 0.2m
player.SetAvatarEyeHeightMaximumByMeters(5.0f);   // 最大 5m
```

### 取得

```csharp
float eye = player.GetAvatarEyeHeightAsMeters();  // ローカル・リモート両対応
bool manual = player.GetManualAvatarScalingAllowed();
```

### 関連イベント

- `OnAvatarChanged(VRCPlayerApi)` — アバター読込完了
- `OnAvatarEyeHeightChanged(VRCPlayerApi player, float previousEyeHeight)` — 視線高変更

**重要制約：アバターのスケール変更は当たり判定には影響しない**（見た目だけ）。

## 音声・アバター音量

```csharp
// ボイス（デフォルト値とオススメ範囲）
player.SetVoiceGain(15f);            // 0-24 dB、デフォルト 15
player.SetVoiceDistanceNear(0f);     // 近距離、デフォルト 0
player.SetVoiceDistanceFar(25f);     // 遠距離、デフォルト 25
player.SetVoiceLowpass(true);        // 遠距離のローパスフィルタ
player.SetVoiceVolumetricRadius(0f); // 0-1000m、音源を広げる

// アバター音（音声以外）
player.SetAvatarAudioGain(10f);             // 0-10 dB
player.SetAvatarAudioFarRadius(40f);
player.SetAvatarAudioNearRadius(...);
player.SetAvatarAudioVolumetricRadius(...);
player.SetAvatarAudioForceSpatial(true);
player.SetAvatarAudioCustomCurve(true);
```

DJ ワールドなど音質重視なら `SetVoiceLowpass(false)`。

## コリジョンイベント

プレイヤーとの当たり判定は 3 種類：

### Trigger（is Trigger コライダー）

```csharp
public override void OnPlayerTriggerEnter(VRCPlayerApi player) { }
public override void OnPlayerTriggerStay(VRCPlayerApi player) { }
public override void OnPlayerTriggerExit(VRCPlayerApi player) { }
```

- コライダーの **Is Trigger** を ON にすること
- 高速なテレポート時にイベントがスキップされる可能性あり

### Physics Collision（動くオブジェクト）

```csharp
public override void OnPlayerCollisionEnter(VRCPlayerApi player) { }
public override void OnPlayerCollisionStay(VRCPlayerApi player) { }
public override void OnPlayerCollisionExit(VRCPlayerApi player) { }
```

**重要：プレイヤーが静止物に「歩いて当たる」だけでは発火しない**。動くオブジェクトとの衝突向け。歩行検知は Trigger を使う。

### Particle Collision

```csharp
public override void OnPlayerParticleCollision(VRCPlayerApi player) { }
```

ParticleSystem の Collision モジュールと Send Collision Messages を有効にする。

## 言語設定（RFC 5646）

```csharp
string current = Networking.LocalPlayer.GetCurrentLanguage();   // "ja-JP"
string[] available = VRCPlayerApi.GetAvailableLanguages();
```

## Drone API（VRChat+ ドローン）

VRChat+ のプレイヤードローンの情報取得と trigger 判定。

### アクセス

```csharp
VRCDroneApi drone = player.GetDrone();     // プレイヤーのドローン API を取得
VRCPlayerApi owner = drone.GetPlayer();    // ドローンの所有プレイヤー
```

### VRCDroneApi

| メンバー | 内容 |
|---------|------|
| `IsDeployed()` | 展開中か |
| `GetPosition()` / `TryGetPosition(out Vector3)` | 位置 |
| `GetRotation()` / `TryGetRotation(out Quaternion)` | 回転 |
| `GetVelocity()` / `TryGetVelocity(out Vector3)` | 速度 |
| `TeleportTo(Vector3, Quaternion, bool lerp)` | 移動（lerp=true でリモートのドローンを補間） |
| `SetVelocity(Vector3)` | 速度設定 |

### Trigger イベント

```csharp
public override void OnDroneTriggerEnter(VRCDroneApi drone) { }
public override void OnDroneTriggerStay(VRCDroneApi drone) { }
public override void OnDroneTriggerExit(VRCDroneApi drone) { }
```

ドローンに干渉するインスタレーション系ワールドで使う。通常は不要。
