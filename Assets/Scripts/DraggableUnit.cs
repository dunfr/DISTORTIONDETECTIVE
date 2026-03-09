using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class DraggableUnit : MonoBehaviour
{
    private Transform originalParent; // 원래 있던 슬롯의 위치를 기억
    private Image unitImage;

    void Awake()
    {
        unitImage = GetComponent<Image>();
    }

    // 1. 드래그 시작 (마우스를 누르고 움직일 때)
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent; // 돌아갈 곳 기억
        
        // 유닛이 다른 UI에 가려지지 않도록 캔버스 최상단으로 빼줍니다.
        transform.SetParent(transform.root); 
        transform.SetAsLastSibling();

        // 중요! 내가 드래그하는 이미지 때문에 내 밑에 있는 슬롯이 클릭을 못 받는 걸 방지
        unitImage.raycastTarget = false; 
    }

    // 2. 드래그 중 (마우스를 따라다님)
    public void OnDrag(PointerEventData eventData)
    {
        // 마우스 위치로 유닛 이미지를 이동
        transform.position = Input.mousePosition;
    }

    // 3. 드래그 종료 (마우스 버튼을 놨을 때)
    public void OnEndDrag(PointerEventData eventData)
    {
        // 다시 클릭을 받을 수 있게 켜줌
        unitImage.raycastTarget = true;
        // 마우스를 놓은 곳 아래에 'Slot(슬롯)'이 있는지 확인 (EventData 활용)
        GameObject objectBelowMouse = eventData.pointerCurrentRaycast.gameObject;

        if (objectBelowMouse != null && objectBelowMouse.CompareTag("BenchSlot"))
        {
            // 성공적으로 다른 슬롯 위에 놓았을 때!
            // 새로운 슬롯의 자식으로 들어가고 위치를 맞춤
            transform.SetParent(objectBelowMouse.transform);
            transform.position = objectBelowMouse.transform.position;
            
            // TODO: 여기서 BenchManager.SwapUnit()을 호출하여 데이터 배열도 실제 위치에 맞게 바꿔줘야 합니다.
            Debug.Log("자리를 옮겼습니다!");
        }
        else
        {
            // 허공이나 이상한 곳에 드롭했다면? 원래 자리로 튕겨 돌아감
            transform.SetParent(originalParent);
            transform.position = originalParent.position;
            Debug.Log("잘못된 위치입니다. 원래 자리로 돌아갑니다.");
        }
    }
}
