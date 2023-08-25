# GomokuOnlineServer
온라인 오목 게임 [GomokuOnline](https://github.com/aajuy/GomokuOnlineClient)의 서버입니다.
- LoginServer: 회원가입과 로그인을 담당합니다.
- MatchServer: 매치 참가 신청/취소와 결과 기록을 담당합니다.
- GameServer: 게임 진행을 담당합니다.
- NetworkLibrary: TCP 소켓 통신을 담당합니다.

## 기술 스택
- 서버: .NET Core, ASP .NET Core, Entity Framework Core
- 인프라: AWS EC2, RDS, MemoryDB for Redis
- 패킷: Protocol Buffers

## 시스템 구성도
![Cloud_Architecture](https://github.com/aajuy/GomokuOnlineServer/assets/88243441/bbd5f2fc-4244-4a37-b366-32596c7d6499)

## 동작 과정
1. 로그인
![1_Login](https://github.com/aajuy/GomokuOnlineServer/assets/88243441/9a9fd2f3-2c10-4670-8773-fe5376c384a3)

<br/>

2. 매치 참여
![2_Match](https://github.com/aajuy/GomokuOnlineServer/assets/88243441/c5415b9c-2481-41fe-833f-c52ba6c5daa8)

<br/>

3. 게임
![3_Game](https://github.com/aajuy/GomokuOnlineServer/assets/88243441/84e69f90-234c-4da7-916a-6c9650ee8797)
