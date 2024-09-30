기본적인 턴제전투
----------------

1. 기본 설계<br>
턴제 전투 시스템은 플레이어와 적이 번갈아 가면서 행동을 수행하는 시스템입니다. 각각의 턴에 어떤 행동(공격, 방어, 스킬 사용 등)을 할지 선택한 후, 그 행동을 처리하고 다음 턴으로 넘어갑니다.

* 기본 시스템 구조<br>
플레이어 턴: 플레이어가 행동을 선택합니다.<br>
적 턴: AI가 적의 행동을 결정하고 실행합니다.<br>
상태 관리: 각 캐릭터의 HP, MP, 스킬 상태 등을 추적합니다.<br>
턴 교체: 플레이어의 턴이 끝나면 적의 턴으로, 적의 턴이 끝나면 다시 플레이어의 턴으로 전환합니다.

2. 상태 관리 클래스 만들기
 ~~~ C#
   public class Character
   {
     public string characrterName;
     public int health;
     public int maxHealth;
     public attackPower;
     public defensePower;

     pulbic Character(staring name, int health, int attack, int defense)
     {
       characterName = name;
       this.health = haelth;
       maxHealth = health;
       attackPower = attack;
       defensePower = defense
     }

     public void Attack(Character target)
     {
       int damage = attackPower - target.defensePower;
       //단순 뺄셈에서 퍼센트 감소로 변경 예정
       damage = Mathf.Max(damage, 0); //최소 0의 데미지를 보장
       target.TakeDamage(damage);
     }

     public void TakeDamage(int damage)
     {
       health -= damage;
       if(health <= 0)
       {
          health = 0;
           //죽었을 때 애니메이션 추가 예정
          Debug.Log("You Die");
       }
       else
       {
          Debug.Log(characterName + "has" + health + "HP left.");
       }
     }    
   
   }
   ~~~

3. 턴 관리 시스템 만들기
~~~ C#
using UnityEngine;

public enum BattleSate {START, PLAYER_TRUN, ENEMY_TURN, WON, LOST}

public calss TrunBasedSystem : MonoBehaviour
{
  public Character player;
  public CHaracter enemy;
  public BattleState state;
  public Animator animator;
  public GameObject deathMessagePanel;  // 죽음 메시지 UI 패널
  public Text deathMessageText;  // 죽음 메시지 텍스트
  public float delayBeforeMainMenu = 3f;  // 메인 화면으로 이동하기 전 대기 시간
  private bool isDead = false; //사망여부
  void Start()
  {
    state = BattleState.START;
    StartBaltte();
  }

  void StartBattle()
  {  
    player = new Character("Player", 100, 20, 5); //health, attackPower, defensePower
    enemy = new Character("Enemy", 50, 15, 3); //health, attackPower, defensePower

    state = BattleState.PLAYER_TURN; //'속도'에 따라 턴을 결정할 예정
    PlayerTurn;
  }

  void PlayerTurn()
  {
    Debug.Log("Player's Turn"); //플레이어가 선택을 할수 있도록 UI를 통해 행동 선택 후 PlayerAction() 호출
  }

  public void PlayerAction()
  {
    player.Attack(enemy);
    if(enemy.health <= 0)
    {
      Die();//적 사망 
      state = BattleState.WON;
      EndBattle();
    }
    else
    {
      state = BattleState.ENEMY_TURN;
      EnemyTurn();
    }
  }

  void EnemyTurn()
  {
    DebugLog("Enemy's Turn");
    enemy.Attack(player); //변경 예정 플레이어의 선택에 따라 n%로 행동을 선택

    if(player.health <= 0)
    {
      Die();//플레이어 사망
      state = BattleState.LOST;
      EndBattle();
    }
    else
    {
      state = BattleState.PLAYER_TURN;
      PlayerTURN();
    }
  }

void Die()
    {
        isDead = true;

        // 죽음 애니메이션 실행
        animator.SetBool("isDead", true);
    }

  void EndBattle()
  {
    if(state == BattleState.WON)
    {
      Debug.Log("WON");
      StartCoroutine(HandleDeath());
    }
    else if(state == BattleState.LOST)
    {
      Debug.Log("You DIE");
      StartCoroutine(HandleDeath());
    }
  }

 IEnumerator HandleDeath()
    {
        // 애니메이션이 끝날 때까지 대기
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // 죽음 문구 패널 활성화
        deathMessagePanel.SetActive(true);
        deathMessageText.text = "You have died! Returning to the main menu...";

        // 몇 초 동안 대기 후 메인 메뉴로 이동
        yield return new WaitForSeconds(delayBeforeMainMenu);
        SceneManager.LoadScene("MainMenu");  // MainMenu라는 씬으로 이동
    }
}
~~~

4. UI 및 행동 선택

버튼에 기능 연결 //초안
~~~ C#
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
  public TurnBasedSystem turnSystem;
  public Button attackButton;

  void start()
  {
    attackButton.onClick.AddListener(OnAttackButtonClick);
  }

  void OnAttackButtonClick()
  {
    if(turnSystem.state == BattleState.PLAYER_TURN)
    {
      turnSystem.PlayerAction();
    }
  }
}
~~~

5. 적 AI 구현
~~~ C#
void EnemyTurn()
{
  Debug.Log("Enemy's Turn);
  enemy.Attack(player);

  if(player <= 0)
  {
    state = BattleState.LOST;
    EndBattle();
  }
  else
  {
    state = BattleState.PLAYER_TURN;
    PlayerTurn();
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
