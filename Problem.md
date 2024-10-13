Yarn Spinner
----------
Name : OpenUPM<br>
URL : https://package.openupm.com<br>
Scope(s) : dev.yarnspinner

VScode 한국어 패치
----------
1. 한국어 언어 팩 설치
VS Code 실행:

VS Code를 실행합니다.
확장 (Extensions) 메뉴 열기:

왼쪽 사이드바에서 확장 (Extensions) 아이콘을 클릭합니다.
또는 Ctrl+Shift+X 단축키를 사용하여 확장 메뉴를 엽니다.
한국어 언어 팩 검색:

검색 창에 Korean Language Pack을 입력합니다.
Korean Language Pack for Visual Studio Code를 찾습니다. 이는 Microsoft에서 제공하는 공식 언어 팩입니다.
설치 (Install):

Korean Language Pack for Visual Studio Code 항목 옆의 Install 버튼을 클릭하여 설치합니다.
2. 언어 변경
명령 팔레트 열기:

F1 키 또는 Ctrl+Shift+P 단축키를 사용하여 명령 팔레트를 엽니다.
언어 변경 명령 실행:

명령 팔레트에 Configure Display Language를 입력하고 선택합니다.
또는 설정 > 언어 설정 (Preferences: Configure Language)을 선택할 수도 있습니다.
언어 선택:

목록에서 ko (Korean)를 선택합니다.
VS Code 재시작:

VS Code를 다시 시작하여 변경 사항을 적용합니다.
3. 설정 파일을 통해 언어 변경 (옵션)
설정 파일 열기:

명령 팔레트를 열고 Preferences: Open Settings (JSON)을 입력하고 선택합니다.
설정 파일이 열립니다.
언어 설정 추가:

아래의 코드를 설정 파일에 추가합니다:
json
코드 복사
{
  "locale": "ko"
}
설정 저장 및 VS Code 재시작:

설정 파일을 저장하고 VS Code를 다시 시작합니다.
완료
위의 단계를 따르면 VS Code의 기본 언어가 한국어로 변경됩니다. 모든 메뉴와 메시지가 한국어로 표시될 것입니다.

VS Code의 한국어 언어 팩을 사용하면 더 친숙한 환경에서 코딩을 할 수 있게 됩니다. 필요한 경우 다시 영어로 변경하려면 동일한 단계를 수행하고 en을 선택하면 됩니다.

VS Code 업데이트가 안끝날 때
-
1. 네트워크 연결 문제<br>
* 원인 : VS Code가 업데이트나 확장 프로그램을 다운로드하고 설치하려고 하지만 서버에 제대로 연결할 수 없습니다.<br>
* 해결책 : 인터넷 연결을 확인하고 업데이트 프로세스를 다시 시작해 보세요.

2. 확장 업데이트가 중단됨
* 원인 : 때때로 VS Code의 확장 프로그램이 업데이트 중에 멈춥니다. 이로 인해 VS Code가 업데이트를 완료하지 못할 수 있습니다.<br>
* 해결책 : 확장 프로그램을 비활성화하거나 제거해 보세요.

VS 코드를 엽니다.<br>
확장 프로그램 탭( ) 을 클릭합니다 Ctrl+Shift+X.
문제를 일으킬 수 있는 확장 프로그램을 비활성화하거나 제거하세요.
VS Code를 다시 시작합니다.

3. 손상된 설치 파일<br>
* 원인 : 설치 파일이 손상되어 업데이트 프로세스가 중단되었을 수 있습니다.
* 해결책 : VS Code를 다시 설치하세요.


시스템에서 VS Code를 제거합니다 .<br>
에서 VS Code 최신 버전을 다운로드하세요 .공식 홈페이지
다시 설치한 후 문제가 지속되는지 확인하세요.

4. 백그라운드 프로세스가 멈춤<br>
* 원인 : 때로는 VS Code(또는 확장 프로그램)와 관련된 백그라운드 프로세스가 중단되어 업데이트가 완료되지 않을 수 있습니다.<br>
* 해결책 : 실행 중인 모든 VS Code 프로세스를 종료합니다.

Windows 의 경우<br>
작업 관리자 ( )를 엽니다 Ctrl+Shift+Esc.<br>
Visual Studio Code 와 관련된 모든 프로세스를 찾아보세요 .<br>
해당 프로세스를 종료하고 VS Code를 다시 시작하세요.

5. 수동 확장 업데이트<br>
* 원인 : 확장 프로그램이 자동 업데이트를 시도하는 데 문제가 있는 경우, 수동으로 업데이트를 시도할 수 있습니다.<br>
* 해결책 <br>
자동 업데이트 비활성화: 비활성화하려면를 File -> Preferences -> Settings검색하세요 .Extensions: Auto Update<br>
확장 프로그램을 수동으로 업데이트하세요.

6. VS코드 재시작<br>
* 해결책 : VS Code를 닫았다가 다시 열면 업데이트 프로세스가 재설정되어 계속 진행될 수 있습니다. 그래도 안 되면 다시 시작해 보세요.

7. 오류에 대한 로그 확인<br>
* 솔루션 : VS Code m<br>
명령 팔레트 ( )를 엽니다 Ctrl+Shift+P.<br>
검색 Developer: Show Logs하고 확인하세요<br>
다음 단계를 따르면 문제를 해결하고 Visual Studio Code를 다시 실행할 수 있습니다. 확장 프로그램을 다시 설치하거나 확인한 후에도 문제가 지속되면 자유롭게

유니티 한글 폰트 깨질 때 설정
----------------
![image](https://github.com/user-attachments/assets/7b8de12c-18a7-4a16-aebd-f9184af89e8f)<br>
* Source Font File: 변환할 폰트 파일<br>
* Sampling Point Size: 폰트의 기본 크기 설정. 이 크기는 Atlas에 렌더링 되는 글자의 실제 픽셀 크기에 영향을 미치므로, 적절한 스케일링을 통해 메모리 사용량을 보존<br>
* Padding: 각 글자 주변에 추가되는 여백을 결정. 이 여백은 서로 다른 글자들 사이의 공간을 둘 때 유용하며, 텍스트의 가독성을 향상함<br>
* Atlas Resolution: 생성되는 폰트 아틀라스의 해상도를 결정. 해상도가 높을수록 텍스트의 품질이 높아지지만, 메모리 사용량이 증가함<br>
* Character Set: ASCII, Unicode 등 생성할 글자 집합을 결정. 참고로 ASCII 코드는 기본적으로 한글을 지원하지 않지만, 해당 창에서 ASCII를 선택하더라도 실제로 유니코드 문자를 지원하기 때문에 한글을 사용할 수 있음<br>
* Render Mode: 텍스트가 SDF, Raster 등 렌더링 되는 방식을 결정<br>
* Get Kerning Pairs: 폰트의 글자 쌍 사이의 간격을 조정시키는 정보를 가져올 것인지 결정


![image](https://github.com/user-attachments/assets/cff62371-3886-4cf8-bcc5-a0842afd53ac)


Visual Studio나 IDE에서 클래스 정의로 이동하기
클래스 정의 이동
----------
DialogueRunner 위에 커서를 놓고 F12 키를 누릅니다.
또는 마우스 오른쪽 버튼을 클릭하고 'Go to Definition'을 선택합니다.
메서드 확인:

DialogueRunner 클래스 파일이 열리면 클래스 내부에 정의된 메서드를 찾습니다.
SelectOption 또는 유사한 이름의 메서드를 검색합니다.

라인 뷰의 텍스트를 아래로 출력 
---------------------------------------------------------------------------------------------
1. ScrollRect: 텍스트를 스크롤하는 데 사용할 수 있는 스크롤기능들을 제공


라인뷰의 텍스트 창 크기 조절이 안될 때
---------------------------------------------------------------------------------------------
1. RectTransform 조작 문제
LineView가 UI의 일부인 경우, RectTransform을 사용하여 크기를 조정해야 합니다. 텍스트 출력 창의 부모 객체나 RectTransform 설정에 문제가 있을 수 있습니다.

해결 방법:

LineView 객체를 선택하고 RectTransform 컴포넌트를 확인합니다.
RectTransform 컴포넌트의 크기 조정 핸들을 사용하여 창의 크기를 조정합니다.
부모 객체의 RectTransform 설정이 자식 객체의 크기 조정에 영향을 미칠 수 있으므로 부모 객체의 설정도 확인합니다.
2. Canvas 설정 문제
UI 요소는 Canvas 객체의 설정에 따라 다르게 동작할 수 있습니다. Canvas의 Render Mode 설정에 따라 UI 요소의 크기 조정이 제한될 수 있습니다.

해결 방법:

Canvas 객체의 Render Mode가 Screen Space - Overlay, Screen Space - Camera, 또는 World Space 중 어떤 것으로 설정되어 있는지 확인합니다.
Render Mode에 따라 UI 요소의 크기 조정 방법이 다를 수 있으므로 적절한 모드를 선택합니다.
3. Layout Component 충돌
LineView 객체나 부모 객체에 Layout 관련 컴포넌트(HorizontalLayoutGroup, VerticalLayoutGroup, ContentSizeFitter 등)가 추가되어 있는 경우, 크기 조정이 제한될 수 있습니다.

해결 방법:

LineView 객체와 부모 객체에 추가된 Layout 관련 컴포넌트를 확인합니다.
필요에 따라 Layout 관련 컴포넌트를 일시적으로 비활성화하거나 제거하고 크기를 조정한 후 다시 활성화합니다.
4. Anchors 설정 문제
RectTransform의 Anchor 설정이 잘못되어 있는 경우, 크기 조정이 의도한 대로 작동하지 않을 수 있습니다.

해결 방법:

RectTransform의 Anchor 설정을 확인합니다. 일반적으로 Anchor는 부모 객체 내에서 UI 요소의 위치와 크기를 제어합니다.
Anchor 설정을 적절히 조정하여 크기 조정이 가능하도록 합니다.
5. Yarn Spinner 스크립트 제한
Yarn Spinner 스크립트나 설정에서 LineView의 크기 조정을 제한하는 부분이 있을 수 있습니다.

해결 방법:

Yarn Spinner 설정 파일이나 스크립트를 확인하여 크기 조정과 관련된 제한이 있는지 확인합니다.
필요에 따라 Yarn Spinner 스크립트를 수정하거나 설정을 조정합니다.

게임화면에서 해상도가 깨질 때
----------
1. Canvas Scaler 설정 확인
Canvas Scaler는 UI 요소가 화면 크기와 해상도에 맞게 스케일링되도록 조정합니다. 잘못된 Canvas Scaler 설정은 텍스트의 해상도 문제를 일으킬 수 있습니다.

Canvas Scaler 확인:

Hierarchy 창에서 텍스트가 포함된 Canvas 오브젝트를 선택합니다.
Inspector 창에서 Canvas Scaler 컴포넌트를 확인합니다.
UI Scale Mode 설정:

UI Scale Mode를 Scale With Screen Size로 설정합니다.
Reference Resolution 필드에 기준 해상도를 입력합니다. 일반적으로 1920x1080 또는 게임의 목표 해상도를 설정합니다.
Match 속성을 조정하여 Width와 Height의 비율을 적절히 설정합니다. 일반적으로 0.5로 설정하면 균형 잡힌 스케일링을 제공합니다.
Dynamic Pixels Per Unit:

Dynamic Pixels Per Unit 값을 높이면 텍스트의 해상도가 개선될 수 있습니다. 기본값(100)을 유지하거나 상황에 따라 높여 보세요.
2. Text 컴포넌트 설정 조정
텍스트 컴포넌트의 설정이 잘못되면 해상도가 낮아 보일 수 있습니다. 아래 설정을 확인하세요.

Font Size 조정:

텍스트 컴포넌트의 Font Size를 충분히 크게 설정하여 텍스트가 더 선명하게 표시되도록 합니다.
Font Rendering Mode:

Font 컴포넌트를 사용하는 경우, Rendering Mode를 Smooth 또는 Hinted Smooth로 설정합니다. 이는 텍스트 렌더링 품질을 향상시킬 수 있습니다.
TextMeshPro 사용 (권장):

TextMeshPro는 Unity의 기본 텍스트 컴포넌트보다 더 높은 품질의 텍스트 렌더링을 제공합니다.
TextMeshPro를 사용하여 텍스트를 표시하면 해상도 문제가 크게 개선될 수 있습니다.
3. TextMeshPro 설정 확인 (TextMeshPro 사용 시)
TextMeshPro를 사용하는 경우, 다음 설정을 확인하여 텍스트 해상도를 개선할 수 있습니다.

TextMeshPro - Text 설정:

Font Size를 적절히 설정합니다.
Auto Size 옵션을 비활성화하고 Font Size를 수동으로 설정해 보세요.
Extra Padding 옵션을 활성화하여 텍스트 렌더링 시 계단 현상을 줄일 수 있습니다.
TextMeshPro Font Asset 설정:

사용 중인 TextMeshPro 폰트 에셋의 Rendering Mode를 Distance Field 16 또는 Distance Field 32로 설정합니다.
이 설정은 고해상도 텍스트를 렌더링하는 데 유리합니다.
4. 게임 뷰 해상도 확인
Unity의 Game 뷰에서 해상도를 확인하여 문제가 발생하지 않도록 합니다.

Game 뷰 해상도 설정:

Game 뷰 상단의 해상도 드롭다운 메뉴를 클릭하여 현재 해상도를 확인합니다.
게임의 목표 해상도를 선택하여 정확한 해상도로 게임을 테스트합니다.
Maximize on Play 설정:

Game 뷰의 Maximize on Play 옵션을 활성화하여 전체 화면으로 게임을 테스트할 때 발생할 수 있는 문제를 방지합니다

YarnCommand를 사용한 명령어 정의
------------
~~~C#
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class YarnCommandHandler : MonoBehaviour
{
    // "jumpToScene"라는 이름의 Yarn 명령어를 정의합니다.
    [YarnCommand("jumpToScene")]
    public void JumpToScene(string sceneName)
    {
        // Unity의 SceneManager를 사용해 씬을 전환합니다.
        SceneManager.LoadScene(sceneName);
    }

    //AddCommandHandler를 사용한 명령어 정의:
    public DialogueRunner dialogueRunner;

    void Start()
    {
        // "jumpToScene" 명령어를 등록하고, 이를 호출할 때 JumpToScene 메서드가 실행되도록 합니다.
        dialogueRunner.AddCommandHandler<string>("jumpToScene", JumpToScene);
    }

    void JumpToScene(string sceneName)
    {
        // Unity의 SceneManager를 사용해 씬을 전환합니다.
        SceneManager.LoadScene(sceneName);
    }
}
~~~

2D 배경 레이어 순서 변경
---------
* Z-축 위치 조정: 오브젝트의 Z-축 위치를 조정하여 화면에서의 깊이(앞/뒤)를 설정합니다.<br>
* Sorting Layer 및 Order in Layer: 2D 게임에서 Sorting Layer와 Order in Layer를 사용하여 오브젝트가 화면에서 그려지는 순서를 결정합니다.<br>
* Canvas의 Sorting Order: UI 캔버스와 관련된 배경은 Canvas 컴포넌트의 Sorting Layer와 Order in Layer를 사용하여 순서를 조정합니다.

완성한 프로젝트를 빌드
------------
1. build setting에 들어간다
2. 씬을 모두 추가하고 플로폼을 설정한다
3. 하단에 build를 클립하면 끝

얀 스피너에서 이미지 출력하기
-
1. 사용자 지정 명령을 사용하여 이미지 표시
대화 중에 이미지를 보여주는 사용자 지정 Yarn Spinner 명령을 만들 수 있습니다. 이는 시스템 Image의 Unity 구성 요소를 사용하여 달성할 수 있습니다 UI.

* 단계별 구현
1. 이미지 UI 요소 만들기 : Unity 편집기에서 새 편집기를 만들고 Canvas(아직 없는 경우) Image여기에 구성 요소를 추가합니다. 이는 이미지의 표시 영역 역할을 합니다.
다음과 같은 이름을 지정할 수 있습니다 DialogueImage.

2. 이미지를 표시하기 위한 사용자 정의 원사 명령 생성 : Yarn 명령 처리기 스크립트에서 Yarn 스크립트의 명령에 따라 이미지를 표시하거나 숨기는 메서드를 만듭니다.
~~~C#
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class DialogueImageHandler : MonoBehaviour
{
    // Canvas 안에 있는 Image UI 객체에 대한 참조
    public Image imageDisplayObject;

    // Start 함수에서 명령어 처리기를 DialogueRunner에 등록
    void Start()
    {
        // DialogueRunner를 찾고 명령어 처리기를 등록
        DialogueRunner dialogueRunner = FindObjectOfType<DialogueRunner>();
        if (dialogueRunner != null)
        {
            dialogueRunner.AddCommandHandler<string>("show_image", ShowImage);
            dialogueRunner.AddCommandHandler("hide_image", HideImage);
        }
        else
        {
            Debug.LogError("DialogueRunner를 찾을 수 없습니다.");
        }
    }

    // show_image 명령어를 처리하는 함수
    public void ShowImage(string imageName)
    {
        // Resources/Images/ 경로에서 이미지 로드
        Sprite imageSprite = Resources.Load<Sprite>("Images/" + imageName);

        if (imageSprite != null)
        {
            // 로드한 이미지를 Image 컴포넌트에 설정
            imageDisplayObject.sprite = imageSprite;
            // 이미지가 보이도록 투명도 조정
            imageDisplayObject.color = new Color(1, 1, 1, 1);
        }
        else
        {
            Debug.LogError("이미지를 찾을 수 없습니다: " + imageName);
        }
    }

    // hide_image 명령어를 처리하는 함수
    public void HideImage()
    {
        // 이미지 투명도를 0으로 설정하여 숨김
        imageDisplayObject.color = new Color(1, 1, 1, 0);
    }
}


Yanr Spinner
<<show_image "imageName">> // 이미지 출력
<<hide_image>> // 이미지 숨기기
~~~

2. Dialogue Views 사용(대안)
텍스트와 이미지를 보다 통합된 방식으로 동시에 표시하려면 Yarn Spinner LineView나 다른 Dialogue View구성 요소를 수정하거나 확장할 수 있습니다.

* LineView 수정 : LineView텍스트와 이미지가 함께 표시되도록 이미지 필드를 포함하도록 확장합니다 .

* 대화에 대한 사용자 정의 UI를 만드세요 : Text같은 패널에 구성 요소와 구성 요소를 추가한 Image다음, 새 대화 텍스트가 표시될 때 두 구성 요소를 동시에 업데이트하도록 스크립트를 수정합니다.


SaveMode
-
유니티 스크립트 내에 문제가 생겼을 때 최소한의 필요한 것들만 불러온 다음 스크립트를 수정하면 SaveMode에서 벗어난다
