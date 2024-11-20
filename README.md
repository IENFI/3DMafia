# 24.11.20
## 시연 준비
** 다음 시연은 11.27(수) **
- 최대한 버그를 안 일으키는 선에서 개선하기
### 준오
- ~~region 고치기 (배포판 준비)~~
- ~~배포하도록 하기~~
### 지헌 - UI 디자인 개선
- 미니게임 설명 추가
- 미니게임 탭 기본을 킨 걸로
- 페이즈 UI
### 현석
- 편지쓰기 UI
### 민영
- UI 애니메이션, Level0 채팅창 개선
### 재혁
- 전선 미니게임 고치기

### 보류
- 문을 더 많이 추가하자
- 벤트 추가

# 24.11.19
## 마지막 테스트
1. ~~방장 아닌 사람이 방설정 켜짐~~
2. ~~유령 움직임 이상함~~
	2.1 ~~유령이 두 번 소환되는 버그임~~
3. ~~마피아가 본인이면 다시 시작할 때 닉네임이 빨개보임~~
4. ~~아이템 오류 여전히 존재
	4.1 스피드업이 중첩될때가있음
	4.2 아이템 30초에서 시간이 안 줄어들음 (페이즈 전환 시 멈춤)~~
5. ~~투표로 죽은사람도 사망알림이 적용됨~~
6. ~~북게임 승리 메시지 뜨고 바로 코인이 지급되어야함~~
7. ~~Y랑 P 막기~~
발걸음 추가했으면 좋겠다.
8. 유령이 되면 게임 클리어가 안 뜸?
유령 움직임이 마음에 안 듦 
9. ~~킬 쿨 30초~~
10. ~~서영석 유령 두개보임 (2번과 같은 오류)~~	
11. ~~시민1 마피아1 남았을 때 끝나야 하는지?~~
12. 최대 인원 11명으로 줄이기 (커마가 11개 뿐)
13. ~~시체 존재하는데 페이즈 이동하면 신고버튼이 반짝거림?!~~
14. **페이즈 UI 추가**
15. 편지 쓰기 UI가 이상함
16. ~~4인이 모여야 게임 시작 주석 제거하기~~
17. ~~Sampling Rate 조절~~
18. 소리 UI 누르면 바로 음소거 되는 것 수정
19. 로그 제거
20. ~~모금 글씨 깨짐~~
Index out of range (편지쓰기)

## 안 급함
1. 마스터 클라이언트가 접속이 좀 느린 버그가 있음
2. 문에 테두리 추가하고 싶음
3. 투표할 때 발언권 부여하기
4. 미니게임 전선 버그 등 버그 찾기

# 24.11.18
## 남은 개발
### 아이템
- 스피드업이 중첩될때가있음
- 아이템 30초에서 시간이 안 줄어들음 (페이즈 전환 시 멈춤)

### 보이스
- 투표할 때 발언권 부여하기

### 미니게임은 일단 고정
- 전선 버그 등 버그 찾기
- 디자인 개선

### 아이템 더 필요
- 한 두 개 정도면 좋을 것
- 낮/밤 바꾸기

### 기타
- 낮에 주택이 너무 어두움
- **페이즈 UI 추가**

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

### 빛
- 플레이어 손전등 위치가 애매함

### 아이템
- 스피드업이 중첩될때가있음
- 아이템 30초에서 시간이 안 줄어들음 (페이즈 전환 시 멈춤)

### 기타
- 하늘 보고 걸으면 느려짐
- 달리는 도중 랜덤스폰 안됨
- 스폰 중복 막기
- 승리모금이 1000이상으로 계속 올라감
- 플레이어가 접속하는 중이면 준비 완료를 하지 않아도 게임시작이 됨

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

### 채팅
- 사람이 나가면 채팅에서 삭제하는 기능 추가?
- 유령이 되면 프로필 사진 희미해지기

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
- SeverScene : 새로고침 위치 이상함

### test
- 버튼 광클 오류?
- 미니게임 전선 오류
- 페이즈 UI 추가

### 확인 필요
- 미션 할당이 안된다?
