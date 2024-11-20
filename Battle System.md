Battle Manager
-
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

    [Header("Player Settings")]
    public GameObject playerObject;
    public GameObject enemyObject;
    public List<Button> skillButtons;
    private Inventory inventory;

    private void Start()
    {
        inventory = Inventory.Instance;
        Debug.Log("Inventory 인스턴스 상태: " + (inventory != null ? "정상" : "null"));
        InitialzeSkillButtons();
    }

    private void InitialzeSkillButtons()
    {
        List<Skill> battleSkills = inventory.GetBattleSkills();
        foreach(var skill in inventory.GetBattleSkills())
        {
            Debug.Log(skill != null ? skill.skillName : "null");
        }

        Debug.Log("배틀 스킬 리스트 개수: " + battleSkills.Count);
        for(int i = 0; i < skillButtons.Count; i++)
        {
            Debug.Log($"배틀 스킬 {i}: {battleSkills[i].skillName ?? "null"}");

            if(i < battleSkills.Count && battleSkills[i] != null)
            {
                Image buttonImage = skillButtons[i].transform.Find("Image").GetComponent<Image>();
                buttonImage.sprite = battleSkills[i].skillIcon;
                buttonImage.enabled = true;
                Debug.Log($"스킬 버튼 {i}에 {battleSkills[i].skillName} 아이콘 장착");
            }
            else
            {
                Debug.Log($"스킬 버튼 {i}에 아이콘 장착 실패");
                Image buttonImage = skillButtons[i].transform.Find("Image").GetComponent<Image>();
                buttonImage.sprite = null;
                buttonImage.enabled = false;
            }
        }
    }

    public void UseSkill(int skillIndex)
    {
        Skill skill = inventory.GetBattleSkills()[skillIndex];
        if(skill != null)
        {
            skill.ExecuteSkill(playerObject, enemyObject);
        }
    }

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

        InitialzeSkillButtons();
    }

    public void EndBattle()
    {
        battleWindow.SetActive(false);
    }
}
~~~

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
