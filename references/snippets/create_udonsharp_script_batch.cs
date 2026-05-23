// ============================================================
// create_udonsharp_script_batch.cs  ―  複数 U# スクリプト一括作成スニペット
//
// MCP for Unity の execute_code の `code` パラメータに丸ごと渡して使う。
// 下の `scripts` 配列を編集してから実行する。Reversi 等の関連スクリプト群を
// AssetDatabase.StartAssetEditing / StopAssetEditing で 1 トランザクションにまとめる。
//
// 戻り値: { count, results: [ { name, csPath, assetPath, csGuid, assetGuid }, ... ] }
//
// 制約（execute_code の wrapper にメソッドボディとして挿入されるため）:
//   - using の追加は不可
//   - import 済み: System / System.Collections.Generic / System.Linq /
//                  System.Reflection / UnityEngine / UnityEditor
//   - System.IO / UdonSharp は完全修飾名で参照する
// ============================================================

// --- 書き換えパラメータ ---------------------------------------
// body が空なら空の Start() テンプレを生成。具体実装は C# クラス全体を文字列で渡す。
var scripts = new[]
{
    new { name = "Foo", folder = "Assets/Prototype/Test/Scripts", body = "" },
    new { name = "Bar", folder = "Assets/Prototype/Test/Scripts", body = "" },
};
// --------------------------------------------------------------

// パス 1: 全フォルダ作成 + 全 .cs 書き出しを 1 トランザクションで実行。
// StartAssetEditing 中は ImportAsset が遅延されるため、ここでは MonoScript は取得しない。
AssetDatabase.StartAssetEditing();
try
{
    foreach (var s in scripts)
    {
        string[] parts = s.folder.Split('/');
        string cur = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            string next = cur + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(cur, parts[i]);
            cur = next;
        }

        string csPath = s.folder + "/" + s.name + ".cs";
        string fileContents = string.IsNullOrEmpty(s.body)
            ? "using UdonSharp;\n" +
              "using UnityEngine;\n" +
              "using VRC.SDKBase;\n" +
              "using VRC.Udon;\n" +
              "\n" +
              "public class " + s.name + " : UdonSharpBehaviour\n" +
              "{\n" +
              "    void Start()\n" +
              "    {\n" +
              "        \n" +
              "    }\n" +
              "}\n"
            : s.body;

        System.IO.File.WriteAllText(csPath, fileContents, System.Text.Encoding.UTF8);
        AssetDatabase.ImportAsset(csPath, ImportAssetOptions.ForceSynchronousImport);
    }
}
finally
{
    AssetDatabase.StopAssetEditing();
}

// パス 2: import が flush された後に MonoScript をロードし .asset を生成。
AssetDatabase.StartAssetEditing();
try
{
    foreach (var s in scripts)
    {
        string csPath    = s.folder + "/" + s.name + ".cs";
        string assetPath = s.folder + "/" + s.name + ".asset";
        MonoScript newScript = AssetDatabase.LoadAssetAtPath<MonoScript>(csPath);
        var programAsset = ScriptableObject.CreateInstance<UdonSharp.UdonSharpProgramAsset>();
        programAsset.sourceCsScript = newScript;
        AssetDatabase.CreateAsset(programAsset, assetPath);
    }
}
finally
{
    AssetDatabase.StopAssetEditing();
}
AssetDatabase.Refresh();

var results = new List<object>();
foreach (var s in scripts)
{
    string csPath    = s.folder + "/" + s.name + ".cs";
    string assetPath = s.folder + "/" + s.name + ".asset";
    results.Add(new
    {
        name      = s.name,
        csPath    = csPath,
        assetPath = assetPath,
        csGuid    = AssetDatabase.AssetPathToGUID(csPath),
        assetGuid = AssetDatabase.AssetPathToGUID(assetPath),
    });
}

return new { count = results.Count, results = results };
