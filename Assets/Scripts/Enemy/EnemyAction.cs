using UnityEngine;

public enum EnemyActionType
{
    Attack,
    Defend,
    AttackDefend
}
[CreateAssetMenu(fileName = "EnemyAction", menuName = "CardGame/Enemy/Action")]
public class EnemyAction : ScriptableObject
{
    public string actionName;
    public EnemyActionType type;

    [Header("수치")]
    public int attackDamage;
    public int blockAmount;

    [Header("의도 UI용")]
    public Sprite intentIcon;
    public Color intentColor = Color.white;

    public void Execute(Enemy self, Player player)
    {
        switch (type)
        {
            case EnemyActionType.Attack:
                DoAttack(self, player);
                break;
            case EnemyActionType.Defend:
                DoDefend(self);
                break;
            case EnemyActionType.AttackDefend:
                DoAttack(self, player);
                DoDefend(self);
                break;
        }
    }
    private void DoAttack(Enemy self, Player player)
    {
        if (player == null) return;
        player.TakeDamage(attackDamage);
    }

    private void DoDefend(Enemy self)
    {
        self.GainBlock(blockAmount);
    }
}

