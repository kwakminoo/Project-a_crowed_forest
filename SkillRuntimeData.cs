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
        if (baseSkill == null || string.IsNullOrEmpty(baseSkill.skillName) || baseSkill.successRate <= 0 || baseSkill.damage <= 0 || baseSkill.skillSprite == null)
        {
            Debug.LogError($"SkillRuntimeData 생성 중 baseSkill이 유효하지 않습니다. 이름={(baseSkill?.skillName ?? "null")}, 성공률={(baseSkill?.successRate ?? 0)}, 데미지={(baseSkill?.damage ?? 0)}, 스프라이트={(baseSkill?.skillSprite == null ? "null" : "존재")}");
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

