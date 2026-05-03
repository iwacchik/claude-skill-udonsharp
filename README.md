# vrc-udonsharp

Claude Code 向けの **UdonSharp 開発支援スキル**。VRChat Worlds SDK / UdonSharp 公式ドキュメントを網羅的にまとめ、Claude が UdonSharp スクリプトを生成・レビューする際に自動で参照する。

## 何をしてくれるか

- UdonSharp の**言語制約**（使えない C# 機能・代替手段）を事前にロード
- `UdonSynced`、`FieldChangeCallback`、`NetworkCallable` など属性の正しい使い方を把握
- ネットワーク同期・オーナーシップ・プレイヤー API・永続化・データコンテナ・グラフィックス・ビデオ・MIDI 等の**全領域**を網羅
- VRChat 公式サンプルワールドの索引を保持
- Claude Code の [Skills 機能](https://docs.claude.com/en/docs/claude-code/skills) により、UdonSharp 関連キーワードで自動発動

## カバレッジ

- **creators.vrchat.com/worlds**：全 99 ページ
- **udonsharp.docs.vrchat.com**：全 13 ページ
- 合計 112 ページ + 手動注入 1 ページ = **113 ページ**を 28 本の references に再構成

進捗・出典は [`SOURCES.md`](SOURCES.md) 参照。

## ディレクトリ構成

```
vrc-udonsharp/
├── SKILL.md              エントリポイント（Claude が最初に読むファイル）
├── SOURCES.md            出典 URL 一覧・fetched 日時トラッキング
├── MAINTENANCE.md        更新ルーチン・トリガー・サイト再列挙手順
├── README.md             このファイル
├── rules/
│   ├── language.md       属性・sync mode・FieldChangeCallback 制約
│   └── basics.md         UdonSharpBehaviour 骨格・命名規則
└── references/
    ├── networking.md / sync-variables.md / network-events.md / ownership.md
    ├── players.md / data-containers.md / persistence.md
    ├── components.md / world-basics.md / vrc-graphics.md
    ├── input-events.md / ui-events.md / animation-events.md / avatar-events.md
    ├── events.md / web-loading.md / video-players.md / midi.md
    ├── event-execution-order.md / performance.md / editor-scripting.md
    ├── build-automation.md / debugging.md / clientsim.md / ai-navigation.md
    ├── udonsharp-meta.md / examples-index.md
    ├── network-id-utility.md / vrchat-api.md
```

## インストール

このスキルは Claude Code の `~/.claude/skills/` または プロジェクト配下の `.claude/skills/` に配置して使う。

### 方法 1：サブモジュール（推奨・Unity プロジェクトも git 管理している場合）

```bash
cd <your-unity-project>
git submodule add https://github.com/<your-org>/vrc-udonsharp.git .claude/skills/vrc-udonsharp
git submodule update --init
```

### 方法 2：junction / symlink（Unity プロジェクトを git 管理しない場合）

Windows の例（コマンドプロンプト、管理者不要の junction）：

```cmd
mklink /J "C:\path\to\UnityProject\.claude\skills\vrc-udonsharp" "C:\path\to\cloned\vrc-udonsharp"
```

macOS / Linux：

```bash
ln -s /path/to/cloned/vrc-udonsharp /path/to/UnityProject/.claude/skills/vrc-udonsharp
```

### 方法 3：直接 clone

```bash
cd <your-unity-project>/.claude/skills/
git clone https://github.com/<your-org>/vrc-udonsharp.git
```

## 使い方

Claude Code のセッション中、以下のようなキーワードを含む質問で**自動的に発動**する：

- UdonSharp / Udon / UdonSharpBehaviour
- VRCPlayerApi / Networking.LocalPlayer / SendCustomEvent
- UdonSynced / C# to Udon / VRChat world scripting

発動条件は [`SKILL.md`](SKILL.md) の frontmatter `description` を参照。

手動発動する場合は Claude に「`vrc-udonsharp` スキルを使って」と指示すればよい。

## メンテナンス

VRChat SDK / UdonSharp のアップデートに追随する更新ルーチンは [`MAINTENANCE.md`](MAINTENANCE.md) に記載。主なトリガー：

- VRChat SDK / UdonSharp の新バージョンリリース
- VRChat Creators 公式ブログ・新機能告知
- 3〜6 ヶ月ごとのサイト再列挙（取りこぼし検出）
- 6 ヶ月〜1 年ごとの定期レビュー

更新履歴は `SOURCES.md` の「完了ログ」に Batch 単位で記録する。

## ライセンス

**MIT License** — Copyright (c) 2026 iwacchi

詳細は [`LICENSE`](LICENSE) を参照。

### 第三者帰属

`rules/` / `references/` 配下のドキュメントは、以下の公式ドキュメントを**出典とする二次的な要約・再構成**を含む：

- [VRChat Creators Documentation](https://creators.vrchat.com/worlds) — © VRChat Inc.
- [UdonSharp Documentation](https://udonsharp.docs.vrchat.com/) — Merlin および VRChat Inc.

コード例・API シグネチャ・挙動仕様は原典に帰属する。各 reference の出典 URL は [`SOURCES.md`](SOURCES.md) に記載。最新の正確な情報は必ず原典を確認すること。

本リポジトリ固有の**構造・日本語化・要約・再構成部分**が MIT ライセンスの対象。

## Disclaimer / 免責

本プロジェクトは**非公式の第三者プロジェクト**です。VRChat Inc. および UdonSharp 開発者（Merlin）との関連・承認・スポンサー関係はありません。「VRChat」「UdonSharp」およびそれに関連する商標は各権利者に帰属します。

This is an unofficial, third-party project. Not affiliated with, endorsed by, or sponsored by VRChat Inc. or the UdonSharp developer (Merlin). "VRChat", "UdonSharp", and related marks are property of their respective owners.

## 関連リンク

- [Claude Code Skills](https://docs.claude.com/en/docs/claude-code/skills)
- [VRChat Creators Documentation](https://creators.vrchat.com/worlds)
- [UdonSharp Documentation](https://udonsharp.docs.vrchat.com/)
- [VRChat Worlds SDK](https://creators.vrchat.com/sdk/)
