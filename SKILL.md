---
name: vrc-udonsharp
description: >
    UdonSharp (C# to Udon Assembly) のスクリプト作成・編集・レビュー・デバッグ時に発動。
    VRChat ワールド開発で UdonSharpBehaviour を扱うすべての場面で使用する。
    Triggers on: UdonSharp, Udon, VRChat world scripting, UdonSharpBehaviour,
    UdonSynced, VRCPlayerApi, SendCustomEvent, Networking.LocalPlayer, C# to Udon.
---

# vrc-udonsharp

UdonSharp は C# を Udon Assembly に変換する言語。標準 C# と異なる制約があるため、コード生成前に必ず `rules/` を読むこと。

## コード生成前に必ずロードするもの

- `rules/language.md` — 属性一覧、sync mode、`FieldChangeCallback` 制約
- `rules/basics.md` — `UdonSharpBehaviour` 基本構造、命名規則

## 必要に応じて参照するもの

- `references/performance.md` — Udon は C# の 200-1000 倍遅い、最適化方針
- `references/udon-assembly-optimization.md` — EXTERN 命令コストの実測知見（補間集約・公式メソッド優先・null 判定・検証手法）
- `references/editor-scripting.md` — `#if UNITY_EDITOR` / `COMPILER_UDONSHARP`、Proxy、`AddUdonSharpComponent`
- `references/create-script.md` — `.cs` + `.asset` セットの新規作成手順、`execute_code` スニペット（`snippets/`）
- `references/event-execution-order.md` — イベント発火順序
- `references/networking.md` — ネットワークの 3 柱、帯域制限、複数 UB 同居ルール
- `references/sync-variables.md` — `[UdonSynced]`、同期可能型、シリアライズイベント、late joiner
- `references/network-events.md` — `SendCustomNetworkEvent` + `[NetworkCallable]`、レート制限
- `references/ownership.md` — オーナーシップ、自動移譲、Master 非依存
- `references/players.md` — `VRCPlayerApi` の基本、参照取得、IsValid、位置・回転・音声・アバタースケール・コリジョン・言語
- `references/data-containers.md` — `DataToken` / `DataList` / `DataDictionary` / `VRCJson`（`List<T>` の代替）
- `references/persistence.md` — PlayerData / PlayerObject、`OnPlayerRestored` タイミング
- `references/input-events.md` — `InputJump` 等の VR/Desktop 両対応入力
- `references/ui-events.md` — uGUI Button/Slider/Toggle を Udon に繋ぐ
- `references/web-loading.md` — `VRCImageDownloader` / `VRCStringDownloader`、VRAM 管理、許可ドメイン
- `references/animation-events.md` — AnimationClip から UdonBehaviour を呼ぶ
- `references/avatar-events.md` — `OnAvatarChanged` / `OnAvatarEyeHeightChanged`
- `references/debugging.md` — ログファイル、オーバーレイ、起動フラグ、例外ウォッチャー
- `references/build-automation.md` — Build & Test / Reload / Publish（Publish は要ユーザー確認）
- `references/components.md` — VRCSceneDescriptor / VRCObjectSync / VRC_Pickup / VRC_Station / VRC_SpatialAudioSource / TextMeshPro 等の VRC シーンコンポーネント
- `references/dynamics.md` — PhysBones / Contacts / VRC Constraints のワールド利用と Udon 操作（SDK 3.10.0+、`ApplyConfigurationChanges` 必須）
- `references/vrctween.md` — VRCTween トゥイーンシステム（SDK 3.10.4+）：移動・フェード・DelayedCall・Virtual Tweens
- `references/world-basics.md` — ワールド作成手順、Community Labs、Unity Layers、SDK Prefabs、Items、Allowlisted Components
- `references/vrc-graphics.md` — VRCGraphics / VRCShader / VRCAsyncGPUReadback / VRCCameraSettings / VRCQualitySettings / Shader Globals
- `references/video-players.md` — AVPro vs Unity VideoPlayer、Allowlist、ライブ配信
- `references/midi.md` — VRC_MidiListener / VRCMidiPlayer、Realtime と Playback
- `references/clientsim.md` — Editor 内シミュレーション、制約、自動テスト、PlayerObject / PlayerData Editor
- `references/ai-navigation.md` — NavMesh 連携、Unity 2022 AI Navigation
- `references/udonsharp-meta.md` — Project Settings / Migration / FAQ / Community Resources
- `references/examples-index.md` — VRChat 公式サンプルワールド索引（Examples Central からインポート可能）
- `references/network-id-utility.md` — Network ID の仕組み、Import/Export Utility、クロスプラットフォーム対応
- `references/vrchat-api.md` — UdonSharp から呼べる VRChat API 網羅（Networking / VRCPlayerApi / UdonBehaviour / VRCPickup / VRCObjectPool / VRCObjectSync / Enum 一覧）

## 作業フロー

1. 言語制約・属性ルールを `rules/language.md` で確認
2. クラス骨格・命名は `rules/basics.md`
3. パフォーマンスが関わるコードは `references/performance.md` と `references/udon-assembly-optimization.md`
4. カスタム Inspector / Gizmos は `references/editor-scripting.md`
5. イベント順序が絡む場合は `references/event-execution-order.md`
