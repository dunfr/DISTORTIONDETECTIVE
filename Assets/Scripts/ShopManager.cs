using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public BenchManager benchManager;
    public int playerGold = 10; // 테스트용 임시 골드
    // 상점 UI의 구매 버튼을 누르면 이 함수가 실행되도록 연결합니다.
    public void OnClickBuyUnit(UnitData selectedUnit, int cost)
    {
        // 1. 골드가 충분한지 확인
        if (playerGold >= cost)
        {
            // 2. 대기석에 빈자리가 있는지 확인하고 데이터 추가
            if (benchManager.AddUnit(selectedUnit))
            {
                // 3. 구매 성공! 골드 차감
                playerGold -= cost;
                Debug.Log($"구매 성공! 남은 골드: {playerGold}");

                // 4. (중요) 데이터가 들어갔으니, 화면에도 유닛을 생성해서 보여줍니다.
                SpawnVisualUnit(selectedUnit); 
            }
            else
            {
                // AddUnit이 false를 반환 = 대기석이 꽉 참
                Debug.Log("대기석이 가득 차서 구매할 수 없습니다!");
            }
        }
        else
        {
            Debug.Log("골드가 부족합니다!");
        }
    }

    private void SpawnVisualUnit(UnitData unit)
    {
        // TODO: 빈 슬롯의 위치(Transform)를 찾아 유닛 프리팹(이미지)을 생성하는 로직
    }
}
