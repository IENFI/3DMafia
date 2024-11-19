# 24.11.19
## 마지막 테스트
1. ~~방장 아닌 사람이 방설정 켜짐~~
2. ~~유령 움직임 이상함~~
	2.1 ~~유령이 두 번 소환되는 버그임~~
3. ~~마피아가 본인이면 다시 시작할 때 닉네임이 빨개보임~~
4. 아이템 오류 여전히 존재
5. 투표로 죽은사람도 사망알림이 적용됨
6. 북게임 승리 메시지 뜨고 바로 코인이 지급되어야함
7. Y랑 P 막기
발걸음 추가했으면 좋겠다.
8. 유령이 되면 게임 클리어가 안 뜸
유령 움직임이 마음에 안 듦 
9. ~~킬 쿨 30초~~
10. ~~서영석 유령 두개보임 (2번과 같은 오류)~~	
11. ~~시민1 마피아1 남았을 때 끝나야 하는지?~~
12. 최대 인원 11명으로 줄이기 (커마가 11개 뿐)
13. ~~시체 존재하는데 페이즈 이동하면 신고버튼이 반짝거림?!~~

## 안 급함
1. 마스터 클라이언트가 접속이 좀 느린 버그가 있음
2. 문에 테두리 추가하고 싶음

# 24.11.18
## 남은 개발
### 아이템
- 스피드업이 중첩될때가있음
- 아이템 30초에서 시간이 안 줄어들음 (페이즈 전환 시 멈춤)

### 보이스
- ~~거리 조절이 안 됨 (일반 소리도 포함)~~
- ~~소리가 너무 작음~~
- 투표할 때 발언권 부여하기

### 미니게임은 일단 고정
- 전선 버그 등 버그 찾기
- 디자인 개선

### 아이템 더 필요
- 한 두 개 정도면 좋을 것
- 낮/밤 바꾸기

### 기타
- 낮에 주택이 너무 어두움
- ~~사운드 설정창 UI가 좀 이상함 (+ ServerScene에서 esc로 꺼졌으면 좋겠음 )~~
- **페이즈 UI 추가**

## 버그
~~- 문 두 개가 문제가 있음 -> 못 고치면 그냥 열어두기~~
~~- Level_0 방 설정 확인 시에 뜨는 UI가 동작이 어색함~~

## 밸런스 조절
~~- 마피아 킬 쿨타임 늘이기~~

## 안 급함
- 플레이어 손전등 위치가 애매함
- 사운드 설정에 보이스도 들어갔으면 좋갰음
- ~~닉네임이 중복돼서 다시 입력했을 때 중복이 아니면 중복됐다는 UI가 삭제되어야 함~~
- 하늘 보고 걸으면 느려짐
- 승리모금이 1000이상으로 계속 올라감
- SeverScene : 새로고침 위치 이상함
- 버튼 광클 오류?

### 방 설정
- 킬 쿨타임
- 페이즈 시간 설정
- 속도..?

## 아이디어
- 사람 들어오면 누가 들어왔는지 알려주는 UI 어떤가!
- 나중에 키 설정들은 한 번에 모아서 같이 관리하도록 하면 어떨까?
- 미션은 3개로 줄이는 게 나아보임 -> 미션을 더 만들어볼까? or 미션 복제 (미니맵도 동기화되도록 해야함)
- 밤에 숨기 기능 있으면 좋을 것 같다. (옷장 숨기?, 웅크리기?)
- 벽 램프 상호작용하면 불 끄기
- 커스마이징UI 하단 페이지바를 조금더 직관적으로
- 유령이 되면 프로필 사진 희미해지기

**(마지막 테스트는 11.19 + 발표 준비, 영상 촬영까지)**

# 24.11.15
## 테스트
	<버그>
### 미니게임
- ~~미니게임 하다 죽으니까 미니게임 창이 그대로 유지됨~~
- ~~편지쓰기 미니게임 배경이 사라짐 (아닌듯)~~
- ~~스도쿠 미니게임 중에 1을 누르면 손전등이 켜짐~~
- ~~미니게임 중에 esc 하면 설정창이 켜짐~~

### UI
- ~~마스터 클라이언트만 방 설정이 켜지는 오류 (Level_0.scene)~~
- ~~로딩창 꺼놓음~~
- ~~채팅창 위치가 이상함, 입력창이 범위를 벗어남~~
- ~~누구 죽이고 나면 마피아 닉네임 색깔이 드러남 (Level_0을 가도 유지됨)~~
- ~~유령 상태에 미니맵 변경 안 됨 (1층-2층)~~
- ~~닉네임 사용중이라 뜨고 접속이 됨~~
- ~~준비완료를 클릭한 상태로 w + space하면 나가지는 문제~~
- ~~유령이 미니맵에 보임~~
- ~~방 설정에서 배경음악 음소거가 안 됨~~
- ~~방 설정이 ServerScene에서부터 Level_1까지 유지되도록 하기~~

### 빛
- 플레이어 손전등 위치가 애매함
- ~~유령 조명이 꺼짐 (Timer.cs에 ToggleAllLights() 함수만 고치면 됨)~~
- ~~아침에 손전등 켜짐~~

### 아이템
- 스피드업이 중첩될때가있음
- 아이템 30초에서 시간이 안 줄어들음 (페이즈 전환 시 멈춤)

### 기타
- 소리가 거리 조절이 안 됨 (문, 보이스 등)
- ~~투표 시간 없음 -> 시간이 끝나면 자동 스킵하도록 하기~~
- ~~문 통과하는 버그 (코루틴 진행되는 동안만 투명해지면 되는데) + 문에 밀리는 오류 있음 ~~
- ~~쓰러질 때 애니메이션에 빌더 아바타가 겹쳐보임~~
- 하늘 보고 걸으면 느려짐
- ~~방을 나가면 게임 매니저가 두 개 됨
-> 나갈 때 playerController.cs 208번 오류~~
- 달리는 도중 랜덤스폰 안됨
- 스폰 중복 막기
- 마피아 킬 쿨타임 늘이기
- 승리모금이 1000이상으로 계속 올라감
- ~~잘못된 문에 Door 태그가 달려있음~~
- 플레이어가 접속하는 중이면 준비 완료를 하지 않아도 게임시작이 됨
- ~~커스터마이징 방이 만들어지면 디비에서 삭제하는 로직이 문제가 있음 -> 일단 삭제하기~~

- 가끔 뜨는 버그
Assertion failed on expression: '!CompareApproximately(aScalar, 0.0F)'
UnityEngine.Quaternion:Lerp (UnityEngine.Quaternion,UnityEngine.Quaternion,single)
PlayerController:Update () (at Assets/Scripts/Character/PlayerController.cs:195)

## 아이디어
- 사람 들어오면 누가 들어왔는지 알려주는 UI 어떤가!
- 나중에 키 설정들은 한 번에 모아서 같이 관리하도록 하면 어떨까?
- 미션은 3개로 줄이는 게 나아보임 -> 미션을 더 만들어볼까? or 미션 복제 (미니맵도 동기화되도록 해야함)

# 24.11.14

	<앞으로 해야할 일>
### 미니게임은 일단 고정
- 전선 버그 등 버그 찾기
- 디자인 개선

### 아이템 더 필요
- 한 두 개 정도면 좋을 것
- 낮/밤 바꾸기

### 보이스 개발 끝까지
- ~~설정에 마이크 온 오프 넣기~~
- ~~마이크 인식 UI 넣기~
	-> 페이즈 UI에 가려져야 하는데 안 가려지는 문제!~~


### 채팅
- ~~프로필 사진~~
- 사람이 나가면 채팅에서 삭제하는 기능 추가?
- ~~투표할 때 플레이어들끼리만 보이고, 유령은 다 보이고 (유령 채팅 보임)~~
- 유령이 되면 프로필 사진 희미해지기
- ~~채팅에 입력창보다 길게 많은 문자를 입력하면 입력창 범위가 이상해짐 (스크롤이 필요할듯)~~

### 방 설정
- 킬 쿨타임
- 페이즈 시간 설정
- 속도..?

임시 테스트 (11.11)

베타 테스트는 다음주 금요일(11.15) - 밸런스 조절


	<보류>
- 맵 증축 보류


	<의견>
- 밤에 숨기 기능 있으면 좋을 것 같다. (옷장 숨기?, 웅크리기?)
- 벽 램프 상호작용하면 불 끄기
- 커스마이징UI 하단 페이지바를 조금더 직관적으로

인터넷이 느린 환경에서 고려해야할 점 생각해볼 필요가 있음...

	<버그>
### dev 기준
- ~~아침에 손전등, 신고버튼 빛이 켜짐~~
- SeverScene : 새로고침 위치 이상함
- ~~신고 버튼 동작 수정 (타이머 코드 수정) -> 페이즈가 바뀜에 따라 변수를 0으로 초기화 하면 됨~~
- 이미 사용중인 닉네임인데 들어가지는 건 뭐지?
- ~~유령 문 툴팁 오류~~

### test
- 버튼 광클 오류?
- 미니게임 전선 오류
- 페이즈 UI 추가
- ~~시야 제한인데 닉네임이 너무 잘보임~~
- ~~보트매니저 12명 수정..~~
- ~~커스터마이징 창을 키고 esc를 누르면 설정 창이 켜짐?~~

### 버그 해결함
- ~~방 설정-Level0 확인 버튼~~
- ~~아바타 오류 (start?) - 시체 추가함~~
- ~~UI 오류(dev)o, 마피아 닉네임o~~
- ~~스도쿠 버튼 안 눌림 -> 죽음 알림창 로직 약간 바꿔야됨~~
- ~~죽음 알림 아이템 갑자기 적용 안 됨~~
- ~~책 정리 완료 UI 깨짐~~
- ~~스도쿠 아웃라인 오류 (키보드로 입력 가능하게 하기)~~
- ~~미니게임 완료하면 메테리얼 뜨게 하기~~
- ~~책 정리 오류 (모든 책을 눌러야 완료됨, 성공 문구 안 지워짐-같은 페이즈 한정)~~
- ~~승리금액 모금하니까 엔터됨(가로 넓이 넓히기)~~
- ~~엑스레이 아이템 오류 (손전등 처럼 범위제한이 있는듯)~~
- ~~승리금액이 0이하로 떨어지도록 방해하기 하면 '코인이 부족합니다'가 뜸 (Coroutine couldn't be started because the the game object 'merchent (9)'is inactive!~~
- ~~playerReportRadius 업데이트 디버그 삭제~~
- ~~방 설정-SeverScene 이전, 다음 버튼 오류~~
- ~~커마 동시에 바꾸면 딜레이 문제로 오류 생김~~

### 애매한 해결
- ~~신고버튼o, 문 열기 (세모)~~
- ~~킬아이콘할당(호스트가 입장이느린경우 할당이 제대로 안되는듯) (세모)~~

### 확인 필요
- 미션 할당이 안된다?
