게임메니저
--------------
1.게임메니저는 게임의 다양한 상태를 종합적으로 관리하는 오브젝트이다<br>

난이도, 게임시작/종료, 

상점페이지
-
* 1단계: 구매 UI 만들기

UI 설정: Unity에서 Canvas구매 메뉴에 대한 UI 요소를 만들고 추가합니다. 여기에는 아이템 구매, 가격 표시, 아이템 설명 버튼이 있는 패널이 포함됩니다. 이 UI를 기본적으로 비활성화하고 플레이어가 대화에서 "구매"를 선택하면 활성화할 수 있습니다.
패널에 다음과 같은 이름을 지정 PurchasePanel하고 게임 시작 시 보이지 않도록 비활성화합니다.

* 2단계: 사용자 정의 원사 명령 생성

구매 UI를 처리하는 스크립트 만들기 : 구매 UI의 가시성을 관리하는 스크립트를 작성합니다. 스크립트에는 플레이어가 대화 옵션에서 "구매"를 선택할 때 UI를 표시하는 메서드가 있습니다.
~~~C#
/Yarn Spinner 커멘드를 설정한 스크립트에 추가로 넣으면 될 것 같음

using UnityEngine;
using Yarn.Unity;

public class PurchaseManager : MonoBehaviour
{
    public GameObject purchasePanel; // Assign the UI panel in the Unity Editor

    // This command will show the purchase panel when called from Yarn Spinner
    [YarnCommand("open_purchase_ui")]
    public void OpenPurchaseUI()
    {
        purchasePanel.SetActive(true);  // Show the purchase panel
    }

    // This command can close the purchase panel
    [YarnCommand("close_purchase_ui")]
    public void ClosePurchaseUI()
    {
        purchasePanel.SetActive(false);  // Hide the purchase panel
    }
}
~~~

* 3단계: 구매 UI를 Yarn Spinner Dialogue에 연결

Yarn 스크립트로 대화 만들기 :이제 대화 스크립트에서 상인 NPC와의 대화를 추가할 수 있습니다. 플레이어가 "구매"를 선택하면 구매 UI를 표시하는 명령이 .yarn트리거됩니다 .
~~~C#
<<open_purchase_ui>>
<<close_purchase_ui>> 
~~~

* 4단계: UI 버튼 연결(선택 사항)<br>
구매 UI에 버튼이 있는 경우 해당 버튼을 스크립트에 연결하여 실제 구매를 관리할 수 있습니다. 이는 Button플레이어가 아이템을 선택할 때 아이템 구매 로직을 트리거하기 위해 Unity의 구성 요소를 사용하여 수행할 수 있습니다.
