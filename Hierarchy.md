- GameManager
  - InventoryManager (Component)
  - YarnCommands (Component)
  - YarnManager (Component)
  - DialogueRunner (Component)
  - DialogueText (Text Component)
  - ChoiceButton1 (Button Component)
  - ChoiceButton2 (Button Component)  

- UI
  - DialogueUI
      - ...

- Player (선택 사항)
  - PlayerController (Component)
  - PlayerData (Component)

- Other GameObjects (필요에 따라 추가)
  - Environment
  - NPC
  - ...
 --------------------------------------------

- YarnManager(YarnManager.cs)
  - (YarnSceneManager 컴포넌트 YarnSceneManager.cs)
  - (DialogueRunner 컴포넌트 DialogueRunner.cs(Yarn Spinner 플러그인에서 제공))
  - (YarnManager 컴포넌트 YarnManager.cs)
    - DialogueText (Text 컴포넌트가 포함된 UI Text 객체)
    - ChoiceButton1 (Button 컴포넌트가 포함된 UI Button 객체)
    - ChoiceButton2 (Button 컴포넌트가 포함된 UI Button 객체)
    - DialogueRunner (Yarn Spinner 플러그인에서 제공)





































