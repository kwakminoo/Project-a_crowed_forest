스킬
-
~~~C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public enum SkillType
{
    Attack, Defense, Dodge, Parry 
}

[CreateAssetMenu(fileName = "New Skill", menuName = "Skill System/Skill")]
public class Skill : ScriptableObject, IItemData
{
    public string skillName;                //스킬 이름
    public string skillOption;              //스킬 설명
    public SkillType skillType;             //스킬 타입
    public Sprite skillIcon;                //스킬 아이콘
    public Sprite skillSprite;              //스킬 에니메이션
    public readonly float slowMotionDuration = 5.0f; //슬로우모션 지속 시간 
    public GameObject skillEffect;          //스킬 이펙트
    public bool grantsTurnOnSuccess;        //스킬 성공시 턴을 가져오는지 여부

    public int damage;                      //스킬 데미지
    public float santiDamage;               //정신력 데미지
    public float defenseMultiplier;         //데미지 감소율
    public float successRate;               //성공 확률(회피나 패링 등)
    public float counterAttackDamage;       //카운터 데미지

    public Sprite GetIcon() => skillIcon;
    public string GetName() => skillName;
    public string GetOption() => skillOption;

    public void ExecuteSkill(GameObject attacker, GameObject target, MonoBehaviour caller)
    {
        Debug.Log($"{skillName}을 사용");

        var enemyScript = target.GetComponent<EnemyScript>();
        if(enemyScript != null)
        {
            enemyScript.TakeDamage(damage);
            Debug.Log($"{skillName}의 데미지: {damage}");
        }

        var player = target.GetComponent<Player>();
        if(player != null)
        {
            player.TakeDamage(damage);
            Debug.Log($"{skillName}의 데미지: {damage}");
        }

        if(skillSprite != null && caller != null)
        {
            SpriteRenderer renderer = attacker.GetComponentInChildren<SpriteRenderer>();
            if(renderer != null)
            {
                caller.StartCoroutine(ShowSpriteEffect(renderer, skillSprite));
            }
            else
            {
                Debug.LogError($"{attacker.name}에 SpriteRenderer가 없습니다");
            }
        }
    }

    public void ShowSkillEffect(GameObject attacker, Sprite skillSprite)
    {
        SpriteRenderer renderer = attacker.GetComponentInChildren<SpriteRenderer>();
    }

    public IEnumerator ShowSpriteEffect(SpriteRenderer renderer, Sprite skillSprite)
    {
        Sprite origianlSprite = renderer.sprite;

        renderer.sprite = skillSprite;
        yield return new WaitForSeconds(0.5f);
        
        renderer.sprite = origianlSprite;
    }

}

~~~
