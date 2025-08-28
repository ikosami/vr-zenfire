using UnityEditor;
using UnityEditor.Compilation;

public class CsprojRefresher
{
    [MenuItem("Tools/Regenerate .csproj Files")]
    public static void RegenerateCsprojFiles()
    {
        CompilationPipeline.RequestScriptCompilation();
        AssetDatabase.Refresh();
    }


    // Cursorを使うときは、External Script Editor Argsに"$(ProjectPath) -g $(File):$(Line)"を指定する
    // これは、Cursorのデフォルトのコマンドで、$(ProjectPath)はプロジェクトのパス、$(File)は開いているファイルのパス、$(Line)は開いているファイルの行数を表します。
    [MenuItem("Tools/Cursor/Set Cursor External Script Editor Args")]
    public static void SetCursorExternalScriptEditorArgs()
    {
        string args = "$(ProjectPath) -g $(File):$(Line)";
        EditorPrefs.SetString("ExternalScriptEditorArgs", args);
    }

}