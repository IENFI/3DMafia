# 3DMafia

> 3D 공간에서 미션, 토론, 투표로 마피아를 찾아내는 1인칭 멀티플레이 소셜 디덕션 게임

3DMafia는 시민과 마피아로 역할을 나누어 진행하는 온라인 3D 마피아 게임입니다.  
시민은 맵을 탐색하며 미션을 수행하고, 마피아는 정체를 숨긴 채 시민을 방해합니다. 플레이어들은 채팅과 보이스로 정보를 나누고 투표를 통해 마피아를 추리합니다.

## 주요 기능

- Photon 기반 온라인 로비, 방 생성·참가 및 씬 동기화
- 시민과 마피아 역할 배정, 사망·유령 상태 및 승리 조건 처리
- 낮과 밤의 페이즈 진행 및 플레이어 투표
- 맵 탐색, 미니맵, 미션과 미니게임
- 텍스트 채팅 및 Photon Voice 기반 음성 대화
- 캐릭터 커스터마이징
- 상점과 스피드 업, 투시 등 인게임 아이템

## 게임 흐름

1. 닉네임으로 서버에 접속합니다.
2. 방을 만들거나 기존 방에 참가합니다.
3. 대기실에서 캐릭터를 꾸미고 준비합니다.
4. 게임이 시작되면 시민 또는 마피아 역할을 배정받습니다.
5. 시민은 미션을 수행하고, 마피아는 시민을 제거하며 게임을 방해합니다.
6. 토론과 투표를 거쳐 한 진영의 승리 조건이 충족되면 게임이 종료됩니다.

## 기술 스택

| 구분 | 기술 |
| --- | --- |
| Engine | Unity `2022.3.20f1` |
| Language | C# |
| Rendering | Universal Render Pipeline `14.0.10` |
| Networking | Photon PUN 2, Photon Realtime |
| Voice | Photon Voice |
| UI | uGUI, TextMeshPro |
| Animation | LeanTween |

## 실행 방법

### 사전 준비

- [Unity Hub](https://unity.com/download)
- Unity Editor `2022.3.20f1`
- 유효한 Photon PUN 및 Voice 설정
- 프로젝트에서 사용하는 외부 DB에 접근할 수 있는 개발 환경

### 프로젝트 열기

```bash
git clone https://github.com/IENFI/3DMafia.git
cd 3DMafia
```

1. Unity Hub에서 **Add project from disk**를 선택합니다.
2. 저장소 내부의 `Mafia` 폴더를 프로젝트로 추가합니다.
3. Unity가 패키지와 에셋 임포트를 마칠 때까지 기다립니다.
4. `Mafia/Assets/Scenes/ServerScene.unity`를 엽니다.
5. Photon 및 DB 연결 설정이 현재 개발 환경에 맞는지 확인한 뒤 Play를 실행합니다.

> 온라인 로그인, 로비, 음성 대화에는 Photon 설정이 필요합니다. 현재 로그인과 일부 플레이어 데이터 흐름은 외부 DB에 연결되어 있으므로 DB 접근 권한이 없으면 관련 기능이 정상 동작하지 않을 수 있습니다.

## 프로젝트 구조

```text
3DMafia/
├─ README.md
├─ docs/
│  └─ development-log-2024-11.md
└─ Mafia/                         # Unity 프로젝트 루트
   ├─ Assets/
   │  ├─ Scenes/                  # 서버, 대기실, 게임 씬
   │  ├─ Scripts/                 # 게임·네트워크·UI 로직
   │  │  ├─ Character/
   │  │  ├─ DB/
   │  │  ├─ Map/
   │  │  ├─ Minimap/
   │  │  ├─ Mission/
   │  │  ├─ Setting/
   │  │  └─ Sounds/
   │  ├─ Photon/                  # PUN, Realtime, Voice SDK
   │  ├─ Prefab/
   │  └─ UI/
   ├─ Packages/
   └─ ProjectSettings/
```

## 개발 기록

2024년 11월의 시연 준비, 테스트 결과, 버그 및 아이디어 기록은 [개발 기록](docs/development-log-2024-11.md)에서 확인할 수 있습니다.

## 프로젝트 상태

이 저장소는 2024년 11월 시연 버전을 기준으로 보관되어 있습니다. 실행 환경이나 외부 서비스 설정에 따라 일부 온라인 기능은 별도 구성이 필요할 수 있습니다.
