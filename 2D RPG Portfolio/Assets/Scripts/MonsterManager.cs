using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterManager : EntityManager
{
    public float followDistance; // 플레이어를 감지하는 거리
    public float attackDistance; // 플레이어를 공격하는 거리

    public float nextThinkTime; // 다음 행동을 결정하기까지 걸리는 시간
    public float thinkTime; // 행동을 결정하고부터 현재까지의 시간
    public int nextMove; // 몬스터의 이동 방향 (-1, 0, 1)
    public bool isAttack; // 공격 중인지 체크
    public bool isDie; // 몬스터가 죽었는지 체크

    // HP 바 관련
    public GameObject hp_Bar; // HP 바 게임오브젝트
    public GameObject hp_fill_GameObject; // HP 바 fill 영역 게임오브젝트
    public GameObject hp_fill_Lerp_GameObject; // HP 바 부드러운 fill 영역 게임오브젝트
    public Image hp_fill; // HP 바 fill
    public Image hp_fill_Lerp; // HP 바 부드러운 fill

    // 드롭 아이템 관련
    public int minItemDrop; // 최소 드롭 아이템 개수
    public int maxItemDrop; // 최대 드롭 아이템 개수

    public override void Die()
    {
        base.Die();

        isDie = true; // 몬스터 사망

        // 게임매니저의 현재 스테이지 몬스터 리스트에서 마지막 값을 삭제
        GameManager.instance.monsterInStageList.RemoveAt(GameManager.instance.monsterInStageList.Count - 1);
    }
    public virtual void MonsterAI() { }
}

