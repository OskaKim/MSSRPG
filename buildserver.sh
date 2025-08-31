#!/bin/bash

# --- 1. Unity 서버 빌드 ---
# UNITY_PATH를 유니티 에디터 실행 파일의 실제 경로로 설정하세요.
# 예시: UNITY_PATH="/Applications/Unity/Hub/Editor/2022.3.16f1/Unity.app/Contents/MacOS/Unity" (macOS)
# 예시: UNITY_PATH="/opt/Unity/Editor/Unity" (Linux)
# 예시: UNITY_PATH="C:\\Program Files\\Unity\\Hub\\Editor\\2022.3.16f1\\Editor\\Unity.exe" (Windows)
# 이 스크립트가 실행되는 곳이 유니티 프로젝트의 루트 폴더여야 합니다.
UNITY_PATH="E:\\6000.2.0b7\\Editor\\Unity.exe" # <--- !!! 여기에 실제 경로를 입력하세요 !!!

# 빌드 로그 파일 경로
BUILD_LOG_FILE="unity_build.log"

echo "--- Unity 서버 빌드 시작 ---"
"${UNITY_PATH}" -batchmode -nographics -quit -projectPath "$(pwd)" -executeMethod BuildScript.BuildServer -logFile "${BUILD_LOG_FILE}"

# 빌드 성공 여부 확인
if [ $? -ne 0 ]; then
    echo "Unity 서버 빌드 실패! 로그를 확인하세요: ${BUILD_LOG_FILE}"
    exit 1
fi
echo "--- Unity 서버 빌드 성공 ---"
