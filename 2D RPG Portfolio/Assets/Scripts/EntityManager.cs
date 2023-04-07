using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    [SerializeField]
    public float health { get; private set; } // 현재 체력
    public float maxHealth; // 최대 체력

    public int attackDamage; // 공격력
    public float moveSpeed; // 이동 속도

    public Transform[] attackTransform; // 공격 위치

    public float[] attackRadius; // 공격 범위
    public float attackCoolDown; // 공격 후 지난 시간
    public float nextAttackTime; // 다시 공격이 가능한 시간

    void Awake()
    {
        health = maxHealth; // 체력 초기화
    }

    // 대미지를 받는 함수
    public virtual void TakeDamage(int damage, Transform Pos)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    // 죽는 함수
    public virtual void Die() { }
}
