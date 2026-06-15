using UnityEngine;

public partial class PlayerController
{
    private const string GroundTag = "Ground";
    private const float GroundNormalThreshold = 0.5f;
    private const float CeilingNormalThreshold = -0.5f;
    private const float WallNormalThreshold = 0.1f;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag(GroundTag))
        {
            return;
        }

        isGrounded = false;
        isTouchingWall = false;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            HandleContact(contact);
        }
    }

    private void HandleContact(ContactPoint2D contact)
    {
        if (contact.normal.y > GroundNormalThreshold)
        {
            HandleGroundContact();
        }
        else if (contact.normal.y < CeilingNormalThreshold)
        {
            HandleCeilingContact();
        }
        else if (Mathf.Abs(contact.normal.x) > WallNormalThreshold)
        {
            HandleWallContact(contact.normal.x);
        }
    }

    private void HandleGroundContact()
    {
        isGrounded = true;
        canAirJump = true;

        if (rb.linearVelocity.y < 0)
        {
            SetVelocityY(0f);
        }
    }

    private void HandleCeilingContact()
    {
        if (rb.linearVelocity.y > 0)
        {
            SetVelocityY(0f);
        }
    }

    private void HandleWallContact(float normalX)
    {
        int detectedWallDirection = normalX > 0 ? -1 : 1;
        int facingDirection = transform.localScale.x > 0 ? 1 : -1;

        if (facingDirection != detectedWallDirection)
        {
            return;
        }

        isTouchingWall = true;
        wallDirection = detectedWallDirection;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag(GroundTag))
        {
            return;
        }

        isGrounded = false;
        isTouchingWall = false;
    }
}
