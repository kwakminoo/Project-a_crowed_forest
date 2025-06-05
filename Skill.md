Skill
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

    public Sprite motionSprite;            //스킬 공격 스프라이트
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
        if (string.IsNullOrEmpty(skillName) || successRate <= 0 || damage <= 0 || skillSprite == null)
        {
            Debug.LogError($"스킬 데이터가 유효하지 않습니다. 이름: {skillName}, 성공률: {successRate}, 데미지: {damage}, 스프라이트: {skillSprite}");
            return;
        }
        
        Debug.Log($"{skillName} 실행 - 설정된 데미지: {damage}");

        float roll = UnityEngine.Random.Range(0f, 1f);
        if (roll > successRate)
        {
            Debug.Log($"{skillName}이(가) 실패했습니다. (Roll: {roll}, Success Rate: {successRate})");
            return;
        }


        var enemyScript = target.GetComponent<EnemyScript>();
        if (enemyScript != null)
        {
            Debug.Log($"{target.name}에 EnemyScript를 찾아 데미지 적용: {damage}");
            enemyScript.TakeDamage(damage);
        }

        var player = target.GetComponent<Player>();
        if (player != null)
        {
            Debug.Log($"{target.name}에 Player 컴포넌트를 찾아 데미지 적용: {damage}");
            player.TakeDamage(damage);
        }
    }

    public Skill Clone()
    {
        Skill clonedSkill = ScriptableObject.CreateInstance<Skill>();
        clonedSkill.skillName = skillName;
        clonedSkill.skillOption = skillOption;
        clonedSkill.skillType = skillType;
        clonedSkill.skillIcon = skillIcon;
        clonedSkill.skillSprite = skillSprite;
        clonedSkill.skillEffect = skillEffect;
        clonedSkill.grantsTurnOnSuccess = grantsTurnOnSuccess;
        clonedSkill.damage = damage;
        clonedSkill.santiDamage = santiDamage;
        clonedSkill.defenseMultiplier = defenseMultiplier;
        clonedSkill.successRate = successRate;
        clonedSkill.counterAttackDamage = counterAttackDamage;
        return clonedSkill;
    }
}

~~~

SkillRunTimeData
---
~~~C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillRuntimeData
{
    public string skillName;
    public string skillOption;
    public SkillType skillType;
    public Sprite skillIcon;
    public Sprite skillSprite;
    public int damage;
    public float santiDamage;
    public float defenseMultiplier;
    public float successRate;
    public float counterAttackDamage;

    public SkillRuntimeData(Skill baseSkill)
    {
        if (baseSkill == null)
        {
            Debug.LogError("SkillRuntimeData 생성 중 baseSkill이 null입니다.");
            return;
        }

        skillName = baseSkill.skillName;
        skillOption = baseSkill.skillOption;
        skillType = baseSkill.skillType;
        skillIcon = baseSkill.skillIcon;
        skillSprite = baseSkill.skillSprite;
        damage = baseSkill.damage;
        santiDamage = baseSkill.santiDamage;
        defenseMultiplier = baseSkill.defenseMultiplier;
        successRate = baseSkill.successRate;
        counterAttackDamage = baseSkill.counterAttackDamage;
    }
}
~~~
