using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("HP")]
    public int maxHp = 80;
    public int currentHp;

    [Header("방어도(Block)")]
    public int block;

    [Header("에너지(Energy)")]
    public int maxEnergy = 3;
    public int currentEnergy;

    [Header("연결 객체")]
    public DeckSystem deckSystem;

    void Start()
    {
        currentHp = maxHp;
        block = 0;
        StartTurn();   // 전투 시작 시 첫 턴 세팅
    }
    #region HP / Block
    public void TakeDamage(int amount)
    {
        // 슬더스처럼 먼저 Block으로 막고 남는 만큼 HP 감소.[web:17]
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
        // UI 갱신 등
        // UpdateHpUI();
        // UpdateBlockUI();
    }
    public void GainBlock(int amount)
    {
        block += amount;
        // UpdateBlockUI();
    }
    public void Heal(int amount)
    {
        currentHp = Mathf.Min(maxHp, currentHp + amount);
        // UpdateHpUI();
    }
    private void Die()
    {
        Debug.Log("Player Dead");
        // 게임오버 처리, 씬 전환, UI 표시 등
    }
    #endregion

    #region Energy

    public bool CanPayCost(int cost)
    {
        return currentEnergy >= cost;
    }

    public void PayCost(int cost)
    {
        currentEnergy -= cost;
        if (currentEnergy < 0) currentEnergy = 0;
        // UpdateEnergyUI();
    }

    #endregion

    #region 

    // 턴 시작 시 호출 (슬더스 기준: 에너지 리필, 일정 수 카드 드로우).[web:15][web:18]
    public int cardsToDrawPerTurn = 5;

    public void StartTurn()
    {
        currentEnergy = maxEnergy;
        block = 0;  // 슬더스는 플레이어 턴 시작 시 Block 초기화.[web:17]
        // UpdateEnergyUI();
        // UpdateBlockUI();

        if (deckSystem != null)
        {
            deckSystem.Draw(cardsToDrawPerTurn);
        }
    }

    // 턴 종료 버튼에서 호출
    public void EndTurn()
    {
        // 핸드 전체 디스카드 후 다음 턴 준비.[web:2][web:18]
        if (deckSystem != null)
        {
            deckSystem.DiscardAllHand();
        }

        // 여기서 적 AI 턴을 호출하거나 TurnManager에 턴을 넘김
        // TurnManager.Instance.EndPlayerTurn();
    }

    #endregion
}
