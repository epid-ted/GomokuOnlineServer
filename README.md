# GomokuOnlineServer
GomokuOnline은 다른 플레이어들과 실시간으로 오목을 즐길 수 있는 온라인 게임입니다.
- 클라이언트: https://github.com/aajuy/GomokuOnlineClient
- 더미 클라이언트: https://github.com/aajuy/GomokuOnlineDummyClient

GomokuOnline의 서버는 LoginServer, MatchServer, GameServer, NetworkLibrary로 구성됩니다. 매치 참가 및 실제 게임 플레이는 TCP 소켓 통신을 통해 이루어지며, 다른 기능들은 HTTP 웹 요청을 통해 처리됩니다.
- LoginServer: 회원가입, 로그인을 담당합니다.
- MatchServer: 매치 참가, 결과 기록, 랭킹, 스태미나를 담당합니다.
- GameServer: 실제 게임 플레이를 담당합니다.
- NetworkLibrary: TCP 소켓 통신을 담당합니다.
## 기술 스택
- 서버: C#, .NET, ASP .NET Core, Entity Framework Core
- 인프라: AWS EC2, RDS, MemoryDB for Redis
- 패킷: Protocol Buffers

## 시스템 구성도
![Cloud_Architecture](https://github.com/aajuy/GomokuOnlineServer/assets/88243441/4a6111ec-d6e3-45ac-ac15-7803e57cf518)

## 동작 과정
### TCP 통신
![0_TCP](https://github.com/aajuy/GomokuOnlineServer/assets/88243441/5dad24c4-39da-49ed-85c5-5bdad26f2ffa)

### 로그인
![1_Login](https://github.com/aajuy/GomokuOnlineServer/assets/88243441/68fad282-eb18-481c-bc73-a6d2c201fcec)

### 매치 참가
![2_Match](https://github.com/aajuy/GomokuOnlineServer/assets/88243441/9da3b362-f9bd-4d1c-8a6d-6a4d8e593439)

### 게임
![3_Game](https://github.com/aajuy/GomokuOnlineServer/assets/88243441/e42d4172-2646-460f-aab0-195155796b4c)
