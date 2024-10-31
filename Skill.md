스킬
-
~~~C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public string skillName;
    public int damage;
    public SkillType skillType;
    public Sprite skillIcon;
    public AnimationClip skillAnimation;
    public GameObject skillEffect;

    public enum SkillType
    {
        Attack, Defense, Dodge, Parry 
    }

    public Skill(string name, int dmg, SkillType type)
    {
        skillName = name;
        damage = dmg;
        skillType = type;
    
        skillIcon = Resources.Load<Sprite>($"Skills/Icons/{name}");
        skillAnimation = Resources.Load<AnimationClip>($"Skills/Animations/{name}");
        skillEffect = Resources.Load<GameObject>($"Skills/Effects/{name}");

    }

    public void UseSkill()
    {

    }
}

~~~
