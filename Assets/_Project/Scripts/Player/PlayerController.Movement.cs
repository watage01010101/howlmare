using UnityEngine;

public partial class PlayerController
{
    private void Move()
    {
        float currentSpeed = isRunning ? runSpeed : speed;
        float targetSpeed = moveInput * currentSpeed;
        float accelRate = GetAccelerationRate(targetSpeed);

        float newVelocityX = Mathf.MoveTowards(
            rb.linearVelocity.x,
            targetSpeed,
            accelRate * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);
        UpdateFacingDirection();
    }

    private float GetAccelerationRate(float targetSpeed)
    {
        bool isStopping = Mathf.Abs(targetSpeed) < StopInputThreshold;
        float accelRate = isStopping ? GetDecelerationRate() : acceleration;

        if (!isGrounded)
        {
            accelRate *= airControlMultiplier;
        }

        return accelRate;
    }

    private float GetDecelerationRate()
    {
        if (!isGrounded && Input.GetKey(KeyCode.Space))
        {
            return deceleration * AirHoldDecelerationMultiplier;
        }

        return deceleration;
    }

    private void UpdateFacingDirection()
    {
        float scaleX = Mathf.Abs(transform.localScale.x);

        if (moveInput > 0)
        {
            transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-scaleX, transform.localScale.y, transform.localScale.z);
        }
    }
}
