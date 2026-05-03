# VRChat API（UdonSharp から呼べる API 一覧）

UdonSharp 公式ドキュメント `/vrchat-api` 由来。UdonSharp スクリプトから呼び出せる VRChat SDK 側の API のリファレンス。**Udon から露出されているメンバーのみ**で、SDK の C# 側に存在しても Udon 未公開のメンバーは呼べない点に注意。

## VRC.SDKBase.Utilities（static）

| メンバー | 内容 |
|---------|------|
| `bool IsValid(object obj)` | null 判定の正規手段。`VRCPlayerApi` や破棄済みオブジェクトの有効性確認に使う |
| `void ShuffleArray<T>(T[] array)` | 配列シャッフル（Udon 内でジェネリック配列として露出） |

## Networking（static）

| メンバー | 内容 |
|---------|------|
| `bool IsMaster` | ローカルプレイヤーが Instance Master かどうか |
| `VRCPlayerApi LocalPlayer` | ローカルプレイヤー参照 |
| `bool IsNetworkSettled` | ネットワーク初期化完了フラグ |
| `bool IsOwner(GameObject obj)` | ローカルが obj の Owner か |
| `bool IsOwner(VRCPlayerApi player, GameObject obj)` | 指定 player が obj の Owner か |
| `VRCPlayerApi GetOwner(GameObject obj)` | obj の Owner 取得 |
| `void SetOwner(VRCPlayerApi player, GameObject obj)` | Owner 変更。同 GameObject 上の全 UdonBehaviour に適用 |
| `double GetServerTimeInSeconds()` | サーバー時刻（秒）。同期タイミング合わせ用 |
| `int GetServerTimeInMilliseconds()` | サーバー時刻（ミリ秒） |
| `float CalculateServerDeltaTime(double a, double b)` | サーバー時刻差を float に |

詳細の運用は `networking.md` / `ownership.md`。

## VRCPlayerApi（プレイヤー参照）

### 状態プロパティ

| メンバー | 内容 |
|---------|------|
| `bool isLocal` | このプレイヤーがローカルか |
| `string displayName` | 表示名 |
| `bool isMaster` | Instance Master か |
| `int playerId` | セッション中の ID |

### 移動・姿勢

| メンバー | 内容 |
|---------|------|
| `Vector3 GetPosition()` / `Quaternion GetRotation()` | 位置 / 回転 |
| `void TeleportTo(Vector3, Quaternion)` | テレポート |
| `void TeleportTo(Vector3, Quaternion, VRC_SceneDescriptor.SpawnOrientation, bool lerpOnRemote)` | 詳細オプション付きテレポート |
| `void SetVelocity(Vector3)` / `Vector3 GetVelocity()` | 速度 |
| `void Respawn()` / `void Respawn(int spawnPointIndex)` | リスポーン |

### 移動パラメータ

| メンバー | 内容 |
|---------|------|
| `SetWalkSpeed(float)` / `GetWalkSpeed()` | 歩行速度 |
| `SetRunSpeed(float)` / `GetRunSpeed()` | 走行速度 |
| `SetStrafeSpeed(float)` / `GetStrafeSpeed()` | 横移動速度 |
| `SetJumpImpulse(float)` / `GetJumpImpulse()` | ジャンプ初速度 |
| `SetGravityStrength(float)` / `GetGravityStrength()` | 重力係数 |
| `Immobilize(bool)` | 移動ロック |
| `UseLegacyLocomotion()` | 旧挙動に戻す |

### トラッキング

| メンバー | 内容 |
|---------|------|
| `TrackingData GetTrackingData(TrackingDataType)` | Head/LeftHand/RightHand/Origin の位置回転 |
| `Vector3 GetBonePosition(HumanBodyBones)` / `Quaternion GetBoneRotation(...)` | ボーン姿勢（IK/アバター依存） |
| `bool IsUserInVR()` | VR モードか |

### 音声・ボイス

| メンバー | 内容 |
|---------|------|
| `SetVoiceDistanceNear/Far(float)` | 近距離・遠距離の減衰境界 |
| `SetVoiceGain(float)` | ボイスゲイン |
| `SetVoiceVolumetricRadius(float)` | 音源ボリューム半径 |
| `SetVoiceLowpass(bool)` | ローパスフィルタ |
| `SetAvatarAudioVolumetricRadius(float)` 他 | アバター音声パラメータ（Near/Far/Gain/Force Spatial 等） |
| `PlayHapticEventInHand(HandType, float duration, float amplitude, float frequency)` | VR 触覚振動 |

### プレイヤー取得

| メンバー | 内容 |
|---------|------|
| `static VRCPlayerApi[] GetPlayers(VRCPlayerApi[] buffer)` | バッファ再利用で全プレイヤー取得 |
| `static int GetPlayerCount()` | 人数 |
| `static VRCPlayerApi GetPlayerById(int id)` | ID から取得 |
| `VRCPlayerApi.PlayerTag` | タグ機能（プレイヤーごとの文字列ラベル） |

詳細は `players.md`。

## UdonBehaviour（自身 / 他 UB 操作）

| メンバー | 内容 |
|---------|------|
| `void SendCustomEvent(string name)` | ローカル呼び出し |
| `void SendCustomNetworkEvent(NetworkEventTarget, string name)` | ネットワーク呼び出し |
| `void SendCustomEventDelayedSeconds(string name, float delay, EventTiming = Update)` | 秒遅延呼び出し |
| `void SendCustomEventDelayedFrames(string name, int frames, EventTiming = Update)` | フレーム遅延 |
| `object GetProgramVariable(string name)` | 動的変数取得 |
| `void SetProgramVariable(string name, object value)` | 動的変数設定 |
| `void RequestSerialization()` | Manual sync の送信トリガー |

**Note**: `GetProgramVariable` / `SetProgramVariable` は動的アクセス用で遅い。通常は直接フィールド参照推奨。

## VRCStation

| メンバー | 内容 |
|---------|------|
| `Mobility PlayerMobility` | `Immobilize` / `Mobile` |
| `bool canUseStationFromStation` | 着席中に別 Station へ直接遷移可能か |
| `void UseStation(VRCPlayerApi)` | プログラムから着席 |
| `void ExitStation(VRCPlayerApi)` | 離脱 |

## VRCPickup

| メンバー | 内容 |
|---------|------|
| `AutoHoldMode AutoHold` | 離しても持ち続けるか |
| `MomentumTransferMethod MomentumTransferType` | 投げたときの速度計算方式 |
| `bool DisallowTheft` | 他プレイヤーの横取り禁止 |
| `bool ExactGun` / `bool ExactGrip` | 把持ポイント（銃 / グリップの位置合わせ） |
| `float proximity` | 掴める距離 |
| `bool IsHeld` | 把持中か |
| `VRC_Pickup.PickupHand currentHand` | 現在の把持手 |
| `void Drop()` | 強制ドロップ |
| `void Drop(VRCPlayerApi)` | 指定プレイヤーがドロップ |
| `void GenerateHapticEvent(float duration, float amplitude, float frequency)` | VR 触覚 |

## VRCObjectPool

| メンバー | 内容 |
|---------|------|
| `GameObject TryToSpawn()` | プール内から未使用オブジェクトを 1 個取り出す（無ければ null） |
| `void Return(GameObject obj)` | プールへ返却 |

詳細は `networking.md`（Object Pool セクション）。

## VRCObjectSync

| メンバー | 内容 |
|---------|------|
| `bool AllowCollisionOwnershipTransfer` | 衝突時の自動オーナー移譲 |
| `void SetKinematic(bool)` | Rigidbody Kinematic 切替 |
| `void SetGravity(bool)` | 重力切替 |
| `void FlagDiscontinuity()` | 位置を急変させたと通知（補間しない） |
| `void TeleportTo(Vector3, Quaternion)` | テレポート |
| `void Respawn()` | リスポーン |

## VRCAvatarPedestal / VRCMirrorReflection / VRCPortalMarker

| メンバー | 内容 |
|---------|------|
| `VRCAvatarPedestal.SetAvatarUse(VRCPlayerApi)` | プレイヤーにアバター変更 UI 提示 |
| `VRCAvatarPedestal.blueprintId` | アバター ID 動的切替 |
| `VRCMirrorReflection.m_DisablePixelLights` 等 | 反射パラメータ |
| `VRCPortalMarker.RefreshPortal()` | Portal 再構築 |

## VRCUrl / VRCUrlInputField

| メンバー | 内容 |
|---------|------|
| `VRCUrl.Get()` | 文字列として取得 |
| `new VRCUrl(string)` | 一部の文脈でのみ許可（静的文字列向け） |
| `VRCUrlInputField.GetUrl()` | ユーザー入力 URL |
| `VRCUrlInputField.SetUrl(VRCUrl)` | プログラムから設定 |

## InputManager（static）

| メンバー | 内容 |
|---------|------|
| `VRCInputMethod currentInputMethod` | 現在の入力デバイス種別 |

## 構造体・Enum

### TrackingData（struct）

```csharp
public struct TrackingData {
    public Vector3 position;
    public Quaternion rotation;
}
```

### SerializationResult（struct）

```csharp
public struct SerializationResult {
    public bool success;
    public int byteCount;
}
```

### UdonInputEventArgs（struct）

入力イベントの第 2 引数。`boolValue` / `floatValue` / `inputMethod` / `handType` を保持。

### Enum 一覧

| Enum | 値 |
|------|----|
| `EventTiming` | `Update` / `LateUpdate` |
| `VRCStation.Mobility` | `Mobile` / `Immobilize` |
| `NetworkEventTarget` | `All` / `Others` / `Owner` / `Self` |
| `VRC_SceneDescriptor.SpawnOrientation` | `Default` / `AlignPlayerWithSpawnPoint` / `AlignRoomWithSpawnPoint` |
| `TrackingDataType` | `Head` / `LeftHand` / `RightHand` / `Origin` / `AvatarRoot` |
| `VRCInputMethod` | `Keyboard`=0 / `Mouse`=1 / `Controller`=2 / `Gaze`=3 / `Vive`=5 / `Oculus`=6 / `Count`=7 |
| `HandType` | `LEFT` / `RIGHT` |
| `UdonInputEventType` | `BUTTON` / `AXIS` |
| `VideoError` | `Unknown` / `InvalidURL` / `AccessDenied` / `PlayerError` / `RateLimited` |
| `VRC_Pickup.AutoHoldMode` | `Yes` / `No` / `AutoDetect` |
| `VRC_Pickup.PickupOrientation` | `Any` / `Grip` / `Gun` |
| `VRC_Pickup.PickupHand` | `None` / `Left` / `Right` |
| `MomentumTransferMethod` | `OnDrop` / `OnRelease` / `None` |

## Udon 未公開メンバーの判別

- SDK の C# クラス定義には載っていても、Udon 側に露出していないプロパティ・メソッドは呼べない
- 迷ったら `VRChat SDK > Udon Program` の Node Graph で使える Node 一覧を確認
- UdonSharp は Udon Assembly にコンパイル通らない場合エラーで落ちる
