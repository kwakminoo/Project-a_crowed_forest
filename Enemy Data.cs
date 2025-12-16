using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy System/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName; //적 이름
    public Sprite enemySprite; //적 이미지
    public int maxHP; //적 최대 체력
    public AnimationClip deathAnimation; //죽음 애니메이션
    public List<Skill> skills; //적이 사용할 스킬
}
