using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerController : MonoBehaviour
{
    [Header("전투 설정 (Hitbox)")]
    public float attackRange = 1.2f;   // 플레이어 중심에서 타격 지점까지의 거리
    public float attackRadius = 1.0f;  // 타격 판정 원의 반지름 (히트박스 크기)
    public LayerMask enemyLayer;       // 적을 판별할 레이어
    public float attackCooldown = 0.4f; // 공격 재사용 대기시간
    private bool canAttack = true;
    
    [Header("이동 및 점프")]
    public float moveSpeed = 8f;
    public float jumpForce = 15f;
    private float horizontalInput;

    [Header("대시")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    private bool isDashing;
    private bool canDash = true;

    [Header("환경 설정")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    private Rigidbody2D rb;
    private Vector2 mousePos;
    private Vector2 lastAttackCenter;
    private bool showHitboxGizmo = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.freezeRotation = true;
    }

    void Update()
    {
        // 키보드나 마우스가 연결되어 있지 않으면 에러를 방지합니다.
        if (Keyboard.current == null || Mouse.current == null) return;

        if (isDashing) return;

        HandleInput();
        HandleAiming();
        HandleCombatInput();
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        CheckGrounded();
        Move();
    }

    // 1. 키보드 직접 입력 처리
    void HandleInput()
    {
        // WASD 이동 (누르고 있는 동안 - isPressed)
        horizontalInput = 0f;
        if (Keyboard.current.aKey.isPressed) horizontalInput -= 1f;
        if (Keyboard.current.dKey.isPressed) horizontalInput += 1f;

        // 점프 (이번 프레임에 눌렀을 때 - wasPressedThisFrame)
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            Jump();
        }

        // 대시
        if (Keyboard.current.shiftKey.wasPressedThisFrame && canDash)
        {
            StartCoroutine(DashRoutine());
        }

        // 상호작용 (에고 기프트 획득 등)
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            Debug.Log("F: 에고 기프트 획득 등 상호작용 실행");
        }
        
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    // 2. 마우스 위치 추적 및 에임
    void HandleAiming()
    {
        // 마우스 스크린 좌표를 읽어와 월드 좌표로 변환
        Vector2 screenPosition = Mouse.current.position.ReadValue();
        mousePos = Camera.main.ScreenToWorldPoint(screenPosition);

        if (mousePos.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else if (mousePos.x < transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    // 3. 전투 및 인격 스킬 입력
    void HandleCombatInput()
    {
        if (!canAttack) return;
        Vector2 attackDirection = (mousePos - (Vector2)transform.position).normalized;

        // 마우스 클릭
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartCoroutine(PerformMeleeAttack(attackDirection, "일반 공격"));
        }
        else if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            StartCoroutine(PerformMeleeAttack(attackDirection, "강공격"));
        }

        // Q, E 인격 스킬
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            Debug.Log("Q: 인격 스킬 1 발동");
        }
        else if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("E: 인격 스킬 2 발동");
        }
    }
    IEnumerator PerformMeleeAttack(Vector2 direction, string attackType)
    {
        canAttack = false;

        // 1. 타격 중심점 계산: 플레이어 위치 + (마우스 방향 * 공격 사거리)
        Vector2 hitboxCenter = (Vector2)transform.position + direction * attackRange;
        lastAttackCenter = hitboxCenter; // 기즈모 표시용 저장

        // 2. 해당 원형 영역 안에 있는 'Enemy 레이어'를 가진 모든 콜라이더 감지
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(hitboxCenter, attackRadius, enemyLayer);

        // 3. 감지된 적들에게 데미지 처리
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log($"[히트!] {enemy.name}에게 {attackType} 적중!");
            
            // 적 스크립트를 가져와 데미지를 입히는 로직 (아래 Enemy 스크립트 참고)
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(10); // 기본 데미지 10 적용
            }
        }

        // 4. 시각적 피드백 (에디터 기즈모 표시)
        StartCoroutine(ShowHitboxVisual());

        // 5. 쿨타임 대기
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
    
    private IEnumerator DashRoutine()
    {
        canDash = false;
        isDashing = true;
        
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f; 

        float dashDirection = horizontalInput != 0 ? Mathf.Sign(horizontalInput) : transform.localScale.x;
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void CheckGrounded()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
    }
    IEnumerator ShowHitboxVisual()
    {
        showHitboxGizmo = true;
        yield return new WaitForSeconds(0.15f); // 0.15초 동안 타격 판정 표시
        showHitboxGizmo = false;
    }
    private void OnDrawGizmos()
    {
        // 1. 기존 바닥 감지 (빨간색)
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // 2. 공격 범위 시각화 (노란색 선 - 에디터에서 상시 확인 가능)
        // 게임 실행 중이 아니더라도 대략적인 타격 범위를 가늠하기 위함
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // 3. 실제 타격 발생 시 히트박스 시각화 (빨간색 채워진 원)
        if (showHitboxGizmo)
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f); // 반투명한 빨간색
            Gizmos.DrawSphere(lastAttackCenter, attackRadius);
        }
    }
}