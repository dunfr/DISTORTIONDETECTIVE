using System;
using UnityEngine;
using UnityEngine.EventSystems;
[RequireComponent(typeof(RectTransform))]
public class CardInteraction : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerEnterHandler, IPointerExitHandler,IPointerDownHandler
{
    [Header("연결")]
    public CardController cardController;   // 앞에서 만든 CardController
    public Transform handParent;           // 원래 손 위치 부모
    public Transform playZone;             // 카드 사용할 Drop 영역
    [Header("Hover 설정")]
    public float hoverScale = 1.1f;
    public float hoverMoveUp = 30f;

    private RectTransform rectTransform;
    private Canvas canvas;
    private Transform originalParent;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private bool isDragging;
    private bool canAffordCost = true;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalScale = rectTransform.localScale;
    }

    private void Update()
    {
        UpdateCostAffordability();
    }

    void Start()
    {
        if (handParent == null)
            handParent = transform.parent;
    }

    // ----- Hover -----
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging) return;

        originalPosition = rectTransform.localPosition;
        rectTransform.localScale = originalScale * hoverScale;
        rectTransform.localPosition = originalPosition + Vector3.up * hoverMoveUp;
        // Glow 켜기 등 효과를 여기서
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, transform.position);
            CardZoomManager.Instance.ShowZoom(cardController.data, screenPos);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDragging) return;

        rectTransform.localScale = originalScale;
        rectTransform.localPosition = originalPosition;
        // Glow 끄기 등
    }

    // ----- Drag -----
    public void OnBeginDrag(PointerEventData eventData)
    {
        
        isDragging = true;

        originalParent = transform.parent;
        originalPosition = rectTransform.position;

        // 드래그 중엔 상단에 보이도록
        transform.SetParent(canvas.transform, true);

        var cg = GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.blocksRaycasts = false; // 다른 DropZone이 제대로 감지하도록
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint);

        rectTransform.position = canvas.transform.TransformPoint(localPoint);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        var cg = GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.blocksRaycasts = true;
        }

        // Drop 위치가 playZone 근처인지 검사 (간단 버전)
        if (playZone != null &&
            RectTransformUtility.RectangleContainsScreenPoint(
                playZone as RectTransform,
                eventData.position,
                eventData.pressEventCamera)&&canAffordCost)
        {
            // 플레이 시도
            TryPlayCard();
        }
        else
        {
            // 원래 손 위치로 되돌리기
            transform.SetParent(originalParent, true);
            rectTransform.position = originalPosition;
            rectTransform.localScale = originalScale;
            
        }
    }

    private void TryPlayCard()
    {
        if (cardController == null)
        {
            // 그냥 사용된 것으로 치고 파괴
            Destroy(gameObject);
            return;
        }

        // CardController.Play() 내에서 에너지 체크 및 실제 사용 처리
        cardController.Play();
    }
    private void UpdateCostAffordability()
    {
        if (cardController != null && cardController.player != null && cardController.data != null)
        {
            canAffordCost = cardController.player.CanPayCost(cardController.data.cost);
        }
    }

}
