using UnityEngine;

public class CardController : MonoBehaviour
{
    public CardData data;
    public DeckSystem deckSystem;
    public Player player;
    public Enemy target;

    private CardView view;

    void Awake()
    {
        view = GetComponent<CardView>();
    }
    void Start()
    {
        if (view != null && data != null)
        {
            view.Setup(data);
        }
    }
    public void Init(CardData cardData, DeckSystem deck, Player p, Enemy defaultTarget)
    {
        data = cardData;
        deckSystem = deck;
        player = p;
        target = defaultTarget;

        if (view != null)
            view.Setup(data);
    }
    public void Play()
    {
        // 코스트 체크, 에너지 차감 등
        if (!player.CanPayCost(data.cost))
            return;

        player.PayCost(data.cost);

        // 효과 실행
        var context = new CardRuntimeContext
        {
            player = player,
            target = target,
            deckSystem = deckSystem
        };
        
        foreach (var effect in data.effects)
        {
            if (effect != null)
                effect.Execute(context);
        }

        // 사용 후 디스카드
        deckSystem.Discard(data);

        // 핸드/보드에서 카드 오브젝트 제거
        Destroy(gameObject);
    }
}
