# Video Players

VRChat は 2 種の Video Player を提供する。用途に応じて選ぶ。

## 2 種の比較

| 項目 | AVPro Video Player | Unity Video Player |
|------|-------------------|-------------------|
| ライブ配信（YouTube Live, Twitch 等） | **対応** | 非対応 |
| Editor プレビュー | **不可**（Build & Test で確認） | `.mp4` / `.webm` 直接リンクで再生可 |
| YouTube/Vimeo 再生 | Build & Test / 本番 | クライアント起動時のみ |
| オーディオチャンネル | 最大 **6 ch**（コンポーネント表示は 8 chだが実装は 6 ch） | 制限なし |

## プレハブ

`Packages/VRChat SDK - Worlds/Samples/UdonExampleScene/Prefabs/VideoPlayers/` に同期済みプレハブがある。**Looping は同期信頼性のため無効化されている**。

## URL 頻度制限

**同一ユーザーが新しい Video URL を扱えるのは 5 秒に 1 回**。複数同時に開く場合は**時差を付けてリクエスト**する（特に late joiner 向け）。

## 配信サイトのホスト

- **無料**：YouTube、Vimeo Basic
- **自己ホスト**：CDN。**`fast start` エンコード**（ffmpeg の `-movflags +faststart`）にしないとストリーミングにならない

## Allowlist（デフォルト許可ドメイン）

以下は**ユーザー設定不要**でアクセス可能：

- YouTube / youtu.be、Twitch.TV、Vimeo、Facebook Video、Google Video
- SoundCloud、NicoNico、Mixcloud
- Akamai CDN、Hyperbeam、VRCDN、Youku、Topaz Chat

### 制限

- **ドメインが完全一致する必要**（短縮 URL は動かない）
- **Android は HTTPS 必須**
- Untrusted URL はユーザー設定で明示許可が必要
- SDK 標準 Video Player は untrusted URL 禁止時の処理を持たない

> Allowlist は**予告なく変更され得る**。公式パートナーシップではない。
