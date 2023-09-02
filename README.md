# GomokuOnlineServer
GomokuOnline은 다른 플레이어들과 실시간으로 오목을 즐길 수 있는 온라인 게임입니다. ([GomokuOnlineClient](https://github.com/aajuy/GomokuOnlineClient))

GomokuOnline의 서버는 로그인 서버, 매치 서버, 게임 서버, NetworkLibrary로 구성됩니다. 매치 참가 및 실제 게임 플레이는 TCP 소켓 통신을 통해 이루어지며, 다른 기능들은 HTTP 웹 요청을 통해 처리됩니다.
- LoginServer: 회원가입, 로그인을 담당합니다.
- MatchServer: 매치 참가, 결과 기록, 랭킹을 담당합니다.
- GameServer: 실제 게임 플레이를 담당합니다.
- NetworkLibrary: TCP 소켓 통신을 담당합니다.
## 기술 스택
- 서버: .NET Core, ASP .NET Core, Entity Framework Core
- 인프라: AWS EC2, RDS, MemoryDB for Redis, Terraform
- 패킷: Protocol Buffers

## 시스템 구성도
![Cloud_Architecture](https://github.com/aajuy/GomokuOnlineServer/assets/88243441/bbd5f2fc-4244-4a37-b366-32596c7d6499)

## 동작 과정
### TCP 통신
![0_TCP](https://github.com/aajuy/GomokuOnlineServer/assets/88243441/ad5e1581-befc-48d1-9b2a-71a72fd76591)

### 로그인
![1_Login](https://github.com/aajuy/GomokuOnlineServer/assets/88243441/e78ec8da-df19-4ff3-a265-a3c9ed977379)

### 매치 참가
![2_Match](https://github.com/aajuy/GomokuOnlineServer/assets/88243441/a6a72ff4-5b5f-4cea-9806-3a9acb03bb60)

### 게임
![3_Game](https://github.com/aajuy/GomokuOnlineServer/assets/88243441/49da5061-71f3-4b4e-9797-f73cbd59502e)
