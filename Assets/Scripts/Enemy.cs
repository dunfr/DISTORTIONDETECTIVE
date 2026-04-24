using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 50;
    private int currentHealth;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name}가 {damage}의 데미지를 입음! (남은 체력: {currentHealth})");

        // 피격 시 하얗게 번쩍이는 효과 (시각적 피드백)
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashRoutine());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private System.Collections.IEnumerator FlashRoutine()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red; // 맞으면 빨갛게 변함
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} 처치됨!");
        // 추후 에고 기프트 드랍 로직 등을 여기에 추가
        Destroy(gameObject);
    }
}