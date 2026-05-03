# UdonSharp Meta（Configuration / Migration / FAQ / Community）

UdonSharp のエディタ設定・バージョン履歴・コミュニティリソース。

## Project Settings

`Edit > Project Settings > Udon Sharp` で設定する。

### コンパイル設定

| 項目 | 内容 | 使いどき |
|------|------|---------|
| **Auto compile on modify** | ファイル保存時に自動コンパイル | 開発中は ON 推奨 |
| **Compile all script** | 変更検知時に全 U# をコンパイル | 依存関係が複雑なプロジェクト |
| **Compile on focus** | Editor フォーカス取得時のみコンパイル | コンパイル頻度を抑えたい時 |

### テンプレート

**Script template override**：カスタム `.cs` テンプレートを設定。`<TemplateClassName>` プレースホルダーでファイル名に置換される。

### デバッグ設定

| 項目 | 内容 |
|------|------|
| **Debug build** | `Inline Code` と `Listen for client exceptions` をまとめて ON/OFF |
| **Inline Code** | 生成アセンブリに C# ソースコードを含める（デバッグ可視性向上） |
| **Listen for client exceptions** | VRChat クライアントのログをプロジェクトに照合（**本番クライアントのエラー追跡**に有効） |

## Migration（v0.x → v1.0）

**UdonSharp 0.x は非推奨**。現在は v1.x が VCC 経由で配布される。

### v1.0 で追加された機能

- `static` メソッド、ジェネリックの一部対応
- `params` / `out` / `ref` / デフォルトパラメータ
- extension method
- **inheritance**、virtual / abstract クラス
- **partial class**
- **enum**
- 複数スクリプトの multi-edit（Inspector）
- **Full prefab support**（variant / instance / nested）
- Editor 拡張のシンプル化

### 破壊的変更

- **U# behaviour 名は `.cs` ファイル名と完全一致必須**
- 同じ program asset に複数 `.cs` を紐付け不可
- program asset に有効なスクリプト参照が必須
- Station / player イベントの古いオーバーロード廃止

### 推奨移行手順

1. **Creator Companion 使用**（自動的に依存解決、最推奨）

### 手動移行

1. プロジェクトをバックアップ
2. 旧フォルダ削除：`VRCSDK`, `Udon`, `UdonSharp`, `Gizmos/UdonSharp`
3. 新 World SDK と UdonSharp 1.1.x をインストール

### よくあるトラブル

| 問題 | 対処 |
|------|------|
| Nested prefab が壊れる | アップグレード前にアンパック or `UdonSharp > Force Upgrade` |
| Assembly definition の不一致 | 独自 assembly def には対応する U# assembly def が必要 |
| Newtonsoft.Json 重複 | `Assets/` 配下の重複 dll を削除、Package Manager 経由のみに |

## FAQ（よくある質問）

### サポートされている機能

**「Udon がサポートするなら UdonSharp もサポートする」** — 対応機能の詳細は GitHub の Class exposure tree で確認。

### Prefab

使用可能。ただし**Unity の仕様上、SerializedField の変更が prefab instance に正しく伝播しない**ことがある。

### プレイヤーのカメラ

**直接アクセス不可**。頭の位置・回転は `VRCPlayerApi.GetTrackingData(TrackingData.Head)` で取得する。

### 複数 UdonSharpBehaviour

**1 GameObject に複数配置可能**（ただし sync 挙動は共通化、`references/networking.md` 参照）。

### 古い FAQ の注記

FAQ には「Custom enum / Generic / Inheritance / Interface / Overload / Properties が未対応」と書かれている**が、これは UdonSharp v0.x 時代の情報**。v1.0 以降は enum・継承・プロパティ等サポート済み。`rules/language.md` の最新情報を優先。

## Community Resources

### 日本語

- **はつぇさんのブログ** — UdonSharp 入門記事
- **やぎりさんのブログ** — 走り書きメモ

### 英語

- **Vowgan** — YouTube チュートリアル（ボタン、インタラクト、プレイヤー操作）

### ツール・ライブラリ

| 名前 | 用途 |
|------|------|
| **CyanEmu** | VRChat クライアント エミュレータ（Editor 内テスト） |
| **orels1's UdonToolKit** | インスペクタ拡張・ユーティリティ |
| **Phasedragon's Input table** | 全 VR コントローラのバインディング一覧 |
| **cannorin's extern search** | Udon 利用可能関数検索（更新古い） |
| **Shatoo's debug tool** | ビルトインイベントを引数付きで呼び出す UI |
| **Jordo's Haptics Testing world** | ハプティクス調整スライダー |

## その他

- UdonSharp は **Open Beta** 段階との公式表記
- **ドキュメント未完成**項目あり — 完全な関数リストは GitHub リポジトリ参照

## News（リリース履歴）

| バージョン | 主な変更 |
|-----------|---------|
| **1.1.9**（2023/08） | **Avatar Scaling 用イベント定義**追加 |
| 1.1.8（2023/05） | SDK 3.2.0 互換化 |
| 1.1.7（2023/02） | **Image Loader / Text Loader イベント**追加 |
| 1.1.6（2022/12） | Nested prefab アップグレード改善、型変換バグ修正（float→int, char→float）、string 複合代入修正 |
| 1.1.5 | ユーザー定義 enum の配列同期修正 |
| 1.1.2（2022/10） | 同期型チェック機能追加 |

詳細は公式ブログ参照。
