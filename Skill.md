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
public class Skill : ScriptableObject
{
    public string skillName;                //스킬 이름
    public SkillType skillType;             //스킬 타입
    public Sprite skillIcon;                //스킬 아이콘
    public AnimationClip skillAnimation;    //스킬 에니메이션
    public readonly float slowMotionDuration = 5.0f; //슬로우모션 지속 시간 
    public GameObject skillEffect;          //스킬 이펙트
    public bool grantsTurnOnSuccess;        //스킬 성공시 턴을 가져오는지 여부

    public int damage;                      //스킬 데미지
    public float santiDamage;               //정신력 데미지
    public float defenseMultiplier;         //데미지 감소율
    public float successRate;               //성공 확률(회피나 패링 등)
    public float counterAttackDamage;       //카운터 데미지

    public bool ExecuteSkill(GameObject player, GameObject target)
    {
        if(skillType == SkillType.Attack)
        {
            ApplyDamage(target);
            return false;
        }
        else if(skillType == SkillType.Defense)
        {
            bool success = Random.value <= successRate;
            if(success)
            {
                Debug.Log($"{skillName}성공, 턴을 가져옵니다");
                return true;
            }
            else
            {
                Debug.Log($"{skillName}실패");
                ApplyDamage(player);
                return false;
            }
        }
        return false;
    }

    private void ApplyDamage(GameObject target)
    {
        Debug.Log($"{skillName} 사용: {target.name}에게 {damage} 데미지, {santiDamage} 정신력 감소");
    }

    private Dictionary<string, Skill> allSkills = new Dictionary<string, Skill>();
}

~~~
