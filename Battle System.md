기본스텟이 있고 레벨없을 통해 스텟을 강화한다. 레벨과 매 회차마다 리셋됀다<br>
무기마다 스킬셋팅이 존재하고 숙련도가 올라가면 새로운 스킬이 개방되거나 기존의 스킬을 강화할 수 있다 플레이어가 선택해서 빌드를 만들어간다<br>
숙련도와 무기는 리셋돼지 않는다<br>
HP는 전투때마다 리셋되지 않고 깍인 채력 그대로 진행된다 아이템이나 스토리진행중 이벤트 등으로 회복이 가능하다

대화중 전투창
-

1. 대화 및 전투 UI 설정: 먼저, 대화 표시와 전투 인터페이스 표시를 전환 할 수 있는 UI를 디자인해야 합니다 .

* 캔버스 설정 
Unity Editor 에서 Canvas(아직 없으면) 생성합니다 .
내부에는 대화 UI 패널 (텍스트와 선택용)과 전투 UI 패널 (체력바, 액션 버튼 등용) Canvas을 둘 수 있습니다 .
대화 UI :

Text내러티브 텍스트를 표시하기 위한 구성요소를 만듭니다 .

Button플레이어 선택이 필요하다면 구성 요소를 추가하세요 .

* 전투 UI
다음과 같은 전투의 UI 요소를 만듭니다.
플레이어와 적의 체력 바 .
공격 , 방어 , 아이템 사용 등 전투 행동을 위한 버튼입니다 .
전투 결과를 표시하는 전투 기록 (피해량과 피해를 설명하는 작은 텍스트 상자와 유사).

* 패널 전환 
장면에서 두 패널을 모두 활성화해 두지 만 한 번에 하나만 표시합니다.
스크립트를 사용해 전투 및 대화 패널의 활성 상태를 전환하여 가시성을 제어할 수 있습니다.

2. 대화와 전투 사이의 전환 처리
플레이어가 옵션(예: '싸움' 또는 '전투')을 선택하면 새로운 장면으로 전환되지 않고 대화 상자 위에 전투 UI가 나타납니다.

* BattleManager 스크립트 생성 
대화 보기와 전투 보기 사이의 전환을 제어하려면 스크립트가 필요합니다.
~~~C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class BattleManager : MonoBehaviour
{
    public GameObject battleWindow;
    public Image enemyImage;
    public TextMeshProUGUI enemyNameText;
    public Image backGroundImage;

    public void StartBattle(string enemyName, string enemySpriteName, string backGroundName)
    {
        battleWindow.SetActive(true);

        enemyNameText.text = enemyName;

        Sprite enemySprite = Resources.Load<Sprite>($"Character/{enemySpriteName}");
        if(enemySprite != null)
        {
            enemyImage.sprite = enemySprite;
        }
        else
        {
            Debug.LogError($"{enemySpriteName}을 찾을 수 없습니다");
        }

        Sprite backGroundSprite = Resources.Load<Sprite>($"Battle Background/{backGroundName}");
        if(backGroundSprite != null)
        {
            backGroundImage.sprite = backGroundSprite;
        }
        else
        {
            Debug.LogError($"{backGroundName}을 찾을 수 없습니다");
        }
    }

    public void EndBattle()
    {
        battleWindow.SetActive(false);
    }
}

~~~

* 링크 버튼 및 UI
전투를 시작하기 위한 버튼을 설정합니다. 예를 들어, 플레이어가 대화에서 "싸움"을 선택하면 StartBattle()메서드가 호출됩니다.
전투가 끝나면 해당 EndBattle()방법을 사용해 대화로 다시 전환하세요.

3. 전투 로직 통합
게임에 전투 시스템 로직을 통합해야 합니다. 작동 방식은 다음과 같습니다.

* 간단한 턴 기반 전투 시스템 
전투 패널 안에서 플레이어가 공격 등의 동작을 선택하면 적이 반응하는 간단한 턴제 시스템을 구현합니다 .

* 대화로 돌아가기 
전투가 끝나면 대화 UI로 돌아가서 스토리를 계속 진행하세요.

4. Yarn Spinner 통합
Yarn Spinner 대화 시스템 내에서 전투 전환을 처리하려면 사용자 지정 명령을 사용하여 전투 UI를 트리거할 수 있습니다.

* Yarn Spinner에서 전투를 시작하기 위한 사용자 지정 명령을 정의하세요
~~~C#
[YarnCommand("start_battle")]
public void StartBattleFromDialogue()
{
    StartBattle();
}
~~~

기본적인 턴제전투
----------------


QTR 턴제 전투
-------------
1. 기본 개념<br>
준비모션 -> 슬로우 -> 공격(결과)

* 준비모션에서 넘어갈 때 중간 과정을 자세히 넣지 않고 준비모션에서 몇초간 기다렸다가 공격이 적중하는 모션으로 넘어간다
* 플레이어나 적은 그 기다리는 몇초동안 행동을 선택하고 그 선택에 따라 결과모션이 달라진다
~~~
1. 일반공격은 프레임orSpeed 변수 값으로 결과를 적용
2. 패링: 공격 모션에서 패링 모션 출력
3. 카운터: 공격모션에서 카운터 모션 출력
4. 회피: 공격모션에서 회피 모션 출력
5. 슈퍼아머: 공격을 맞는 모션 출력 후 공격 모션을 출력
~~~

* BattleScene에서 Player와 Enemy는 기본적으로 위치를 고정해놓고 UI를 턴제처럼 만들어서 전투를 진행

ex) 적이 공격을 하면 공격모션이 오는 n초의 시간동안 어떤 행동을 할 수 있고 기본적을 턴이 순차적으로 돌아가되 카운터, 스턴 등의 상황에선 일방적인 턴이 가능
     '강인도'에 따라 슈퍼아머가 적용, 회피를 사용하면 공격을 n%의 확률로 피하고 턴 종료
