# Events（VRChat / Udon の全イベント一覧）

UdonSharpBehaviour で `public override` できる / 発火するイベントまとめ。実行順序詳細は `event-execution-order.md`。

## ライフサイクル系（Unity 準拠）

| イベント | タイミング |
|---------|----------|
| `Start()` | 初回 Update 直前、1 回だけ |
| `Update()` | 毎フレーム |
| `FixedUpdate()` | 物理ステップごと |
| `LateUpdate()` | 全 Update 完了後 |
| `PostLateUpdate()` | LateUpdate 後、VRChat 独自。トラッキング・IK 完了後の値を使う処理向け |
| `OnEnable()` | コンポーネント有効化時 |
| `OnDisable()` | 無効化時（ClientSim では Play → Edit 切替時に実行されない、SDK 3.10.5+） |
| `OnDestroy()` | 破棄時 |

## プレイヤー系

| イベント | 引数 | 内容 |
|---------|------|------|
| `OnPlayerJoined(VRCPlayerApi)` | 入室プレイヤー | **PlayerData はまだ未到着** |
| `OnPlayerLeft(VRCPlayerApi)` | 退出プレイヤー | オーナー移譲は発火**前**に完了 |
| `OnPlayerRestored(VRCPlayerApi)` | プレイヤー | PlayerData 到着後。**データアクセスはここから** |
| `OnPlayerRespawned(VRCPlayerApi)` | プレイヤー | リスポーン時 |
| `OnPlayerDataUpdated(VRCPlayerApi, PlayerData.Info[])` | プレイヤー、変更情報 | PlayerData 変更・受信時 |

## アバター系

| イベント | 引数 | 内容 |
|---------|------|------|
| `OnAvatarChanged(VRCPlayerApi)` | プレイヤー | アバター読み込み完了 |
| `OnAvatarEyeHeightChanged(VRCPlayerApi, float previousEyeHeight)` | プレイヤー、前回の視線高(m) | 切替 or スケーリングで変化 |

### アバターイベントの時系列注意

- **ローカルプレイヤー**：視線高イベントは永続化された高さに対して 1 回だけ発火
- **リモートプレイヤー**：同期中間値ごとに**複数回発火する可能性**あり、`OnAvatarChanged` 完了前にも来うる
- **入室時の最初の `previousEyeHeight` は 0** になることがある（初期受信値の判別に使える）

## インタラクション系

| イベント | 内容 |
|---------|------|
| `Interact()` | Use ボタン押下時（proximity 内） |
| `OnPickup()` | Pickup 開始 |
| `OnDrop()` | Pickup 終了 |
| `OnPickupUseDown()` | Pickup 中 Use 押下 |
| `OnPickupUseUp()` | Pickup 中 Use 離し |
| `OnStationEntered(VRCPlayerApi)` | Station 着席 |
| `OnStationExited(VRCPlayerApi)` | Station 離脱 |

## コリジョン系（プレイヤー）

詳細は `players.md`。

- **Trigger**：`OnPlayerTriggerEnter` / `Stay` / `Exit`（Is Trigger 有効時）
- **Physics Collision**：`OnPlayerCollisionEnter` / `Stay` / `Exit`（**歩行侵入では発火しない**）
- **Particle**：`OnPlayerParticleCollision`

## コリジョン系（オブジェクト／Unity 準拠）

`OnTriggerEnter(Collider)`, `OnTriggerStay`, `OnTriggerExit`,
`OnCollisionEnter(Collision)`, `OnCollisionStay`, `OnCollisionExit`

## ネットワーク系

| イベント | 内容 |
|---------|------|
| `OnPreSerialization()` | 送信直前（Owner） |
| `OnPostSerialization(SerializationResult)` | 送信直後（Owner）、success/byteCount |
| `OnDeserialization()` | 受信後（非 Owner、late joiner 含む） |
| `OnOwnershipRequest(VRCPlayerApi, VRCPlayerApi)` → `bool` | 移譲前、false で拒否 |
| `OnOwnershipTransferred(VRCPlayerApi)` | 移譲完了 |
| `OnMasterTransferred` | Instance Master 変更 |
| `OnVariableChanged` | 個別変数変更（FCC 以外の監視手段） |

## 入力系

詳細は `input-events.md`。

| イベント | 概要 |
|---------|------|
| `InputJump(bool, UdonInputEventArgs)` | ジャンプ入力 |
| `InputUse(bool, UdonInputEventArgs)` | Use |
| `InputGrab(bool, UdonInputEventArgs)` | Grab |
| `InputDrop(bool, UdonInputEventArgs)` | Drop |
| `InputMoveHorizontal(float, UdonInputEventArgs)` | 左右移動軸 |
| `InputMoveVertical(float, UdonInputEventArgs)` | 前後移動軸 |
| `InputLookHorizontal(float, UdonInputEventArgs)` | 左右視点軸 |
| `InputLookVertical(float, UdonInputEventArgs)` | 上下視点軸 |
| `OnInputMethodChanged(VRCInputMethod)` | 入力方式切替 |

## Web Loading 系

詳細は `web-loading.md`。

| イベント | 内容 |
|---------|------|
| `OnStringLoadSuccess(IVRCStringDownload)` | 文字列 DL 成功 |
| `OnStringLoadError(IVRCStringDownload)` | 失敗 |
| `OnImageLoadSuccess(IVRCImageDownload)` | 画像 DL 成功 |
| `OnImageLoadError(IVRCImageDownload)` | 失敗 |

## Video Player 系

詳細は `video-players.md`。`VRCUnityVideoPlayer` / `VRCAVProVideoPlayer` からコールバックが来る。

| イベント | 引数 | 内容 |
|---------|------|------|
| `OnVideoStart()` | — | 再生開始（ロード完了後最初） |
| `OnVideoPlay()` | — | 再生再開（Pause→Play 含む） |
| `OnVideoPause()` | — | 一時停止 |
| `OnVideoEnd()` | — | 再生完了 |
| `OnVideoLoop()` | — | ループして戻った |
| `OnVideoReady()` | — | URL ロード完了、`Play()` 呼び出し可 |
| `OnVideoError(VideoError)` | エラー種別 | ロード失敗・再生失敗 |

## MIDI 系

詳細は `midi.md`。`VRC_MidiListener` / デバイス経由で発火。

| イベント | シグネチャ | 内容 |
|---------|-----------|------|
| `MidiNoteOn` | `(int channel, int number, int velocity)` | ノートオン |
| `MidiNoteOff` | `(int channel, int number, int velocity)` | ノートオフ |
| `MidiControlChange` | `(int channel, int number, int value)` | CC 変更 |

## キャラクターコントローラー衝突

| イベント | 引数 | 内容 |
|---------|------|------|
| `OnControllerColliderHitPlayer(VRCPlayerControllerColliderHit)` | hit 情報 | プレイヤーの CharacterController が非トリガーに当たったとき |

## オーバーライドの注意

- 全て `public override` で定義
- 引数の型と順序を正確に合わせる
- `Start`, `Update` 等の Unity 標準は `override` ではなく直接定義（`private void Start()` で OK）
