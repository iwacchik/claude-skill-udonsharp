# Build & Test（ワールドのローカルテスト）

VRChat ワールドを VRChat クライアントで実際にテストする。Unity の Play mode だけでは検証できない機能（アバター、ネットワーク等）を確認するのに必須。

## 初期セットアップ

1. **VRChat SDK Control Panel を開く**（`VRChat SDK > Show Control Panel`）
2. サインイン
3. **Settings タブ**で `VRChat.exe` のパスを設定
4. **Builder タブ**で以下を実行：
   - `Setup Layers for VRChat`
   - `Set Collision Matrix`

## 単独クライアント起動

1. `Force Non-VR` を ON（Desktop モードで軽量起動）
2. **Build & Test** ボタンをクリック

## 複数クライアント（ネットワーク機能のテスト）

- Builder タブで **Number of Clients** を 2 以上に設定
- Build & Test 実行で N クライアントが同一ワールドに入室
- **最初のクライアントが instance master** になり、synced 変数を更新できる

## Build & Reload（クライアント再起動なしでリロード）

- **Number of Clients = 0** で Build & Test を実行
- 起動中のクライアントが新ビルドに自動切替

### `--watch-worlds` フラグ

```
VRChat.exe --watch-worlds --profile=0 --no-vr
```

起動中クライアントは**ビルド完了時に自動でローカルインスタンスに再接続**する。開発の高速反復に。

## 既知の制約

- **「Build & Reload 後に追加した Build & Test クライアントは、同じルームに自動参加しない**」ので、客数を 0 に戻してから再度リロードすると揃う
- Build & Publish（オンライン公開）は**必ずユーザー確認**を経ること。自動化 skill では触らない

## プログラマティック実行（エディタスクリプト）

```csharp
using VRC.SDK3.Editor;

UnityEditor.EditorPrefs.SetInt("numClients", 2);

if (VRCSdkControlPanel.TryGetBuilder<IVRCSdkWorldBuilderApi>(out var builder))
{
    var task = builder.BuildAndTest();
}
```

`IVRCSdkWorldBuilderApi` の主要メソッド：

| メソッド | 安全性 |
|---------|-------|
| `Build()` | 安全（ビルドのみ） |
| `BuildAndTest()` | 安全（ローカルテスト） |
| `TestLastBuild()` | 安全（既存ビルドを起動） |
| `BuildAndUpload(...)` | **破壊的（公開）** |

Build & Publish 系は必ずユーザー確認を経てから呼ぶ。CI では承認プロセス経由に限定。
