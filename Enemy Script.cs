using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
    public EnemyData enemyData;
    public Image enemyImage;
    public int currentHP; //현재 체력
    public int maxHP;
    public Image enemyHPBar;
    private Animator animator;
    
    private void Awake()
    {
        if(enemyData != null)
        {
            InitializeEnemy(enemyData);
        }
    }

    //적 데이터 초기화
    public void InitializeEnemy(EnemyData data)
    {
        enemyData = data;
        if (enemyData.skills != null)
        {
            // 방어적 복사
            enemyData.skills = new List<Skill>(enemyData.skills);

            // 유효하지 않은 스킬 제거
            enemyData.skills.RemoveAll(skill =>
            {
                if (skill == null || string.IsNullOrEmpty(skill.skillName) || skill.successRate <= 0 || skill.damage <= 0 || skill.skillSprite == null)
                {
                    Debug.LogError($"유효하지 않은 스킬 제거: 이름={(skill?.skillName ?? "null")}, 성공률={(skill?.successRate ?? 0)}, 데미지={(skill?.damage ?? 0)}, 스프라이트={(skill?.skillSprite == null ? "null" : "존재")}");
                    return true; // 제거
                }
                return false; // 유지
            });
        }
        else
        {
            Debug.LogError("EnemyData.skills가 null입니다.");
        }

        enemyData = data;
        maxHP = enemyData.maxHP;
        currentHP = maxHP;
        Debug.Log($"{enemyData.enemyName} 초기화 완료: 체력 {currentHP}/{enemyData.maxHP}");
    }


    public void UseSkill(Player target)
    {
        if(enemyData == null || enemyData.skills == null || enemyData.skills.Count == 0)
        {
            Debug.LogError("적 스킬이 설정 되지 않았습니다");
            return;
        }

        SkillRuntimeData selectedSkill = ChooseSkill();

        if(selectedSkill == null)
        {
            Debug.LogError($"{enemyData.enemyName}이(가) 사용할 수 있는 스킬이 없습니다");
            return;
        }

        BattleManager battleManager = FindFirstObjectByType<BattleManager>();
        if (battleManager != null)
        {
            battleManager.HandleSkillCameraTransition(target, selectedSkill);
        }

        if (enemyImage != null)
        {
            StartCoroutine(ChangeEnemyImage(selectedSkill.skillSprite, 1.0f));
        }
        ExecuteSkillRuntime(selectedSkill, target); // 런타임 데이터 기반 실행
    }

    private void ExecuteSkillRuntime(SkillRuntimeData skill, Player target)
    {
        if (string.IsNullOrEmpty(skill.skillName) || skill.successRate <= 0 || skill.damage <= 0 || skill.skillSprite == null)
        {
            Debug.LogError($"스킬 데이터가 유효하지 않습니다. 이름: {skill.skillName}, 성공률: {skill.successRate}, 데미지: {skill.damage}, 스프라이트: {skill.skillSprite}");
            return;
        }

        Debug.Log($"{enemyData.enemyName}이(가) {skill.skillName} 실행 - 설정된 데미지: {skill.damage}");

        float roll = UnityEngine.Random.Range(0f, 1f);
        if (roll > skill.successRate)
        {
            Debug.Log($"{skill.skillName}이(가) 실패했습니다. (Roll: {roll}, Success Rate: {skill.successRate})");
            return;
        }

        target.TakeDamage(skill.damage);
    }

    public IEnumerator ChangeEnemyImage(Sprite newSprite, float duration)
    {
        if (enemyImage != null)
        {
            // 기존 이미지 저장
            Sprite originalSprite = enemyImage.sprite;

            // 스킬 이미지로 변경
            enemyImage.sprite = newSprite;

            yield return new WaitForSeconds(duration);

            // 기존 이미지로 복원
            if (originalSprite != null)
            {
                enemyImage.sprite = originalSprite;
            }
            else
            {
                Debug.LogWarning("originalSprite가 null입니다. 기본 이미지를 설정할 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("enemyImage가 null 상태입니다. 이미지 변경이 불가능합니다.");
        }
    }

    public SkillRuntimeData ChooseSkill()
    {
        if (enemyData.skills == null || enemyData.skills.Count == 0)
        {
            Debug.LogError($"{enemyData.enemyName}에게 할당된 스킬이 없습니다");
            return null;
        }

        List<Skill> validSkills = enemyData.skills.FindAll(skill =>
        {
            if (skill == null || string.IsNullOrEmpty(skill.skillName) || skill.successRate <= 0 || skill.damage <= 0 || skill.skillSprite == null)
            {
                Debug.LogWarning($"유효하지 않은 스킬이 필터링됨: 이름={(skill?.skillName ?? "null")}, 성공률={(skill?.successRate ?? 0)}, 데미지={(skill?.damage ?? 0)}, 스프라이트={(skill?.skillSprite == null ? "null" : "존재")}");
                return false;
            }
            return true;
        });

        if (validSkills.Count == 0)
        {
            Debug.LogError($"{enemyData.enemyName}에게 유효한 스킬이 없습니다");
            return null;
        }

        int randomIndex = UnityEngine.Random.Range(0, validSkills.Count);
        Skill selectedSkill = validSkills[randomIndex];

        return new SkillRuntimeData(selectedSkill.Clone());
    }

    //체력 변경 메소드
    public void TakeDamage(int damage)
    {
        currentHP = Mathf.Max(currentHP - damage, 0); // HP 감소, 최소값은 0
        Debug.Log($"{enemyData.enemyName}이(가) {damage}의 데미지를 받았습니다");
        if (enemyHPBar == null)
        {
            Debug.LogError("Enemy HPBar가 null입니다. 연결 상태를 확인하세요.");
        }

        UpdateHPBar();

        if(currentHP <= 0)
        {
            Die();
        }
    }

    private void UpdateHPBar()
    {
        if (enemyHPBar == null)
        {
            Debug.LogError("Enemy HPBar가 연결되지 않았습니다.");
            return;
        }

        float hpRatio = Mathf.Clamp01((float)currentHP / maxHP); // HP 비율 계산
        enemyHPBar.fillAmount = hpRatio; // HPBar 길이 설정
    }

    public void Die()
    {
        if(enemyData.deathAnimation != null)
        {
            Debug.Log($"{enemyData.enemyName}이(가) 쓰러졌습니다");
            Animator animator = GetComponent<Animator>();
            if(animator != null)
            {
                animator.Play(enemyData.deathAnimation.name);
            }
        }

    }

    //스킬 가져오기
    public List<Skill> GetSkills()
    {
        return enemyData.skills;
    }
}