using UnityEngine;

public partial class PlayerController
{
    private const float JumpCutMultiplier = 0.5f;
    private const float AirJumpMultiplier = 0.9f;
    private const float HoldJumpBoost = 10f;
    private const float WallJumpHorizontalForce = 8f;
    private const float WallSlideSpeed = -2f;

    private void Jump()
    {
        if (!jumpRequest)
        {
            return;
        }

        if (!isGrounded && isTouchingWall && rb.linearVelocity.y < 0f)
        {
            WallJump();
        }
        else if (isGrounded)
        {
            GroundJump();
        }
        else if (canAirJump)
        {
            AirJump();
        }

        jumpRequest = false;

        if (isJumpHeld && rb.linearVelocity.y > 0 && !jumpReleased)
        {
            rb.linearVelocity += Vector2.up * HoldJumpBoost * Time.fixedDeltaTime;
        }
    }

    private void GroundJump()
    {
        SetVelocityY(jumpForce);
        canAirJump = true;
    }

    private void AirJump()
    {
        SetVelocityY(jumpForce * AirJumpMultiplier);
        canAirJump = false;
    }

    private void WallJump()
    {
        float jumpX = -wallDirection * WallJumpHorizontalForce;

        SetVelocity(jumpX, jumpForce);
        isTouchingWall = false;
        canAirJump = true;
    }

    private void SetVelocityY(float velocityY)
    {
        SetVelocity(rb.linearVelocity.x, velocityY);
    }

    private void SetVelocity(float velocityX, float velocityY)
    {
        rb.linearVelocity = new Vector2(velocityX, velocityY);
    }

    private void ApplyJumpCut()
    {
        if (!jumpReleased)
        {
            return;
        }

        if (rb.linearVelocity.y > 0)
        {
            SetVelocityY(rb.linearVelocity.y * JumpCutMultiplier);
        }

        jumpReleased = false;
    }

    private void ApplyBetterFall()
    {
        if (isTouchingWall && !isGrounded && rb.linearVelocity.y < 0)
        {
            SetVelocityY(WallSlideSpeed);
            return;
        }

        if (!isGrounded && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * Time.fixedDeltaTime;
        }
    }
}
