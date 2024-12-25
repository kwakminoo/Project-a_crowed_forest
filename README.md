# Project-a_crowed_forest

* [Chat GPT](https://chatgpt.com/gpts)

* [Dote Changer](https://giventofly.github.io/pixelit/#tryit)

* [Delete Background](https://www.adobe.com/kr/express/feature/image/remove-background)

* [Yarn Spinner](https://try.yarnspinner.dev/)

* [Mark Down](https://inpa.tistory.com/entry/MarkDown-%F0%9F%93%9A-%EB%A7%88%ED%81%AC%EB%8B%A4%EC%9A%B4-%EB%AC%B8%EB%B2%95-%F0%9F%92%AF-%EC%A0%95%EB%A6%AC)

###### 기억하자 코딩은 AI가 더 잘하지만 생각은 내가 더 잘한다...내가 옳다

예정 사항
-----------
배틀 시스템 완성

배틀창이 떠있을 떄는 다이얼로그 멈추기

스토리 텍스트가 선택지를 누루더라도 그 텍스트가 같은 타이틀 내에 있으면 새로 텍스트를 출력하는것이 아닌 그냥 버튼 밑에 생성하고 버튼이 사라지지 않게 하기

턴제전투 완성

스킬 스크립트 추가해서 스킬초기화 버그 고치기

배틀메니저 역활 분리하기

2.5D게임에서의 게임오브젝트 요소에 대해 알아보기
간단한 턴제전투 완성하기

Hierarchy
-
~~~C#
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
│   │   │   │   │       ├── SkillButton1
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
~~~
