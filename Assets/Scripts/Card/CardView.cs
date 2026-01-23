using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class CardView : MonoBehaviour
{
    [Header("데이터")]
    public CardData cardData;

    [Header("UI 참조")]
    public Image artworkImage;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        if (cardData != null)
        {
            Setup(cardData);
        }
    }
    public void Setup(CardData data)
    {
        cardData = data;

        if (artworkImage != null)
            artworkImage.sprite = data.artwork;

        if (costText != null)
            costText.text = data.cost.ToString();

        if (nameText != null)
            nameText.text = data.cardName;

        if (descriptionText != null)
            descriptionText.text = data.description;
    }

    // 사용할 수 없을 때(에너지 부족 등) 비주얼 처리용
    public void SetInteractable(bool interactable)
    {
        if (canvasGroup == null) return;

        canvasGroup.interactable = interactable;
        canvasGroup.blocksRaycasts = interactable;
        canvasGroup.alpha = interactable ? 1f : 0.5f;
    }
}
