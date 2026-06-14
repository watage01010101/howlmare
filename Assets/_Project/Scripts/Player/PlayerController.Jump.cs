using UnityEngine;

public partial class PlayerController
{
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

        if (Input.GetKey(KeyCode.Space) && rb.linearVelocity.y > 0 && !jumpReleased)
        {
            rb.linearVelocity += Vector2.up * HoldJumpBoost * Time.fixedDeltaTime;
        }
    }

    private void GroundJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        canAirJump = true;
    }

    private void AirJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * AirJumpMultiplier);
        canAirJump = false;
    }

    private void WallJump()
    {
        float jumpX = -wallDirection * WallJumpHorizontalForce;

        rb.linearVelocity = new Vector2(jumpX, jumpForce);
        isTouchingWall = false;
        canAirJump = true;
    }

    private void UpdateCoyoteTimers()
    {
        if (groundCoyoteTimer > 0)
        {
            groundCoyoteTimer -= Time.fixedDeltaTime;
        }

        if (wallCoyoteTimer > 0)
        {
            wallCoyoteTimer -= Time.fixedDeltaTime;
        }
    }

    private void ApplyJumpCut()
    {
        if (!jumpReleased)
        {
            return;
        }

        if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * JumpCutMultiplier);
        }

        jumpReleased = false;
    }

    private void ApplyBetterFall()
    {
        if (isTouchingWall && !isGrounded && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, WallSlideSpeed);
            return;
        }

        if (!isGrounded && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * Time.fixedDeltaTime;
        }
    }
}
