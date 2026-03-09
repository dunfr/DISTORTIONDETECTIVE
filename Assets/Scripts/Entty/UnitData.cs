using UnityEngine;

public class UnitData
{
    public int unitID;         // 유닛 고유 번호 (예: 1=전사, 2=궁수)
    public string unitName;    // 유닛 이름
    public int starLevel;      // 성급 (1성, 2성, 3성)

    // 생성자
    public UnitData(int id, string name, int star)
    {
        unitID = id;
        unitName = name;
        starLevel = star;
    }
}
