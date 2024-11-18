using UnityEditor;
using UnityEngine;

public class BuildScript
{
    [MenuItem("Build/Build Test Environment (Mac OS)")]
    public static void BuildTestMacOS()
    {
        string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);

        try
        {
            // TEST_ENVIRONMENT 추가
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, currentSymbols + ";TEST_ENVIRONMENT");

            // 빌드 옵션 설정
            string[] scenes = { "Assets/Scenes/ServerScene.unity", "Assets/Scenes/Level_0.unity", "Assets/Scenes/Level_1.unity" };
            string outputPath = "Builds/TestEnvironmentBuild_MacOS.app";

            // 빌드 옵션: Development Build 활성화
            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputPath,
                target = BuildTarget.StandaloneOSX,
                options = BuildOptions.Development | BuildOptions.AllowDebugging
            };

            // 빌드 실행
            BuildPipeline.BuildPlayer(buildOptions);
            Debug.Log("Test Environment Build completed.");
        }
        finally
        {
            // 원래 상태 복원
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, currentSymbols);
        }
    }

    // 테스트 환경 빌드 (Windows)
    [MenuItem("Build/Build Test Environment (Windows)")]
    public static void BuildTestWindows()
    {
        string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);

        try
        {
            // TEST_ENVIRONMENT 추가
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, currentSymbols + ";TEST_ENVIRONMENT");

            // 빌드 옵션 설정
            string[] scenes = { "Assets/Scenes/ServerScene.unity", "Assets/Scenes/Level_0.unity", "Assets/Scenes/Level_1.unity" };
            string outputPath = "Builds/TestEnvironmentBuild_Windows.exe"; // Windows용 .exe 경로

            // 빌드 옵션: Development Build 활성화
            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development | BuildOptions.AllowDebugging
            };

            // 빌드 실행
            BuildPipeline.BuildPlayer(buildOptions);
            Debug.Log("Test Environment Build (Windows) completed.");
        }
        finally
        {
            // 원래 상태 복원
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, currentSymbols);
        }
    }
    

    [MenuItem("Build/Build Production (MacOS)")]
    public static void BuildProductionMacOS()
    {
        // 빌드 옵션 설정
        string[] scenes = { "Assets/Scenes/ServerScene.unity", "Assets/Scenes/Level_0.unity", "Assets/Scenes/Level_1.unity" };
        string outputPath = "Builds/ProductionBuild_MacOS.app";

        // 빌드 실행
        BuildPipeline.BuildPlayer(scenes, outputPath, BuildTarget.StandaloneOSX, BuildOptions.None);
        Debug.Log("Production Build completed.");
    }

    // 프로덕션 빌드 (Windows)
    [MenuItem("Build/Build Production (Windows)")]
    public static void BuildProductionWindows()
    {
        string[] scenes = { "Assets/Scenes/ServerScene.unity", "Assets/Scenes/Level_0.unity", "Assets/Scenes/Level_1.unity" };
        string outputPath = "Builds/ProductionBuild_Windows.exe";

        // 빌드 옵션 설정
        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputPath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None // 프로덕션 빌드, 추가 옵션 없음
        };

        // 빌드 실행
        BuildPipeline.BuildPlayer(buildOptions);
        Debug.Log("Production Build (Windows) completed.");
    }
}
