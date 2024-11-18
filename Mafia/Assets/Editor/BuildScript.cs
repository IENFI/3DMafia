using UnityEditor;
using UnityEngine;

public class BuildScript
{
    [MenuItem("Build/Build Test Environment")]
    public static void BuildTest()
    {
        string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);

        try
        {
            // TEST_ENVIRONMENT 추가
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, currentSymbols + ";TEST_ENVIRONMENT");

            // 빌드 옵션 설정
            string[] scenes = { "Assets/Scenes/ServerScene.unity", "Assets/Scenes/Level_0.unity", "Assets/Scenes/Level_1.unity" };
            string outputPath = "Builds/TestEnvironmentBuild.exe";

            // 빌드 실행
            BuildPipeline.BuildPlayer(scenes, outputPath, BuildTarget.StandaloneOSX, BuildOptions.None);
            Debug.Log("Test Environment Build completed.");
        }
        finally
        {
            // 원래 상태 복원
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, currentSymbols);
        }
    }

    [MenuItem("Build/Build Production")]
    public static void BuildProduction()
    {
        // 빌드 옵션 설정
        string[] scenes = { "Assets/Scenes/ServerScene.unity", "Assets/Scenes/Level_0.unity", "Assets/Scenes/Level_1.unity" };
        string outputPath = "Builds/ProductionBuild.exe";

        // 빌드 실행
        BuildPipeline.BuildPlayer(scenes, outputPath, BuildTarget.StandaloneOSX, BuildOptions.None);
        Debug.Log("Production Build completed.");
    }
}
