using UnityEngine;

public enum CardType
{
    Attack,
    Skill,
    Power
}
[CreateAssetMenu(fileName = "CardData", menuName = "CardGame/Card")]
public class CardData : ScriptableObject
{
    public string cardId;       
    public string cardName;
    
    [TextArea]
    public string description;
    public Sprite artwork;

    public CardType cardType;
    public int cost;

    public int damage;
    public int block;

    public CardEffect[] effects;
}
