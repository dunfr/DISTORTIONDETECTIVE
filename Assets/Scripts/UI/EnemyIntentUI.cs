using UnityEngine;
using UnityEngine.UI;
public class EnemyIntentUI : MonoBehaviour
{
    public Image iconImage;
    public Text valueText;

    public void SetIntent(EnemyAction action)
    {
        if (action == null)
        {
            iconImage.enabled = false;
            valueText.text = "";
            return;
        }

        iconImage.enabled = true;
        iconImage.sprite = action.intentIcon;
        iconImage.color = action.intentColor;

        // 공격/방어 수치 중 의미 있는 값 표시
        int value = 0;
        if (action.type == EnemyActionType.Attack ||
            action.type == EnemyActionType.AttackDefend)
        {
            value = action.attackDamage;
        }
        else if (action.type == EnemyActionType.Defend)
        {
            value = action.blockAmount;
        }

        valueText.text = value.ToString();
    }
}
