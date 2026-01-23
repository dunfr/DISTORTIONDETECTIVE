using UnityEngine;
using System.Collections.Generic;
public class DeckSystem : MonoBehaviour
{
    [Header("초기 덱 구성 (CardData 목록)")]
    public List<CardData> startingDeck;

    [Header("런타임 덱/핸드/디스카드")]
    public List<CardData> drawPile = new List<CardData>();
    public List<CardData> discardPile = new List<CardData>();
    public List<CardData> hand = new List<CardData>();
    
    public GameObject cardPrefab;
    public Transform handParent;
    public Transform playZone;
    public Player player;
    public Enemy enemy;
    public int maxHandSize = 10;

    void Awake()
    {
        SetupDeck();
    }

    public void SetupDeck()
    {
        drawPile.Clear();
        discardPile.Clear();
        hand.Clear();

        // 초기 덱 복사
        drawPile.AddRange(startingDeck);

        Shuffle(drawPile);
    }

    public void Draw(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (drawPile.Count == 0)
            {
                if (discardPile.Count == 0)
                    return;

                drawPile.AddRange(discardPile);
                discardPile.Clear();
                Shuffle(drawPile);
            }
            var data = drawPile[0];
            drawPile.RemoveAt(0);
            hand.Add(data);
            SpawnCardInHand(data);
        }
    }

    public void Discard(CardData card)
    {
        if (hand.Remove(card))
        {
            discardPile.Add(card);
            // 카드 오브젝트 파괴 / 애니메이션 등
        }
    }

    public void DiscardAllHand()
    {
        foreach (var card in hand)
        {
            discardPile.Add(card);
            // 카드 오브젝트 파괴 / 애니메이션 등
        }
        hand.Clear();
    }

    public void Shuffle(List<CardData> list)
    {
        // Fisher-Yates
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }
    void SpawnCardInHand(CardData data)
    {
        var go = Instantiate(cardPrefab, handParent);
        var controller = go.GetComponent<CardController>();
        controller.Init(data, this, player, enemy);
    
        var interaction = go.GetComponent<CardInteraction>();   
        if (interaction != null)
        {
            interaction.handParent = handParent;
            interaction.playZone = playZone;
        }
    }
}
