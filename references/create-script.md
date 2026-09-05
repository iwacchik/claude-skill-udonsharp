# Create UdonSharp Script（.cs + .asset セット作成）

UdonSharp の新規スクリプトは **`.cs` 単独では機能しない**。同フォルダ・同名の `UdonSharpProgramAsset`（`.asset`）が必要で、`sourceCsScript` 経由で `.cs` の MonoScript を参照する。

UdonSharp 自体には programmatic に作成する公開 API は存在せず、唯一の正規実装は `UdonSharpBehaviourEditor.CreateUSharpScript()`（`Assets > Create > U# Script` メニュー）。SDK 3.10.5 では `internal static string CreateUSharpScript(string folderPath, bool createProgramAsset)` に分離され、作成した `.cs` のプロジェクト相対パスを返すようになったが、依然として public ではない。このドキュメントはその中核ロジックを **MCP for Unity の `execute_code`** 経由で実行する手順を提供する。

公式実装の場所: `Packages/com.vrchat.worlds/Integrations/UdonSharp/Editor/Editors/UdonSharpBehaviourEditor.cs:80-185`（SDK 3.10.5 時点）

## 配置先の制約（SDK 3.10.5）

- 公式ダイアログは **`Assets/` または `Packages/` 配下**のみ許可する（3.10.5-beta.1 で `Assets/` 限定に制限 → beta.2 で `Packages/` を再許可）。プロジェクト外のパスは「Invalid path」ダイアログで明示的に拒否される
- `Packages/` 配下では `AssetDatabase.CreateAsset` が直接使えないため、公式実装は `.asset` を一旦 `Assets/pkgUdon_<GUID>.asset` に作成してから `AssetDatabase.MoveAsset` で移動する。本ドキュメントのスニペットは `folder` が `Assets/` 配下である前提で書かれている（`Packages/` 配下に作る場合は同じ「Assets に作成 → MoveAsset」が必要）

## 何ができるか

```
<folder>/
├── <ClassName>.cs            ← UdonSharpBehaviour テンプレート
├── <ClassName>.cs.meta       ← Unity が AssetDatabase 経由で自動生成
├── <ClassName>.asset         ← UdonSharpProgramAsset（sourceCsScript で .cs を参照）
└── <ClassName>.asset.meta    ← Unity 自動生成
```

`.meta` 2 つは **Unity が `AssetDatabase` API 経由の作成時に自動生成する**。手動で書くと GUID 衝突や crc 不整合を起こすので避ける。

## 前提

- Unity が起動している
- `Window > MCP for Unity > Start Server` でサーバ稼働中
- Claude Code セッションに `mcp__UnityMCP__execute_code` が読み込まれている

## 単一スクリプト作成

`snippets/create_udonsharp_script.cs` のスニペットを `execute_code` の `code` パラメータに渡す。先頭の 3 行（`scriptName` / `folder` / `body`）を書き換えて呼ぶ。

戻り値: `{ csPath, assetPath, csGuid, assetGuid }`

## 複数スクリプト一括作成

`snippets/create_udonsharp_script_batch.cs` を使う。`scripts` 配列を編集して呼ぶと `AssetDatabase.StartAssetEditing` / `StopAssetEditing` で 1 トランザクションにまとめる。Reversi 等の関連スクリプト群を一気に作るのに使う。

戻り値: `{ count, results: [...] }`

## execute_code の wrapper 仕様

execute_code はユーザーコードを以下の wrapper に挿入する:

```csharp
using System; using System.Collections.Generic; using System.Linq;
using System.Reflection; using UnityEngine; using UnityEditor;
public static class MCPDynamicCode {
    public static object Execute() {
        // ここに渡したコードが入る
    }
}
```

メソッドボディに挿入されるため `using` 追加は不可。`System.IO` / `UdonSharp` 等は完全修飾名で参照する（`UnityEngine` / `UnityEditor` のみ wrapper が import 済み）。

## カスタムボディを渡す

スニペットの `body` パラメータが空なら空 `Start()` テンプレを生成。`[UdonSynced]` フィールド入りなど具体的な実装を含めたいときは C# クラス全体を `body` に文字列で入れる。**`body` を渡したときは scriptName と class 名の一致は呼び出し側の責任**。

## execute_code が使えない場合

代替として「Editor 拡張をプロジェクトに置く」方式。同じロジックを `Assets/.../Editor/UdonSharpScriptCreator.cs` に書いて `[MenuItem]` で叩く。スニペット内容はほぼコピー可能（`return` を `Debug.Log` に置き換える）。

## 動作確認のチェックリスト

実行後:
1. Project ウィンドウで `<ClassName>.cs` の隣に `<ClassName>` という U# Program Asset アイコン（緑 #）が表示される
2. `<ClassName>.asset` を選択 → Inspector で `Source C# Script: <ClassName>` が表示される
3. `Assets/SerializedUdonPrograms/<asset-guid>.asset` がコンパイル後に自動生成される
4. `gameObject.AddUdonSharpComponent<<ClassName>>()` が `Unable to find valid U# program asset` エラーを出さない

## やってはいけないこと

- `.cs.meta` / `.asset.meta` を手書きする → GUID 重複や Unity の inconsistency 警告の原因
- `.asset` を YAML 直書きする → `serializedUdonProgramAsset` のリンクや `serializationData` を Unity 側に再生成させる必要があり副作用が読みにくい
- `.cs` だけ作って `.asset` は Unity に自動生成させる期待 → **`UdonSharpProgramAssetPostprocessor` は `.cs` を見ない**ため自動生成されない（実装確認済み）
- `manage_script` の `create` だけで完結すると思う → `.cs` を書いて refresh するだけで `.asset` は作らない

## 既知の注意点

スニペット実行後、コンソールに `ArgumentNullException`（`VRC.Editor.EnvConfig.EditorUpdate()` → `SetAudioSettings()` → `AssetDatabase.SaveAssets()` 由来）が出ることがある。これは VRChat SDK 側の周期処理がアセット作成・U# コンパイルと競合して起きる cosmetic な例外で、スニペットのコードはスタックに登場しない。`UdonSharpProgramAsset` の生成・`sourceCsScript` リンク・`SerializedUdonProgram` 生成には実害がないため、スニペット側の対策は不要。
