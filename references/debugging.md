# Debugging（Udon プロジェクトのデバッグ手法）

## ログファイル

VRChat クライアントの出力ログ：

```
C:\Users\{UserName}\AppData\LocalLow\VRChat\VRChat\output_log_HH-MM-SS.txt
```

セッションごとに新規ファイル。`Debug.Log` / `Debug.LogWarning` / `Debug.LogError` は全部ここに記録される。UdonSharp のランタイム例外もここに出る。

## 画面内ログオーバーレイ

- **`RShift + Backtick + 3`** でオーバーレイ表示
- デバッグ GUI 有効時のみ利用可能

## 起動フラグ（デバッグ推奨）

```
VRChat.exe --no-vr --enable-debug-gui --enable-udon-debug-logging
```

設定方法 3 種：
1. **VRC Quick Launcher**（Creator Companion の Tools タブ）— 最も手軽
2. **バッチファイル**（`debug.bat` に上記コマンドを書く）
3. **Steam 起動オプション**

## エラー解析のコツ

- ログから `"halted"` を grep すると UdonBehaviour の致命的エラーが見つかる
- エラー文言（`Object reference not set` 等）からアサイン漏れ等を特定

## UdonSharp ランタイム例外ウォッチャー

- **デフォルト有効**（`Edit > Project Settings > Udon Sharp > Listen for client exceptions`）
- 実行時例外を Unity Console に表示
- **エラー行番号 + 文字位置**を報告

## パフォーマンス注意

デバッグ機能は実行コストがかかる。**本番前に OFF にする**。ログファイルも肥大化する。

## デバッグ Log の運用

ユニークなタグで grep しやすく：

```csharp
Debug.Log("[JumpCounter] Interact: value=" + _count);
```

## 同期挙動の検証

1. `OnPreSerialization` / `OnPostSerialization` / `OnDeserialization` に Debug.Log
2. Build & Test で 2 クライアント起動
3. 片方で操作し、両方のログを比較
4. `OnPostSerialization.byteCount` で送信サイズ記録

## ネットワーク統計 API（`VRC.SDK3.Network.Stats`）

帯域・遅延の定量把握。`references/networking.md` に詳細。

## World Debug Views（ワールド内デバッグツール）

Quick Menu Settings から開ける診断ツール群。**`Right Shift + ~ + N`**（数字キー）のショートカットで個別ページ表示。

### 主要ページ

| ページ | 内容 |
|-------|------|
| AssetBundle / Memory | ロード済みアセットとメモリ使用量 |
| Version / Info | VRChat ビルド情報とホットキー一覧 |
| Log Viewer | 出力ログ、Udon クラッシュ、デバッグメッセージ |
| Players | プレイヤー統計（master / VR モード / ネットワーク間隔） |
| **Net Objects** | **Network 対象オブジェクトのオーナー情報と帯域情報**（最重要） |
| Audio Sources | アクティブな音声コンポーネント |

### オーバーレイ表示

PhysBones、ネットワークオブジェクト統計、プレイヤー情報、UI Shape アウトラインを**ワールド内に直接描画**。

### 公開権限

**`World Debugging` を有効にする**ことで、他のプレイヤーにも診断ツール利用を許可できる（デフォルトでは一部制限あり）。

## Udon Moderation Tool Guidelines（参照のみ）

Udon 製モデレーションツールに関するガイドラインは、**VRChat Creator Guidelines の「Worlds」セクション**に集約されている：

- https://hello.vrchat.com/creator-guidelines

ワールド規約を超えた moderation tool 設計時はこちらを参照。
