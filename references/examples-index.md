# Examples Index（VRChat 公式サンプル索引）

公式ドキュメント（`creators.vrchat.com/worlds/examples/`）に掲載されているサンプル一覧。実装パターンの参照に使う。各サンプルは **VRChat SDK > Example Central** から Unity プロジェクトに import できる。

## 直下サンプル

| サンプル | 目的 | 主要コード / パターン |
|---------|------|-------------------|
| **Udon Basics** | 回転キューブ、Interact、Teleport、Script 間通信の 4 本の入門 | `Update`, `Interact()`, `TeleportTo`, 直接プロパティアクセス |
| **AI Navigation** | NPC が NavMesh 上で Red→Green へ移動。Blue 追加で経路再計算 | `NavMeshAgent`, `NavMeshSurface.BuildNavMesh`, `VRCObjectSync` |
| **Detect Controller Collide** | CharacterController の衝突判定 | `OnControllerColliderHit()`、VRChat 固有 `OnControllerColliderHitPlayer()` |
| **Image Loading** | `VRCImageDownloader` によるスライドショー。GitHub Pages ホスト推奨 | 組込 `ImageDownload` スクリプト、または自作 |
| **MIDI Playback** | 音楽と視覚を連動。4 チャネル分の鍵盤を色で可視化 | `VRCMidiPlayer`、`MidiGrid` UdonBehaviour、mod 12 で 12 音表示 |
| **Minimap** | 上空カメラ + `Graphics.Blit` でマップ texture 生成、自分は青、他プレイヤー緑のドット | Pickup prefab、カスタムシェーダー、更新頻度調整可 |
| **Mute Others** | 全プレイヤーをミュート/解除するボタン | `SetVoiceDistanceFar(0)` で無音、`25` で元に戻す |
| **Player Join Zones** | ゾーン入退出でプレイヤー名追跡、3 モード遷移、BossPicker 派生例 | OnPlayerTriggerEnter/Exit、OnPlayerLeft、同期リスト |
| **Screen Canvas** | 2D スクリーン UI（VR 非装着ユーザー向け）、VR 時は自動非表示 | `HideInVR` Graph、`TeleportToTarget` で瞬移動 |

## Obstacle Course（World Jam 2 スターター）

タイムトライアルのサンプル実装。複数のサブシステムで構成。

| ページ | 内容 |
|-------|------|
| **uoc-flythrough** | Cinemachine による自動カメラ撮影。**公開前に削除すること** |
| **uoc-how-stuff-works** | システム構成：PlayerDataManager（個別状態）、Course（チェックポイント進行）、ScoreManager（タイム同期）、PlayerModsManager（速度・ジャンプ一時変更） |
| **uoc-window** | Unity Editor 拡張ウィンドウ：チェックポイント管理、プレイヤー数、Score Display、PowerUp 配置、デフォルト移動値 |
| **build-from-custom-parts** | カスタム checkpoint/powerup/hazard 作成。CourseTrigger layer、OnPlayerDataEnter、Spawner と Hazard の分離 |
| **build-from-demo-parts** | Starter.unity + プレハブから配置。CTRL でグリッドスナップ、Obstacle Jam Utilities Window |

## Udon Example Scene（個別スクリプト例）

| ページ | 内容 |
|-------|------|
| **avatar-scaling-settings** | `disableAvatarScaling`, `minimumHeight`, `maximumHeight`, `alwaysEnforceHeight`（範囲外を min/max に強制） |
| **player-mod-setter** | Jump(3) / Run(4) / Walk(2) / Strafe(2) / Gravity(1) / Legacy Locomotion のデフォルト設定とサンプル |
| **simple-pen-system** | 3D ペン。VRCPickup + VRCObjectSync + LineRenderer、配列で点群同期（Manual 同期）、Use 開始で Pool から取得、閾値超で点追加、Use 終了で最終同期 |
| **udon-video-sync-player** | ビデオ同期。URL と time を `Vector2 (_timeAndOffset)` に詰めて送信、server 時刻と組合せて late joiner 計算、`_syncFrequency` 調整可 |
| **world-audio-settings** | 入室時に音声設定を適用。他スクリプトで後から変更可。**現在は default シーンに含まれない** |

## Persistence サンプル（PlayerData / PlayerObject 活用）

| ページ | 内容 |
|-------|------|
| **playerdata-types** | 18 種データ型全ての Get/Set デモ（bool, byte, int, float, double, long, short, string, sbyte, uint, ulong, ushort, Vector2/3/4, Quaternion, Color, Color32, byte[]）。`PlayerDataController` + `OnPlayerDataUpdated` |
| **position-sync** | プレイヤー位置・回転を 0.5 秒毎に記録、再入室で復元。`OnPlayerRestored` 起点、接地時のみ記録 |
| **health-bar** | PlayerObject で HP 同期・永続化。TakeDamage はローカルのみ適用、死亡でフル HP 復活 |
| **leaderboard** | ハイスコアの永続＋ヒエラルキー順序連動表示。`Leaderboard` と `LeaderboardSlot`（PlayerObject） |
| **persistent-idle-game** | クリッカー形式の idle ゲーム。`POINTS_KEY` / `AUTOCLICKERS_KEY` を `OnPlayerRestored` でロード |
| **persistent-pen** | 最大 20 本のライン永続保存。`VRCEnablePersistence` により URL 不要。Eraser あり。`SimplePenSystem` を VRCPlayerObject で個別保持 |
| **post-processing-settings** | Bloom 値を `settings_pp_weight` で永続化。UI Slider → PlayerData → Volume weight 反映 |
| **simple-rpg** | クラス選択、EXP、レベル UP、武器、戦闘（particle collision）。PlayerObject の `[UdonSynced]` で自動永続 |
| **unlock-items** | 実績アンロック（時間、距離、respawn、隠しなど）。`OnPlayerRestored` で復元、定期チェックで新規解放 |
