using UnityEngine;

public class Enemy : MonoBehaviour
{
     [Header("HP")]
    public int maxHp = 50;
    public int currentHp;

    [Header("방어도(Block)")]
    public int block;

    [Header("행동 패턴 (Intent)")]
    public EnemyAction[] actions;   
    private int currentActionIndex = 0;

    [Header("참조")]
    public Player player;
    public EnemyIntentUI intentUI;
    void Start()
    {
        currentHp = maxHp;
        block = 0;
        ChooseNextAction();
    }

    #region HP / Block

    public void TakeDamage(int amount)
    {
        int remaining = amount;

        if (block > 0)
        {
            int blockUsed = Mathf.Min(block, remaining);
            block -= blockUsed;
            remaining -= blockUsed;
        }

        if (remaining > 0)
        {
            currentHp -= remaining;
            if (currentHp <= 0)
            {
                currentHp = 0;
                Die();
            }
        }

        // TODO: HP/Block UI 갱신
    }

    public void GainBlock(int amount)
    {
        block += amount;
        // TODO: Block UI 갱신
    }

    private void Die()
    {
        Debug.Log("Enemy Dead");
        // 전투 종료 처리, 리워드 지급, 애니메이션 등.[web:25]
        Destroy(gameObject);
    }

    #endregion

    #region 턴 흐름

    // TurnManager가 적 턴 시작 시 호출
    public void StartEnemyTurn()
    {
        // 슬더스 기준: 적은 턴 넘어가도 Block이 남아있지만, 원하면 여기서 block = 0 해도 됨.[web:17]

        ExecuteCurrentAction();
        ChooseNextAction();

        // 적 턴 끝 -> TurnManager가 다시 플레이어 턴 시작
        TurnManager.Instance.StartPlayerTurn();
    }

    private void ChooseNextAction()
    {
        if (actions == null || actions.Length == 0)
        {
            currentActionIndex = -1;
            return;
        }

        // 단순 순환형 패턴 (A-B-C-A-B-C...).[web:23]
        currentActionIndex = (currentActionIndex + 1) % actions.Length;

        // TODO: Intent UI 업데이트 (현재 액션을 표시)
        if (intentUI != null)
        {
            intentUI.SetIntent(actions[currentActionIndex]);
        }
    }

    private void ExecuteCurrentAction()
    {
        if (currentActionIndex < 0 || currentActionIndex >= actions.Length)
            return;

        var action = actions[currentActionIndex];
        if (action == null) return;

        action.Execute(this, player);
    }

    #endregion
}
