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

단계별 예:

* BattleManager 스크립트 생성 
대화 보기와 전투 보기 사이의 전환을 제어하려면 스크립트가 필요합니다.
~~~C#
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public GameObject dialoguePanel;  // The panel showing the dialogue
    public GameObject combatPanel;    // The panel showing the combat UI
    public Text dialogueText;         // The text component for dialogue
    public Text combatLog;            // The text component for combat log

    // Method to start the battle
    public void StartBattle()
    {
        dialoguePanel.SetActive(false);  // Hide the dialogue panel
        combatPanel.SetActive(true);     // Show the combat panel
        combatLog.text = "You have encountered an enemy!";  // Example combat log
    }

    // Method to end the battle and return to dialogue
    public void EndBattle()
    {
        combatPanel.SetActive(false);  // Hide the combat panel
        dialoguePanel.SetActive(true); // Show the dialogue panel
        dialogueText.text = "You won the battle and continue your journey.";  // Update the dialogue text
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

* 전투 UI 업데이트 
플레이어가 취하는 행동에 따라 전투 UI(체력 막대와 전투 로그 등)를 업데이트할 수 있습니다. 전투 행동에 버튼을 사용하고, 그에 따라 전투 로그를 업데이트합니다.
~~~C#
public void Attack()
{
    // Example attack logic
    int damage = Random.Range(5, 10);
    combatLog.text += "\nYou attacked the enemy for " + damage + " damage.";
    
    // Update enemy health (this can be another method call)
    // Once enemy health is 0, end the battle
    if (enemyHealth <= 0)
    {
        EndBattle();
    }
}
~~~

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
