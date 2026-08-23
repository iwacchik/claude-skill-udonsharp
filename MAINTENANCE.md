# Maintenance & Update Routine

vrc-udonsharp スキルを VRChat / UdonSharp 公式ドキュメントの更新に追随させるためのルーチン手順。
新しい会話・別セッションでも再現できるように、**Claude に読ませる前提**で書いている。

## いつ更新するか（トリガー）

| トリガー | 頻度 | 何をするか |
|---------|------|-----------|
| **VRChat SDK の新バージョンリリース** | 随時 | リリースノートで触れられたページを再 fetch |
| **UdonSharp の新バージョンリリース** | 随時 | `udonsharp-meta.md` の News を更新、変更点のあるページ fetch |
| **VRChat Creators ブログ / 新機能告知** | 随時 | 追加された新ページがあれば SOURCES.md に追加して fetch |
| **定期レビュー** | 6 ヶ月〜1 年 | 主要ページをサンプリングで再 fetch して diff |
| **サイト再列挙（取りこぼし検出）** | 3 ヶ月〜半年 | **サイト構造全体を再列挙**して SOURCES.md と照合、未登録ページを発見 |
| **スキル利用中の発見**（「この API が書いてない」等） | 随時 | 該当ページを再 fetch |

## 更新ルーチン（5 ステップ）

### Step 1：変更の検知

次のソースをチェック：

- **VRChat Creators トップ**：https://creators.vrchat.com/worlds — 新しいサイドバー項目が増えていないか
- **UdonSharp News**：https://udonsharp.docs.vrchat.com/news — リリース履歴
- **VRChat 公式リリースノート**：https://docs.vrchat.com/release-notes
- **VRChat SDK パッケージの CHANGELOG**（`Packages/com.vrchat.worlds/CHANGELOG.md`）

### Step 2：更新対象ページの特定

- 変更があったページ、または新規ページを `SOURCES.md` にリストアップ
- 既存ページの再 fetch は `SOURCES.md` の各行に書いてある **fetched 日時**が古いもの（例：半年以上前）から着手

#### 補足：サイト再列挙（取りこぼし検出）

初回の一括注入時でも「全域網羅」できていない場合がある（Batch 16 で `/vrchat-api` と `/events` を発見した事例）。再列挙の手順：

1. `WebFetch(サイトのトップ URL, "サイドバー / ナビゲーション / 目次に含まれる全ページの URL を列挙")` で構造を取得
2. 取得したリストを `SOURCES.md` の既存エントリと照合
3. 未登録ページを抽出 → Step 3 へ

対象サイト：`creators.vrchat.com/worlds` と `udonsharp.docs.vrchat.com` の両方。

**推奨：sitemap.xml で機械的に列挙する**（2026-07-18 Batch 17 で確立）：

```bash
curl -sL -A 'vrc-udonsharp-skill-updater/1.0 (contact: <メールアドレス>)' \
  https://creators.vrchat.com/sitemap.xml -o sitemap.xml
```

- 全ページ URL が漏れなく取れるため、WebFetch のナビゲーション要約（不完全になりがち）より確実
- **User-Agent 必須**：UA なしは Cloudflare WAF が 403（waf_code 13799）で弾く。アプリ名 + 連絡先を含めること
- 取得した URL 一覧を SOURCES.md と `comm` 等で突合して未登録ページを抽出する
- なお udonsharp.docs.vrchat.com は udonsharp.dev へリダイレクトされるようになった。UdonSharp ドキュメントは creators.vrchat.com の `/worlds/udon/udonsharp/` にも移設済み


### Step 3：WebFetch で取得

対象 URL を `WebFetch` で取得。プロンプトは**フェッチしたい具体的な情報**を列挙すると精度が上がる（過去ログに良い例多数）。

```
WebFetch(URL, "○○について全て抽出してください：
1. API の完全なシグネチャ
2. 制約と注意点
3. サンプルコード全部
省略せず詳細に。")
```

### Step 4：差分を反映

- 既存の `references/*.md` の該当セクションを Read
- **新情報 / 変更点**を特定（古くなった記述、追加された API、廃止機能）
- `Edit` で最小限の差分を適用するか、内容が大きく変わるなら `Write` で全面書き換え
- 該当ファイルがなければ新規作成（必要に応じて `SKILL.md` の references リストに追加）

### Step 5：SOURCES.md を更新

- 処理したページの行を `[x] URL (fetched: YYYY-MM-DD)` に更新
- `## 完了ログ` の末尾に `YYYY-MM-DD Batch: 概要` を追記
- 進捗カウントを更新

## SOURCES.md の拡張フォーマット

各ページの状態は以下の記法で管理：

```
- [x] URL (fetched: 2026-04-21)            — 処理済み
- [ ] URL                                   — 未処理
- [?] URL                                   — 取得失敗（手動注入待ち）
- [-] URL                                   — 対象外
```

fetched 日時を書いておくと、**古いものから再 fetch する運用**が取れる。

## 差分が大きいときの判断

| 状況 | アクション |
|------|----------|
| 誤字・細かい表現修正程度 | 触らなくても良い |
| API が追加された | 該当セクションに追記 |
| API が廃止された | 旧 API の記述を削除、代替を明記 |
| 構造が大きく変わった | `Write` で全面書き換え、該当 reference を再構築 |
| 新しいトピック領域 | 新規 `references/X.md` を作成、`SKILL.md` に導線追加 |

## 注意点

### WebFetch の限界

- **要約ベースで取得される**ため、具体的な API シグネチャや数値を取りたい場合はプロンプトで明示指定
- **SVG 図表はテキスト化されない**（Event Execution Order の公式図など）
- **複数 DOM スラッグで同じページを指す**ケースあり（過去 network-id-utility で発生、最終的に手動 HTML 保存で解決）

### 手動注入パターン

WebFetch で取得不可な場合：

1. ユーザーに該当ページを HTML 保存してもらう
2. 保存パスを教えてもらう
3. `Read` / `Grep` で内容抽出
4. 通常通り `references/*.md` に反映

### legacy 版との共存

- 初期プロトタイプ版は `vrc-udonsharp-legacy` にリネームして退避済み（正規版は本スキル `vrc-udonsharp`）
- `.claude/skills/` に両方置いたままだと Claude Code がどちらも読む可能性があるため、本運用では legacy 側の `SKILL.md` frontmatter `name` を変更するか、ディレクトリごと削除する

## 典型的な更新例

### 例 1：UdonSharp 1.2.0 リリース

1. `udonsharp.docs.vrchat.com/news` を fetch
2. 追加/廃止機能を特定
3. 関連する `references/*.md`（例：language.md、sync-variables.md）を更新
4. `udonsharp-meta.md` の News 表に新バージョン行を追加
5. SOURCES.md の該当行の fetched 日時を更新

### 例 2：新しい VRCGraphics API 追加

1. `creators.vrchat.com/worlds/udon/vrc-graphics/` を fetch
2. 新サブページが増えていたら `SOURCES.md` にエントリ追加
3. 新サブページを fetch
4. `references/vrc-graphics.md` に追記
5. SOURCES.md 更新

### 例 3：同期挙動が変わった（ユーザー発見）

1. ユーザーから「Manual sync の挙動が変わったっぽい」と報告
2. `creators.vrchat.com/worlds/udon/networking/variables` と `network-details` を fetch
3. `references/sync-variables.md` / `networking.md` を diff して更新
4. SOURCES.md の該当行の fetched 日時を更新

### 例 4：再列挙で取りこぼし発見（2026-04-21 Batch 16 の実例）

1. `WebFetch(https://udonsharp.docs.vrchat.com/, "サイドバーとトップページから辿れる全ページ URL を列挙")` で 13 ページ検出
2. SOURCES.md の udonsharp.docs セクションと照合 → 11 ページのみ登録、`/vrchat-api` と `/events` が未登録と判明
3. 2 ページを WebFetch して `references/vrchat-api.md`（新規）と `references/events.md`（追記）に反映
4. SOURCES.md に 2 行追加 `(fetched: 2026-04-21)`、完了ログに Batch 16 追記
5. SKILL.md の references リストにも新規ファイルを追加

## Claude への指示テンプレ（再利用可）

別セッションで更新作業をやるときは、これをコピペして指示する：

```
.claude/skills/vrc-udonsharp/MAINTENANCE.md を読んでルーチンに従い、
以下のページを更新してください：
- URL1
- URL2
...

処理：各 URL を WebFetch → 既存 references/*.md との diff → 更新 → SOURCES.md の fetched 日時更新。
```

## doc 同期対象外ファイル

- `references/udon-assembly-optimization.md` — 実測由来の独自知見（手動管理）。更新バッチでは上書き・再 fetch の対象にしないこと。
