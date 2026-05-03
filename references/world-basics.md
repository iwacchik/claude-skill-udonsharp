# World Basics（ワールド作成・レイヤー・運用）

## 初めてのワールド（最小手順）

1. Unity プロジェクトに **VRChat SDK** を導入（VCC 経由推奨）
2. アカウント：VRChat.com アカウント必要、**New User トラスト以上**でアップロード可
3. `VRChat SDK > Show Control Panel` で `VRC Scene Descriptor` を追加（自動で `VRCWorld` プレハブ配置）
4. `VRCWorld` の位置・回転がデフォルトスポーン。必要なら Spawn 位置を追加（`spawns[]` に Transform を登録）
5. `VRC_SceneDescriptor` で respawn height、reference camera 等を設定
6. Build & Test で確認、その後 Publish

## 公開前のチェックリスト

- ワールド名、説明、定員、Content Warnings、Tags、Thumbnail
- SDK 検証メッセージのエラー解消
- ビルド対象プラットフォーム（PC / Android）

## Community Labs（公開）

Community Labs 投稿ルール：

- **1 ユーザー 7 日に 1 回**まで新規投稿可
- 既存ワールドの更新はいつでも可
- Public 昇格の判定対象：**単一 VR ユーザーのスポーン地点で 45 FPS 以上**が目安

### パフォーマンス NG パターン

- 複数ミラー（深刻）
- Video Player が 3 つ以上
- リアルタイムライト過剰
- VR 非対応シェーダー

### コンテンツ規約

- 仮置きアバターを Public 後に TOS 違反アバターに差し替え → **1ヶ月の停止処分**
- アバターワールドはタイトルに `avatar` / `avi` を入れる
- ゲームワールドで**主要な遊び**がゲームなら `game` タグ

## Unity Layers（32 レイヤー）

VRChat は Unity の 32 レイヤーを使い分ける。

### 予約 vs ユーザー

- **Layer 0-21：予約**（アップロード時に自動リセット）
- **Layer 22-31：ユーザー**（自由に編集可、ビルド時に維持される）

### 主な予約レイヤー

| Layer | 名前 | 用途 |
|-------|------|------|
| 9 | Player | リモートプレイヤー |
| 10 | PlayerLocal | ローカルプレイヤー（頭ボーン非表示） |
| 13 | Pickup | 掴めるアイテム（デフォルト） |
| 18 | MirrorReflection | ミラー内のみ表示、メインカメラで非表示 |

### Sticker 防止

VRChat+ のステッカーを特定メッシュに貼らせたくない場合、**Layer 8（Interactive）**に Collider を移す。VRChat / Unity はこのレイヤーを使っていない。

### Physics 呼出でのレイヤーマスク

予約レイヤーを除外してレイキャストする。結果には `Utilities.IsValid` で null / 保護されたオブジェクトをフィルタ（UdonBehaviour 停止回避）。

### Interact 透過レイヤー

以下のレイヤーは Interact 操作が透過する（ブロックしない）：

`UiMenu`, `UI`, `PlayerLocal`, `MirrorReflection`

それ以外はデフォルトで Interact / Grab / Toggle を遮る。

## SDK Prefabs（標準プレハブ）

`Packages > VRChat SDK - Worlds > Samples > UdonExampleScene > Prefabs` に収録：

| プレハブ | 用途 |
|---------|------|
| `VRCWorld` | SceneDescriptor のテンプレート（必須プレハブ） |
| `VRCAvatarPedestal` | アバター展示＆切替 |
| `VRCChair` | 座れる椅子 |
| `VRCMirror` | ミラー（反射設定済み） |
| `VRCPortalMarker` | ワールド間ポータル（**シーン root に置くこと**） |
| `VRCVideoSync` | AVPro + Unity Video のプレイヤー |
| Simple Pen System | 3D ペン |
| Udon Variable Sync | 同期実装の参考 |

## Supported Scripted Assets（公式サポート済の外部アセット）

VRChat が公式に対応している script 系アセット：

| アセット | 用途 | 備考 |
|---------|------|------|
| **TextMeshPro** | リッチテキスト、文字描画 | Unity 標準 Text より推奨 |
| **Post Processing Stack v2** | 画面エフェクト | Package Manager 経由。**Test フォルダは import 時に除外** |
| **Final IK** | IK 制御 | アニメ・リギング |
| **Dynamic Bone** | 物理揺れ | **v1.3.0 まで互換**（それ以降は不具合あり） |

**Quest プラットフォーム**では上記の多くが使えない。例外は TextMeshPro。

## Items in Udon Worlds

VRChat+ ユーザーがスポーンできるツール・ガジェット。

### 設計上の影響

- Items は **Item レイヤー**にスポーンする
- **Udon プログラムは Items を参照できない**（発見しても `null` として返る）
- Physics メソッドでの取得結果は `Utilities.IsValid` で必ずフィルタすること
- LayerMask で Item レイヤーを除外しておくのが安全

### 旧 Mirror の対応

古い VRCMirror は Item が映らない。最新プレハブは対応済み。古いものを使っている場合、Mirror の `Reflect Layers` に **Item** を追加する。

### ワールドで無効化する

「My Worlds」ページの `Items Enabled` トグルで無効化可能（要事由記入）。

## Allowlisted World Components

ワールドで使えるコンポーネントは allowlist 制。**リストに無いものは動作しない**。

### Unity 標準（約 90+ コンポーネント）

Animator, Rigidbody, Camera, Canvas, 各種 Collider, Joint, Renderer, ParticleSystem 等。

### VRChat 独自

`VRC_AvatarPedestal`, `VRCContactReceiver`, `VRCContactSender`, `VRC_MidiListener`, `VRC_MirrorReflection`, `VRCPhysBone`, VRCStation、各種 Portal / Audio 等。

### サードパーティ対応

- **TextMeshPro**（full）
- **Unity UI**（Button, Slider, Toggle, Dropdown, InputField, Layout Groups 等）
- **Post Processing Stack V2**
- **AVPro**（Video Player 系）
- **Final IK**（VRChat 側で改造版）

### 非推奨・廃止

- **Dynamic Bone** は廃止、`VRCPhysBone` に移行

### プラットフォーム制限

Android / Quest は追加の制限あり。PC と同じつもりで組むと Quest ビルドで弾かれる可能性。
