FROM ubuntu:22.04

RUN apt update && apt install -y libglib2.0-0 libx11-6 libxcursor1 libxrandr2 libxi6 libgconf-2-4 libnss3 libasound2 libatk1.0-0 libgtk-3-0 libxss1 libxcomposite1 libxdamage1

COPY ./Build/Server /app
WORKDIR /app

EXPOSE 7777/udp
EXPOSE 7777/tcp

CMD ["./MyUnityServer.x86_64", "-batchmode", "-nographics", "-logfile", "/dev/stdout"]