# Knowledge Sources for vrc-udonsharp

VRChat Worlds 公式ドキュメントを網羅して vrc-udonsharp スキルに注入するための進捗トラッキング。

## 記法

- `[ ] URL` — 未処理
- `[x] URL` — 処理済み（スキルに注入完了）
- `[x] URL (fetched: YYYY-MM-DD)` — 再 fetch して更新済み（初回注入後）
- `[?] URL` — 自動取得失敗（**手動注入が必要**）
- `[-] URL` — 対象外（UdonSharp ワールド開発に不要）

**初回一括注入は 2026-04-21**。以降の更新は各行に `(fetched: ...)` を付与する。
更新ルーチンは `MAINTENANCE.md` 参照。

## creators.vrchat.com/worlds 直下

- [x] https://creators.vrchat.com/worlds/creating-your-first-world
- [x] https://creators.vrchat.com/worlds/submitting-a-world-to-be-made-public
- [x] https://creators.vrchat.com/worlds/items
- [x] https://creators.vrchat.com/worlds/layers
- [x] https://creators.vrchat.com/worlds/sdk-prefabs
- [x] https://creators.vrchat.com/worlds/supported-assets
- [x] https://creators.vrchat.com/worlds/whitelisted-world-components

## /worlds/components/ — Scene Components（12）

- [x] https://creators.vrchat.com/worlds/components/textmeshpro
- [x] https://creators.vrchat.com/worlds/components/vrc_avatarpedestal
- [x] https://creators.vrchat.com/worlds/components/vrc_cameradolly
- [x] https://creators.vrchat.com/worlds/components/vrc_mirrorreflection
- [x] https://creators.vrchat.com/worlds/components/vrc_objectsync
- [x] https://creators.vrchat.com/worlds/components/vrc_pickup
- [x] https://creators.vrchat.com/worlds/components/vrc_portalmarker
- [x] https://creators.vrchat.com/worlds/components/vrc_scenedescriptor
- [x] https://creators.vrchat.com/worlds/components/vrc_spatialaudiosource
- [x] https://creators.vrchat.com/worlds/components/vrc_station
- [x] https://creators.vrchat.com/worlds/components/vrc_uishape
- [x] https://creators.vrchat.com/worlds/components/vrc_enablepersistence

## /worlds/udon/ — Udon ルート

- [x] https://creators.vrchat.com/worlds/udon/
- [x] https://creators.vrchat.com/worlds/udon/ai-navigation
- [x] https://creators.vrchat.com/worlds/udon/animation-events
- [x] https://creators.vrchat.com/worlds/udon/avatar-events
- [x] https://creators.vrchat.com/worlds/udon/debugging-udon-projects
- [x] https://creators.vrchat.com/worlds/udon/event-execution-order
- [x] https://creators.vrchat.com/worlds/udon/external-urls
- [x] https://creators.vrchat.com/worlds/udon/image-loading
- [x] https://creators.vrchat.com/worlds/udon/input-events
- [x] https://creators.vrchat.com/worlds/udon/string-loading
- [x] https://creators.vrchat.com/worlds/udon/udon-moderation-tool-guidelines
- [x] https://creators.vrchat.com/worlds/udon/ui-events
- [x] https://creators.vrchat.com/worlds/udon/using-build-test
- [x] https://creators.vrchat.com/worlds/udon/world-debug-views
- [-] https://creators.vrchat.com/worlds/udon/graph/ — Udon Node Graph（UdonSharp 対象外）
- [-] https://creators.vrchat.com/worlds/udon/vm-and-assembly/ — VM 内部仕様（通常コードで使わない）

## /worlds/udon/players/ — Player API（8）

- [x] https://creators.vrchat.com/worlds/udon/players/
- [x] https://creators.vrchat.com/worlds/udon/players/drones/
- [x] https://creators.vrchat.com/worlds/udon/players/getting-players
- [x] https://creators.vrchat.com/worlds/udon/players/player-audio
- [x] https://creators.vrchat.com/worlds/udon/players/player-avatar-scaling
- [x] https://creators.vrchat.com/worlds/udon/players/player-collisions
- [x] https://creators.vrchat.com/worlds/udon/players/player-forces
- [x] https://creators.vrchat.com/worlds/udon/players/player-positions

## /worlds/udon/data-containers/ — Data Containers（6）

- [x] https://creators.vrchat.com/worlds/udon/data-containers/
- [x] https://creators.vrchat.com/worlds/udon/data-containers/byte-and-bit-operations
- [x] https://creators.vrchat.com/worlds/udon/data-containers/data-dictionaries
- [x] https://creators.vrchat.com/worlds/udon/data-containers/data-lists
- [x] https://creators.vrchat.com/worlds/udon/data-containers/data-tokens
- [x] https://creators.vrchat.com/worlds/udon/data-containers/vrcjson

## /worlds/udon/networking/ — Networking（12）

- [x] https://creators.vrchat.com/worlds/udon/networking/
- [x] https://creators.vrchat.com/worlds/udon/networking/compatibility
- [x] https://creators.vrchat.com/worlds/udon/networking/debugging
- [x] https://creators.vrchat.com/worlds/udon/networking/events
- [x] https://creators.vrchat.com/worlds/udon/networking/late-joiners
- [x] https://creators.vrchat.com/worlds/udon/networking/network-components
- [x] https://creators.vrchat.com/worlds/udon/networking/network-details
- [x] https://creators.vrchat.com/worlds/udon/networking/network-id-utility — **ユーザーが保存した HTML から手動取得・注入済み**
- [x] https://creators.vrchat.com/worlds/udon/networking/network-stats
- [x] https://creators.vrchat.com/worlds/udon/networking/ownership
- [x] https://creators.vrchat.com/worlds/udon/networking/performance
- [x] https://creators.vrchat.com/worlds/udon/networking/variables

## /worlds/udon/persistence/ — Persistence（3）

- [x] https://creators.vrchat.com/worlds/udon/persistence/
- [x] https://creators.vrchat.com/worlds/udon/persistence/player-data
- [x] https://creators.vrchat.com/worlds/udon/persistence/player-object

## /worlds/udon/video-players/ — Video Players（2）

- [x] https://creators.vrchat.com/worlds/udon/video-players/
- [x] https://creators.vrchat.com/worlds/udon/video-players/www-whitelist

## /worlds/udon/vrc-graphics/ — VRCGraphics（5）

- [x] https://creators.vrchat.com/worlds/udon/vrc-graphics/
- [x] https://creators.vrchat.com/worlds/udon/vrc-graphics/asyncgpureadback
- [x] https://creators.vrchat.com/worlds/udon/vrc-graphics/vrc-camera-settings
- [x] https://creators.vrchat.com/worlds/udon/vrc-graphics/vrc-quality-settings
- [x] https://creators.vrchat.com/worlds/udon/vrc-graphics/vrchat-shader-globals

## /worlds/udon/midi/ — MIDI（3）

- [x] https://creators.vrchat.com/worlds/udon/midi/
- [x] https://creators.vrchat.com/worlds/udon/midi/midi-playback
- [x] https://creators.vrchat.com/worlds/udon/midi/realtime-midi

## /worlds/clientsim/ — ClientSim（3）

- [ ] https://creators.vrchat.com/worlds/clientsim/
- [ ] https://creators.vrchat.com/worlds/clientsim/playerObject-editor
- [ ] https://creators.vrchat.com/worlds/clientsim/playerdata-editor-window

## /worlds/examples/ — Examples 直下（12）

- [x] https://creators.vrchat.com/worlds/examples/udon
- [x] https://creators.vrchat.com/worlds/examples/ai-navigation
- [x] https://creators.vrchat.com/worlds/examples/detect-controller-collide
- [x] https://creators.vrchat.com/worlds/examples/image-loading
- [x] https://creators.vrchat.com/worlds/examples/midi-playback
- [x] https://creators.vrchat.com/worlds/examples/minimap
- [x] https://creators.vrchat.com/worlds/examples/mute-others
- [x] https://creators.vrchat.com/worlds/examples/player-join-zones
- [x] https://creators.vrchat.com/worlds/examples/screen-canvas

### /worlds/examples/obstacle-course/（5）

- [x] https://creators.vrchat.com/worlds/examples/obstacle-course/build-from-custom-parts
- [x] https://creators.vrchat.com/worlds/examples/obstacle-course/build-from-demo-parts
- [x] https://creators.vrchat.com/worlds/examples/obstacle-course/uoc-flythrough
- [x] https://creators.vrchat.com/worlds/examples/obstacle-course/uoc-how-stuff-works
- [x] https://creators.vrchat.com/worlds/examples/obstacle-course/uoc-window

### /worlds/examples/persistence/（9）

- [x] https://creators.vrchat.com/worlds/examples/persistence/health-bar
- [x] https://creators.vrchat.com/worlds/examples/persistence/leaderboard
- [x] https://creators.vrchat.com/worlds/examples/persistence/persistent-idle-game
- [x] https://creators.vrchat.com/worlds/examples/persistence/persistent-pen
- [x] https://creators.vrchat.com/worlds/examples/persistence/playerdata-types
- [x] https://creators.vrchat.com/worlds/examples/persistence/position-sync
- [x] https://creators.vrchat.com/worlds/examples/persistence/post-processing-settings
- [x] https://creators.vrchat.com/worlds/examples/persistence/simple-rpg
- [x] https://creators.vrchat.com/worlds/examples/persistence/unlock-items

### /worlds/examples/udon-example-scene/（5）

- [x] https://creators.vrchat.com/worlds/examples/udon-example-scene/avatar-scaling-settings
- [x] https://creators.vrchat.com/worlds/examples/udon-example-scene/player-mod-setter
- [x] https://creators.vrchat.com/worlds/examples/udon-example-scene/simple-pen-system
- [x] https://creators.vrchat.com/worlds/examples/udon-example-scene/udon-video-sync-player
- [x] https://creators.vrchat.com/worlds/examples/udon-example-scene/world-audio-settings

## udonsharp.docs.vrchat.com（UdonSharp 公式ドキュメント）

- [x] https://udonsharp.docs.vrchat.com/
- [x] https://udonsharp.docs.vrchat.com/setup
- [x] https://udonsharp.docs.vrchat.com/examples
- [x] https://udonsharp.docs.vrchat.com/community-resources
- [x] https://udonsharp.docs.vrchat.com/configuration
- [x] https://udonsharp.docs.vrchat.com/migration
- [x] https://udonsharp.docs.vrchat.com/frequently-asked-questions
- [x] https://udonsharp.docs.vrchat.com/udonsharp
- [x] https://udonsharp.docs.vrchat.com/random-tips-&-performance-pointers
- [x] https://udonsharp.docs.vrchat.com/news
- [x] https://udonsharp.docs.vrchat.com/editor-scripting
- [x] https://udonsharp.docs.vrchat.com/vrchat-api (fetched: 2026-04-21)
- [x] https://udonsharp.docs.vrchat.com/events (fetched: 2026-04-21)

---

## 集計

- 未処理：約 110 ページ
- 取得失敗（要手動）：1 ページ（network-id-utility）
- 対象外：2 ページ

進捗：**完了（113 / 112）** 🎉

**全ページ注入完了**。network-id-utility は手動 HTML から追加。本スキルは全域 coverage 達成。
2026-04-21 再列挙で udonsharp.docs の `/vrchat-api` と `/events` が取りこぼしとして発見・追加注入。

## 完了ログ
- 2026-04-21 Batch 1：UdonSharp 言語仕様 / セットアップ / パフォーマンス Tips / Editor Scripting / Udon 概要 / Event Execution Order → v2 に `SKILL.md`、`rules/language.md`、`rules/basics.md`、`references/performance.md`、`references/editor-scripting.md`、`references/event-execution-order.md` 作成
- 2026-04-21 Batch 2：Networking 概要 / Ownership / Events / Late Joiners / Variables / Network Details → `references/networking.md`、`references/sync-variables.md`、`references/network-events.md`、`references/ownership.md` 作成
- 2026-04-21 Batch 3：Networking Compatibility / Debugging / Components / Stats / Performance / Player API 概要 → `references/networking.md` 拡張（互換性・components・stats・debugging・プロパティ・イベント追加）、`references/players.md` 作成
- 2026-04-21 Batch 4：Getting Players / Audio / Avatar Scaling / Collisions / Forces / Positions → `references/players.md` を全面拡充（取得 / 位置 / ボーン / トラッキング / 移動 / スケール / 音声 / コリジョン / 言語）
- 2026-04-21 Batch 5：Data Containers 全 6 ページ（intro / DataToken / DataList / DataDictionary / VRCJson / Byte-Bit Ops）→ `references/data-containers.md` 作成
- 2026-04-21 Batch 6：Persistence 3 / Input / UI / Image Loading → `references/persistence.md`、`input-events.md`、`ui-events.md`、`web-loading.md`（Image 部分）作成
- 2026-04-21 Batch 7：String Loading / Animation Events / Avatar Events / Debugging Udon / Using Build Test / External URLs → `web-loading.md` に String 拡充 + `animation-events.md`、`avatar-events.md`、`debugging.md`、`build-automation.md` 作成
- 2026-04-21 Batch 8：Scene Components 主要 6（SceneDescriptor / ObjectSync / Pickup / Station / SpatialAudioSource / TextMeshPro）→ `references/components.md` 作成
- 2026-04-21 Batch 9：Scene Components 残り 6（AvatarPedestal / CameraDolly / MirrorReflection / PortalMarker / UIShape / EnablePersistence）→ `references/components.md` 追記
- 2026-04-21 Batch 10：/worlds 直下 6（first-world / submitting / items / layers / sdk-prefabs / supported-assets）→ `references/world-basics.md` 作成
- 2026-04-21 Batch 11：whitelisted-world-components + VRCGraphics 5 → `world-basics.md` 追記（allowlist）＋ `references/vrc-graphics.md` 作成
- 2026-04-21 Batch 12：Video Players 2 + MIDI 3 + ClientSim 1 → `references/video-players.md`、`midi.md`、`clientsim.md` 作成
- 2026-04-21 Batch 13：ClientSim 2（Editor ウィンドウ）+ AI Navigation + Drones + World Debug Views + Moderation → `clientsim.md` 追記、`ai-navigation.md` 作成、`players.md` に Drone 追記、`debugging.md` に World Debug 追記
- 2026-04-21 Batch 14：UdonSharp docs 6（root / examples / community / config / migration / FAQ）→ `references/udonsharp-meta.md` 作成
- 2026-04-21 Batch 15：Examples 直下 6（Udon intro / AI Nav / Controller Collide / Image Loading / MIDI Playback / Minimap）→ `references/examples-index.md` 作成
- 2026-04-21 Batch 16（追補）：udonsharp.docs 再列挙で取りこぼし 2 ページ発見 — `/vrchat-api`（UdonSharp から呼べる VRChat API 網羅）と `/events`（UdonSharp override 可能イベント正規シグネチャ）→ `references/vrchat-api.md` 新規作成、`references/events.md` に PostLateUpdate / Video / MIDI / OnControllerColliderHitPlayer 追記
