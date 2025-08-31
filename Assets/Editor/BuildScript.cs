using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEditor.Build.Reporting;

public class BuildScript
{
    // 빌드 결과물이 저장될 경로입니다. 
    // Dockerfile의 COPY 명령과 일치해야 합니다.
    // 현재 Dockerfile에 맞추어 "Build/Server"로 설정합니다.
    private static string BUILD_PATH = "Build/Server";
    
    // 서버 빌드 메서드입니다.
    // Unity 커맨드 라인에서 -executeMethod BuildScript.BuildServer 로 호출됩니다.
    public static void BuildServer()
    {
        Debug.Log("서버 빌드를 시작합니다...");

        // 빌드 경로 설정
        string buildLocation = Path.Combine(Application.dataPath, "..", BUILD_PATH);
        
        // 기존 빌드 폴더가 있다면 삭제하여 깨끗하게 다시 빌드합니다.
        if (Directory.Exists(buildLocation))
        {
            Directory.Delete(buildLocation, true);
            Debug.Log($"기존 빌드 폴더 삭제: {buildLocation}");
        }
        Directory.CreateDirectory(buildLocation); // 새 빌드 폴더 생성

        // 빌드에 포함될 씬들을 가져옵니다.
        // 현재 빌드 설정에 추가된 모든 씬을 사용합니다.
        string[] scenes = GetEnabledScenes();
        if (scenes.Length == 0)
        {
            Debug.LogError("빌드할 씬이 없습니다. Build Settings에 씬을 추가하세요.");
            EditorApplication.Exit(1); // 빌드 실패로 종료
            return;
        }

        // 빌드 옵션 설정
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.locationPathName = Path.Combine(buildLocation, GetServerExecutableName());
        
        // Linux 서버 빌드용 타겟 플랫폼
        buildPlayerOptions.target = BuildTarget.StandaloneLinux64; 
        
        // 빌드 옵션: 데디케이티드 서버 모드 활성화 (그래픽스 없이)
        buildPlayerOptions.options = BuildOptions.CompressWithLz4HC | BuildOptions.EnableHeadlessMode | BuildOptions.AllowDebugging;
        // BuildOptions.EnableHeadlessMode는 BuildOptions.ServerBuild와 동일한 기능을 수행합니다.
        // BuildOptions.AllowDebugging은 디버깅을 위해 유용합니다.

        Debug.Log($"빌드 타겟: {buildPlayerOptions.target}");
        Debug.Log($"빌드 경로: {buildPlayerOptions.locationPathName}");
        Debug.Log($"빌드 옵션: {buildPlayerOptions.options}");
        Debug.Log($"빌드할 씬 수: {scenes.Length}");

        // 빌드 실행
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);

        // 빌드 결과 확인
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"서버 빌드 성공! 총 시간: {report.summary.totalTime} | 크기: {report.summary.totalSize} 바이트");
            EditorApplication.Exit(0); // 성공적으로 종료
        }
        else if (report.summary.result == BuildResult.Failed)
        {
            Debug.LogError($"서버 빌드 실패! 오류: {report.summary.totalErrors}");
            EditorApplication.Exit(1); // 빌드 실패로 종료
        }
        else
        {
            Debug.LogWarning($"서버 빌드 경고 또는 취소됨: {report.summary.result}");
            EditorApplication.Exit(0); // 경고나 취소는 성공으로 간주
        }
    }

    // Build Settings에 활성화된 씬들의 경로를 가져오는 헬퍼 메서드
    private static string[] GetEnabledScenes()
    {
        System.Collections.Generic.List<string> scenes = new System.Collections.Generic.List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                scenes.Add(scene.path);
            }
        }
        return scenes.ToArray();
    }

    // 서버 실행 파일의 이름을 결정합니다.
    // Linux 빌드의 경우 일반적으로 ".x86_64" 확장자가 붙습니다.
    private static string GetServerExecutableName()
    {
        // Application.productName은 프로젝트의 Product Name 설정 값을 가져옵니다.
        return Application.productName + ".x86_64"; 
    }
}
