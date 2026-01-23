using UnityEngine;
[CreateAssetMenu(fileName = "DamageEffect", menuName = "CardGame/Effects/Damage")]
public class DamageEffect:CardEffect
{
    public int amount;

    public override void Execute(CardRuntimeContext context)
    {
        if (context.target == null) return;
        context.target.TakeDamage(amount);
    }
}
[CreateAssetMenu(fileName = "DrawEffect", menuName = "CardGame/Effects/Draw")]
public class DrawEffect : CardEffect
{
    public int drawAmount = 1;

    public override void Execute(CardRuntimeContext context)
    {
        if (context.deckSystem == null) return;
        context.deckSystem.Draw(drawAmount);
    }
}