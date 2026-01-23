using UnityEngine;

public abstract  class CardEffect : ScriptableObject
{
    public abstract void Execute(CardRuntimeContext context);
}
public class CardRuntimeContext
{
    public Player player;
    public Enemy target;
    public DeckSystem deckSystem;
}