# Project: A Crowed Forest

> 기억하자. **코딩은 AI가 더 잘하지만**, **생각은 내가 더 잘한다**  
> 📌 _내가 옳다_

---

## 📝 Twine (군대에서 스토리 정리 도구)

- Twine을 통해 스토리 분기 구조를 시각화 및 정리
- 작성 내용은 Yarn 코드로 변환하거나 참고용으로 활용 가능
- 설치: [https://twinery.org/](https://twinery.org/)

## 📚 Useful Links

- [ChatGPT](https://chatgpt.com/gpts) — AI 도움 받기
- [Pixel Dot Editor](https://giventofly.github.io/pixelit/#tryit) — 도트 변환기
- [Remove Background](https://www.adobe.com/kr/express/feature/image/remove-background) — 배경 제거
- [Yarn Spinner Try](https://try.yarnspinner.dev/) — 대화 스크립트 작성
- [Markdown Guide](https://inpa.tistory.com/entry/MarkDown-%F0%9F%93%9A-%EB%A7%88%ED%81%AC%EB%8B%A4%EC%9A%B4-%EB%AC%B8%EB%B2%95-%F0%9F%92%AF-%EC%A0%95%EB%A6%AC)

---

## 🧩 현재 작업 중 시스템 및 기능 정리

- https://assetstore.unity.com/top-assets/top-free?srsltid=AfmBOoq6mfkimKmsIj0-Hsce_0O4rmym63Yiwtb9gVdrbqy9IL06QEtO&utm_source=chatgpt.com
- https://itch.io/game-assets/tag-particles/tag-unity?utm_source=chatgpt.com

### ⚔️ 전투 시스템

- 턴제 전투 + 슬로우 반응 시스템
- 플레이어 턴 시 카메라 줌 & 고정 + 스킬/아이템 슬롯 노출
- RadialMenu 기반 스킬/아이템 UI 인터페이스
- 전투 종료 후 `YOU WIN / YOU DIE` 메세지 & 보상창

### 🧬 특성 시스템

- 최대 3개 보유 가능
- 스토리, 장비, 아이템 등으로 획득
- 중복 방지 및 자동 제거 로직 포함

### 🪓 무기 숙련도 시스템

- 무기 종류별 숙련도 상승 → 스킬 해금
- 무기 공격력 증가 보너스 포함
- 해금된 스킬만 인벤토리 UI 및 전투 UI에 표시

### 🧪 아이템 기능

| 이름 | 효과 |
|------|------|
| 발화 부싯가루 | 무기에 **화염 속성** 부여 |
| 번개 부싯가루 | 무기에 **번개 속성** 부여 |
| 썩은 부싯가루 | **독성** 속성 부여 |
| 붉은 부싯가루 | **출혈** 속성 부여 |
| 슬럼 캔디 | HP 회복 + 미세한 **독성** 부작용

### 🎁 보상 시스템

- 전투 종료 후 보상 창
- Yarn 커맨드를 통해 보상 지정 가능
- 무기 숙련도에 따라 추가 스킬 해금 시 자동 보상 표시

### 🗺️ 맵 시스템

- 노드 기반 시각적 맵 UI (탐험형)
- 각 노드마다 전투/대화/스토리 이벤트 연동 가능
- 조건부 이동, 탐색 제한 설정 가능 (예: 열쇠 필요, 시간 경과 등)
- 빛 표현


---

## 🧵 Yarn Spinner 커맨드 확장

- 전투 시작 커맨드에 보상 아이템/스킬 지정 가능
- 숙련도 조건 충족 시 해금된 스킬도 보상 UI에 병합
- `StartBattleCommand(...)`에 다음 인자 포함:
  - enemyDataName
  - backGroundName
  - battleBGM
  - nextYarnNode
  - firstTurn
  - rewardList (아이템/스킬 이름)

## 📜 예정 기능

- 전투 내 아이템 사용 구현
- 무기마다 스킬 자동 정렬 & 해금 기준 표시
- 특성 획득 연출 & 효과 미리보기

---

## 🧠 스토리 설계 & 내러티브

### 🎭 주요 캐릭터 스토리 흐름

- **카인**: 생존과 세계관 중심, 빈민가 → 다양한 루트 (마피아, 신도 등)
- **레이븐**: 복수와 진실 탐색
- **혼**: 잊힌 과거 회복

> 예: 카인의 엔딩 중 하나에서 레이븐에게 죽고 레이븐 시점으로 이어지는 구조

## 🎮 게임 시작 & 세이브 시스템

### 🧱 메인 타이틀 화면
- 게임 실행 시 첫 화면으로 **타이틀 메뉴 UI** 등장
- 구성 요소:
  - **새 게임 시작**
  - **불러오기 (세이브 슬롯 UI)**
  - **설정 (옵션)**
  - **게임 종료**

### 💾 세이브/로드 시스템
- **자동 세이브**: 특정 조건(전투 직전, 스토리 분기점 등)에서 자동 저장
- **수동 세이브**: 메뉴에서 직접 저장 가능 (세이브 슬롯 형식)
- **세이브 구조**
  - Yarn Node 위치
  - 플레이어 상태(HP, 정신력, 스탯, 숙련도, 인벤토리, 특성 등)
  - 장비 및 스토리 진행 정보

---

## ☠️ 게임 오버 시 복귀 선택지

### 🧩 게임 오버 연출
- 플레이어 HP 또는 정신력이 0이 되었을 때 연출
- 다크소울 스타일의 **YOU DIED** 메시지

### 🧭 선택지 제공
- **이전 대화로 되돌아가기**: 마지막 Yarn 대화 위치 복귀
- **이전 분기로 되돌아가기**: 이전 주요 스토리 분기점으로 복귀
- **처음부터 시작하기**: 타이틀 화면으로 돌아가 새 게임 시작

### 🗃 관련 시스템 연계
- 세이브 슬롯에 자동 저장된 분기 지점 정보 필요
- 각 선택지는 저장된 상태 정보를 기반으로 게임 재시작

---

## 🎬 컷신 시스템 구현 예정 기능

### ✅ 컷신 재생 시스템
- Unity `VideoPlayer` 컴포넌트를 이용해 동영상 기반 컷신 재생
- `.mp4` 파일은 `StreamingAssets` 폴더에서 로드

### ✅ Yarn Spinner 연동
- Yarn 커맨드: `<<play_cutscene "파일명.mp4">>`
- 컷신 종료 시 자동으로 다음 노드 실행

### ✅ 컷신 중 제어 차단
- 컷신 재생 중:
  - 플레이어 입력 비활성화
  - HUD 및 상호작용 UI 숨김

### ✅ 컷신 종료 처리
- 영상 종료 후:
  - UI 복원
  - Yarn 스토리 진행 재개
- 필요 시 스킵 기능 구현 가능 (예: 버튼 입력 시 컷신 중단)


## 📁 예상 구현 파일

- `MainTitleUI.cs`
- `SaveSystem.cs`
- `SaveSlotUI.cs`
- `GameOverManager.cs`
- `DialogueCheckpointManager.cs` (Yarn 연동용)



---

## 🧱 구조 및 Hierarchy (Unity)

```Hierarchy

Main Camara
    Dialogue
├── Canvas
│   ├── CustomLineView(Custem Line View 스크립트)
│   │   ├── Scrollbar
│   │   │   ├── Viewport
│   │   │   │   ├── Content
│   │   │   │   │   ├── StoryText(Prefab)
│   │   │   │   │   └── Button(Prefab)
│   │   │   │   │       └── Text
│   │   │   └── SlidingArea
│   │   │       └── Handle
│   │   ├── BattleView(Battle Manager 스크립트)
│   │   │   ├── BattleWindow
│   │   │   │   ├── Background
│   │   │   │   ├── EnemyObject(Enemy Scrpit 스크립트)
│   │   │   │   │   ├── Enemy (Image)
│   │   │   │   │   ├── EnemyHP
│   │   │   │   │   │   ├── EnemyHPBar
│   │   │   │   │   │   └── EnemyHPText
│   │   │   │   │   ├── EnemyName (Image)
│   │   │   │   │   │   └── Text
│   │   │   │   ├── PlayerObject
│   │   │   │   │   ├── Raven (Image)
│   │   │   │   │   ├── PlayerHP
│   │   │   │   │   │   ├── HPBar
│   │   │   │   │   │   └── HPText
│   │   │   │   │   ├── PlayerSanity
│   │   │   │   │   │   ├── SanityBar
│   │   │   │   │   │   └── SanityText
│   │   │   │   │   └── SkillButtons
│   │   │   │   │       ├── SkillButton1Add commentMore actions
│   │   │   │   │       │   └── Image
│   │   │   │   │       ├── SkillButton2
│   │   │   │   │       │   └── Image
│   │   │   │   │       ├── SkillButton3
│   │   │   │   │       │   └── Image
│   │   │   │   │       └── SkillButton4
│   │   │   │   │           └── Image
│   │   ├── UIView
│   │   │   ├── HPBar
│   │   │   ├── HPFrameBar
│   │   │   ├── SanityBar
│   │   │   ├── SanityFrameBar
│   │   │   ├── StoryTopBar
│   │   │   ├── StoryBar
│   │   │   ├── Background
│   │   │   ├── InventoryView(Inventory, Inventory Manager, Player 스크립트)
│   │   │   │   ├── InventoryWindow
│   │   │   │   │   ├── RavenDrake (PlayerCharacter Image)
│   │   │   │   │   ├── XButton
│   │   │   │   │   ├── SkillSlot
│   │   │   │   │   │   ├── SkillButton1
│   │   │   │   │   │   │   └── Image
│   │   │   │   │   │   ├── SkillButton2
│   │   │   │   │   │   │   └── Image
│   │   │   │   │   │   ├── SkillButton3
│   │   │   │   │   │   │   └── Image
│   │   │   │   │   │   └── SkillButton4
│   │   │   │   │   │       └── Image
│   │   │   │   │   ├── WeaponSlot
│   │   │   │   │   │   └── Image
│   │   │   │   │   ├── TopSlot
│   │   │   │   │   │   └── Image
│   │   │   │   │   ├── BottomSlot
│   │   │   │   │   │   └── Image
│   │   │   │   │   ├── WeaponItemWindow
│   │   │   │   │   │   ├── XButton
│   │   │   │   │   │   ├── ScrollView
│   │   │   │   │   │   │   ├── Content
│   │   │   │   │   │   │   │   └── WeaponSlotPrefab
│   │   │   │   │   │   │   └── ScrollbarVertical
│   │   │   │   │   │   └── EquipmentWindowPrefab (GameObject)
│   │   │   │   │   │   │   ├── ScrollView (GameObject)
│   │   │   │   │   │   │   │   ├── Viewport (Mask Component)
│   │   │   │   │   │   │   │   │   └── Content (GameObject) ← 아이템/스킬 슬롯 프리팹이 동적으로 생성됨
│   │   │   │   │   │   │   │   │   │   └── Slot(Prefabs)
│   │   │   │   │   │   │   ├── OptionWindow (GameObject)
│   │   │   │   │   │   │   │   ├── XButton (Button) ← 창 닫기 버튼
│   │   │   │   │   │   │   │   ├── Icon (Image) ← 선택한 아이템/스킬의 아이콘
│   │   │   │   │   │   │   │   ├── Name (TextMeshProUGUI) ← 선택한 아이템/스킬의 이름
│   │   │   │   │   │   │   │   ├── Description (TextMeshProUGUI) ← 선택한 아이템/스킬의 설명
│   │   │   │   │   │   │   │   ├── EquipButton (Button) ← 장착 버튼
│   │   │   │   │   │   │   │   └── UnequipButton (Button) ← 해제 버튼

피드백
-
### 임대웅
읽은 부분은 그리 길지 않고 세계관이나 캐릭터에 대한 깊은 영역을 제시하지는 않기에 캐릭터 시트와 세계관 정리, 스페인이나 가문의 이야기 등을 피드백하지는 않겠다.
다분히 흥미롭고 이걸 어떻게 풀어가는가가 중요하다고 생각하기 때문이다.
황색 성역 파트를 쓸 때 만연체를 의도하고 말꼬리를 늘였는지는 모르겠으나, 만연체를 쓴다고 해서 추상어나 한자어(안도와 희망이 어린, 아비규환), 관용적 어구를 많이 쓸 필요는 없다고 생각한다.
해당 표현들이 많아질수록 글은 이야기라는 느낌보다 비평문이라는 느낌을 받기가 쉽기 때문이다. 
황색 성역 파트가 그렇다. 분명히 게임 속 등장하는 인물의 독백이 위주인 이야기일텐데,
읽다보면 이야기를 보고 있는 독자가 정리한 감상문이라는 생각이 든다.
특히 지하실에서 철창 속에 갇힌 인물들과 대면한 이후에 "더 안쪽으로 들어간다"를 선택했을 때 상황이 그렇다. 
주인공이 냉정하고 고독한 인물이며, 갇혀있는 사람들에게 어떠한 정이나 연민을 느끼지 못하고 관찰자로서 기능하는 것은 이해할 수 있다.
근데, 그렇다면 굳이 구원이니 욕망이니 하는 말들을 내세워 사람들을 판단하고 관찰할 필요가 있을까? 
이들을 무시하고 목적을 향해 나아가는 모습이 자연스럽지 않을까 한다. 아래의 지문을 보자.

( 옆에 있던 남자의 얼굴에서 붉은 피가 솟구쳤다. 그러나 아무도 그의 죽음을 신경 쓰지 않았다. 단검을 쥔 자를 끌어내리고 서로의 목을 할퀴는 광경은 지극히 인간적인 광경이었다
철창 안은 아비규환이었다.
비명과 고통의 울부짖음은 순식간에 침묵으로 바뀌고, 사람들은 다시 붉게 물든 손으로 단검을 쥐기 위해 싸웠다.
그들은 탈출할 수 있었을지도 모른다.
녹슨 철창과 그들의 수는 충분했고, 단검은 예리했으며, 손은 강했다.
나는 그들의 모습을 지켜보며, 어쩌면 그들이 탈출할 수도 있었을지 모른다고 생각했다. 그러나 그들은 협력하지 않았다. 절망 속에서 서로를 믿기보다는, 자신의 손으로 만들어낸 지옥에 갇힌 채 서로를 찢었다.

그러나 그들은 그러지 못했다.
그들이 선택한 것은 협력과 구원이 아닌 서로를 찢는 탐욕이었다.
나는 그들을 외면하며, 깊은 어둠 속으로 발걸음을 옮겼다.

구원을 나눠 가진다는 것은 그들에게 사치였다.
녹슨 철창은 그들에게 막을 수 없는 벽이었다.
협력이라는 도구는 그저 희미한 이상에 불과했고, 그들 손에 닿은 것은 단검이 아니라 서로의 목이었다.

나는 그 소리를 뒤로하고 발걸음을 옮겼다.
비명과 고통이 뒤섞인 울부짖음이 점점 희미해졌다.

나는 잠시 멈춰 뒤를 돌아보았다.
철창 안에서 벌어지는 비극적인 광경은 흔하고도 흔한 경극 같았다.
밑바닥에서 인간성은 사치였고, 비극은 길가의 돌멩이보다 흔했다.
그들은 구원받을 자격을 스스로 버렸다.
구원은 누군가가 주는 것이 아니기에 나는 다시 발걸음을 옮겼다.
이 길의 끝에, 내가 구해야 할 것은 그들 속에는 없었다. ) 

이러한 독백이 길어지기보다 

( 옆에 있던 남자의 얼굴에서 피가 솟구쳤다. 그러나 그에게 향하는 눈길은 없었다.
그보다 주위를 둘러싼 이들은 단검을 쥔 자를 끌어내리고 서로의 목을 움켜쥐는데 온 신경을 쏟고 있었다.
비명소리는 차츰 줄어갔고, 단검은 손과 손을 타며 사람들을 옮겨갔다.

단검에게 녹슨 철창은 분명 살아있는 사람보다 물렀겠지만 
그들은 그리 생각하지 않는 것 같았다.
나는 여러 소리들을 뒤로 하고 발걸음을 옮겼다.
소리가 줄어갈 즈음, 발을 멈추고 뒤를 돌아보았다.
구원이라..

구원은 누군가가 주는 것이 아니다.
나는 다시 발걸음을 옮긴다.
이 길의 끝에, 내가 구해야 할 것이 그들 속에는 없었다. )

정도로 줄여보는 것도 괜찮겠다는 생각이다. 협력이니 구원을 나눠가지지 못했니 하는 평가를 이야기 속 인물에게 맡기기보다 이 상황에서 주인공은 관찰자로만,
평가는 게임을 하는 사람 혹은 독자가 하는 게 맞지 않을까?(물론 위의 수정은 상당히 주관적인 내 취향과 문체를 띄고 있다.)
혹은 주인공이 목적 이외의 남에게 전혀 관심이 없다면 
아예 이 파트("더 안쪽으로 들어간다"를 선택했을 때)는 주인공의 시점이 아닌 철창 속에 갇힌 이들 중 한 명의 시점으로 진행하는 것도 괜찮을 것 같다.
그외에도 황색성역을 살펴보는 부분들의 묘사가 좀 더 간결한 편이 좋지 않을까 생각한다.

물론 내 생각이 무조건 맞다는 것도 아니고 한 명의 독자로서 다분히 나의 문학적 취향에 따라
이러한 생각을 했을 뿐이라는 점을 기억해주길 바란다.

### 송정환


