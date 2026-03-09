using UnityEngine;
using System.Collections.Generic;
public class BenchManager : MonoBehaviour
{
    private const int MAX_BENCH_SIZE = 9;

    // 대기석 데이터를 담을 배열 (빈 칸은 null이 됩니다)
    public UnitData[] benchSlots = new UnitData[MAX_BENCH_SIZE];

    // 1. 유닛 추가 (상점에서 샀을 때)
    public bool AddUnit(UnitData newUnit)
    {
        for (int i = 0; i < MAX_BENCH_SIZE; i++)
        {
            // 빈자리(null)를 찾으면 유닛을 넣고 true 반환
            if (benchSlots[i] == null)
            {
                benchSlots[i] = newUnit;
                Debug.Log($"{newUnit.unitName}이(가) 대기석 {i}번에 추가되었습니다.");
                return true;
            }
        }

        // 반복문을 다 돌았는데도 빈자리가 없다면?
        Debug.Log("대기석이 가득 찼습니다!");
        return false; // 구매 실패 처리용
    }

    // 2. 유닛 제거 (팔았을 때)
    public void RemoveUnit(int index)
    {
        if (index >= 0 && index < MAX_BENCH_SIZE && benchSlots[index] != null)
        {
            Debug.Log($"{benchSlots[index].unitName}을(를) 판매했습니다.");
            benchSlots[index] = null; // 해당 칸을 다시 빈자리로 만듦
        }
    }

    // 3. 유닛 위치 스왑 (드래그 앤 드롭으로 자리 바꿀 때)
    public void SwapUnit(int indexA, int indexB)
    {
        // 범위를 벗어나지 않는지 안전 검사
        if (indexA < 0 || indexA >= MAX_BENCH_SIZE || indexB < 0 || indexB >= MAX_BENCH_SIZE) return;

        // A와 B의 데이터를 교환 (임시 변수 temp 사용)
        UnitData temp = benchSlots[indexA];
        benchSlots[indexA] = benchSlots[indexB];
        benchSlots[indexB] = temp;

        Debug.Log($"{indexA}번 칸과 {indexB}번 칸의 유닛이 교체되었습니다.");
    }
    public void CheckAndMerge(UnitData addedUnit)
    {
        // 최대 성급(보통 3성)이면 합성하지 않음
        if (addedUnit.starLevel >= 3) return; 

        // 합성에 쓰일 유닛들의 인덱스를 모아둘 리스트
        List<int> sameUnitsIndices = new List<int>();

        // 대기석 배열을 쭉 훑어봅니다.
        for (int i = 0; i < MAX_BENCH_SIZE; i++)
        {
            if (benchSlots[i] != null)
            {
                // ID와 성급이 모두 같은 유닛을 찾으면 리스트에 인덱스 저장
                if (benchSlots[i].unitID == addedUnit.unitID && 
                    benchSlots[i].starLevel == addedUnit.starLevel)
                {
                    sameUnitsIndices.Add(i);
                }
            }
        }

        // 동일한 유닛이 3마리 모였다면? 합성 시작!
        if (sameUnitsIndices.Count >= 3)
        {
            Debug.Log($"{addedUnit.unitName} {addedUnit.starLevel}성 3마리 발견! 합성 진행!");

            // 1. 재료가 된 3마리 유닛 데이터를 대기석에서 지움 (null 처리)
            for (int i = 0; i < 3; i++) // 딱 3마리만 지움
            {
                int slotIndex = sameUnitsIndices[i];
                benchSlots[slotIndex] = null; 
            
                // TODO: 여기서 실제 화면에 있는 유닛 UI(오브젝트) 3개도 파괴(Destroy)해야 합니다.
            }

            // 2. 한 단계 높은 성급의 새로운 유닛 생성
            UnitData upgradedUnit = new UnitData(addedUnit.unitID, addedUnit.unitName, addedUnit.starLevel + 1);

            // 3. 업그레이드된 유닛을 다시 대기석에 추가
            AddUnit(upgradedUnit); 
        
            // TODO: 여기서 업그레이드된 새 유닛 UI(오브젝트)를 화면에 생성해야 합니다.

            // 4. (핵심!) 2성이 만들어졌는데, 기존에 2성이 2마리 더 있었다면? 3성으로 연쇄 합성!
            CheckAndMerge(upgradedUnit); 
        }
    }
}