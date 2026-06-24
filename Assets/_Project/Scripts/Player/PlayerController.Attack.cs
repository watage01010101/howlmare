using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerController
{
    [SerializeField] private bool showAttackHitbox = true;
    [SerializeField] private float attackHitboxDisplayTime = 0.3f;

    private Vector2 lastAttackCenter;
    private float lastAttackTime;

    private void HandleAttack()
    {
        if (!WasAttackPressedThisFrame())
        {
            return;
        }

        Vector2 attackCenter = GetAttackCenter();
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackCenter, attackRange, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemyHealth = hit.GetComponentInParent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }

        lastAttackCenter = attackCenter;
        lastAttackTime = Time.time;
    }

    private Vector2 GetAttackCenter()
    {
        float facingDirection = transform.localScale.x >= 0f ? 1f : -1f;
        return (Vector2)transform.position + Vector2.right * facingDirection * attackOffset;
    }

    private bool WasAttackPressedThisFrame()
    {
        Keyboard keyboard = Keyboard.current;
        Gamepad gamepad = Gamepad.current;
        Mouse mouse = Mouse.current;

        return (keyboard != null && keyboard.jKey.wasPressedThisFrame)
            || (gamepad != null && gamepad.buttonWest.wasPressedThisFrame)
            || (mouse != null && mouse.leftButton.wasPressedThisFrame);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GetAttackCenter(), attackRange);
    }

    private void OnDrawGizmos()
        {
            if (!showAttackHitbox)
            {
                return;
            }

            if (Application.isPlaying)
            {
                if (Time.time - lastAttackTime > attackHitboxDisplayTime)
                {
                    return;
                }

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(lastAttackCenter, attackRange);
                return;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(GetAttackCenter(), attackRange);
        }
}
