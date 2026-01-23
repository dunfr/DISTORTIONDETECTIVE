using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CardZoomManager : MonoBehaviour
{
    public static CardZoomManager Instance;
    [Header("줌 설정")]
    public GameObject zoomPrefab;        // 큰 카드 프리팹
    public Canvas zoomCanvas;            // 별도 Canvas (Render Mode: Screen Space - Camera)
    public float zoomScale = 1.5f;
    public Vector3 screenOffset = new Vector3(0, -100, 0);
    
    private RectTransform currentZoomCard;
    private CardData lastZoomedCard;
    private bool isZooming = false;
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 넘어가도 유지 (선택)
    }

    public void ShowZoom(CardData cardData, Vector3 screenPos)
    {
        // 기존 줌 끄기
        HideZoom();

        if (zoomPrefab == null || cardData == null) return;

        // 큰 카드 생성
        var zoomObj = Instantiate(zoomPrefab, zoomCanvas.transform);
        currentZoomCard = zoomObj.GetComponent<RectTransform>();
        
        var zoomView = zoomObj.GetComponent<CardView>();
        if (zoomView != null) zoomView.Setup(cardData);

        // 화면 중앙에서 약간 아래로 위치
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            zoomCanvas.transform as RectTransform,
            screenPos + screenOffset,
            null,
            out Vector2 localPoint);

        currentZoomCard.anchoredPosition = localPoint;
        currentZoomCard.localScale = Vector3.one * zoomScale;

        lastZoomedCard = cardData;
        isZooming = true;
    }
    public void HideZoom()
    {
        if (currentZoomCard != null)
        {
            Destroy(currentZoomCard.gameObject);
            currentZoomCard = null;
            isZooming = false;
        }
    }
    
    void Update()
    {
        // 우클릭 떼면 줌 종료
        if (isZooming&&!Mouse.current.rightButton.isPressed)
        {
            HideZoom();
        }
    }
}
