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
    public Sprite skillAnimation;    //스킬 에니메이션
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

    /*public Skill(string name, int dmg, SkillType type, float defMult = 1.0f, float , int counterDmg = 0)
    {
        skillName = name;
        damage = dmg;
        skillType = type;
        defenseMultiplier = defMult;
        counterAttackDamage = counterDmg;
    
        skillIcon = Resources.Load<Sprite>($"Skills/Icons/{name}");
        skillAnimation = Resources.Load<AnimationClip>($"Skills/Animations/{name}");
        skillEffect = Resources.Load<GameObject>($"Skills/Effects/{name}");

    }*/

    public void ExecuteSkill(GameObject attacker, GameObject target)
    {
        Debug.Log($"{skillName}을 사용");

        var enemyScript = target.GetComponent<EnemyScript>();
        if(enemyScript != null)
        {
            enemyScript.TakeDamage(damage);
        }

        if(attacker.TryGetComponent<Animator>(out Animator animator))
        {
            animator.Play(skillAnimation.name);
        }
    }

}
~~~
