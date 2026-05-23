// ============================================================
// create_udonsharp_script.cs  ―  単一 U# スクリプト作成スニペット
//
// MCP for Unity の execute_code の `code` パラメータに丸ごと渡して使う。
// 下の「書き換えパラメータ」3 つ（scriptName / folder / body）を編集してから実行する。
//
// 戻り値: { csPath, assetPath, csGuid, assetGuid }
//
// 制約（execute_code の wrapper にメソッドボディとして挿入されるため）:
//   - using の追加は不可
//   - import 済み: System / System.Collections.Generic / System.Linq /
//                  System.Reflection / UnityEngine / UnityEditor
//   - System.IO / UdonSharp は完全修飾名で参照する
// ============================================================

// --- 書き換えパラメータ ---------------------------------------
string scriptName = "TestUdonScript";                 // クラス名 = ファイル名
string folder     = "Assets/Prototype/Test/Scripts";  // 配置先（Assets/ からの相対）
string body       = "";                               // 空なら空の Start() テンプレを生成。
                                                       // 具体実装を入れたいときは C# クラス全体を文字列で渡す
                                                       // （その場合 scriptName とクラス名の一致は呼び出し側の責任）
// --------------------------------------------------------------

// フォルダを 1 階層ずつ AssetDatabase 経由で作成（.meta の整合性を保つため手動作成しない）
string[] parts = folder.Split('/');
string cur = parts[0];
for (int i = 1; i < parts.Length; i++)
{
    string next = cur + "/" + parts[i];
    if (!AssetDatabase.IsValidFolder(next))
        AssetDatabase.CreateFolder(cur, parts[i]);
    cur = next;
}

string csPath    = folder + "/" + scriptName + ".cs";
string assetPath = folder + "/" + scriptName + ".asset";

// body 未指定なら UdonSharpBehaviour の標準テンプレを生成
string fileContents = string.IsNullOrEmpty(body)
    ? "using UdonSharp;\n" +
      "using UnityEngine;\n" +
      "using VRC.SDKBase;\n" +
      "using VRC.Udon;\n" +
      "\n" +
      "public class " + scriptName + " : UdonSharpBehaviour\n" +
      "{\n" +
      "    void Start()\n" +
      "    {\n" +
      "        \n" +
      "    }\n" +
      "}\n"
    : body;

// .cs を書き出して同期インポート → MonoScript を取得
System.IO.File.WriteAllText(csPath, fileContents, System.Text.Encoding.UTF8);
AssetDatabase.ImportAsset(csPath, ImportAssetOptions.ForceSynchronousImport);
MonoScript newScript = AssetDatabase.LoadAssetAtPath<MonoScript>(csPath);

// UdonSharpProgramAsset を生成し sourceCsScript で .cs を参照させる
// （UdonSharpBehaviourEditor.CreateUSharpScript と同じ正規経路）
var programAsset = ScriptableObject.CreateInstance<UdonSharp.UdonSharpProgramAsset>();
programAsset.sourceCsScript = newScript;
AssetDatabase.CreateAsset(programAsset, assetPath);
AssetDatabase.Refresh();

return new
{
    csPath    = csPath,
    assetPath = assetPath,
    csGuid    = AssetDatabase.AssetPathToGUID(csPath),
    assetGuid = AssetDatabase.AssetPathToGUID(assetPath),
};
