#!/bin/bash

# --- 2. Docker 컨테이너 정리 및 실행 ---
echo "--- Docker 컨테이너 정리 시작 ---"
# 실행 중인 컨테이너가 없어도 에러가 발생하지 않도록 리다이렉트
docker stop uniserver001 > /dev/null 2>&1 || true
docker rm uniserver001 > /dev/null 2>&1 || true
echo "--- Docker 컨테이너 정리 완료 ---"

echo "--- Docker 이미지 빌드 시작 ---"
docker build -t unity-server .
echo "--- Docker 이미지 빌드 완료 ---"

echo "--- Docker 컨테이너 실행 중 (포트 7777 UDP/TCP 매핑) ---"
docker run --name uniserver001 -p 7777:7777/udp -p 7777:7777/tcp unity-server
echo "--- Docker 컨테이너 실행 완료 ---"
