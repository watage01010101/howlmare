using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
    private const string GroundTag = "Ground";
    private const float GroundNormalThreshold = 0.5f;
    private const float CeilingNormalThreshold = -0.5f;
    private const float WallNormalThreshold = 0.1f;

    private readonly Dictionary<Collider2D, ContactState> contactStates = new Dictionary<Collider2D, ContactState>();

    private struct ContactState
    {
        public bool isGrounded;
        public bool isTouchingWall;
        public int wallDirection;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag(GroundTag))
        {
            return;
        }

        ContactState contactState = default;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            HandleContact(contact, ref contactState);
        }

        contactStates[collision.collider] = contactState;
        UpdateContactState();
    }

    private void HandleContact(ContactPoint2D contact, ref ContactState contactState)
    {
        if (contact.normal.y > GroundNormalThreshold)
        {
            HandleGroundContact(ref contactState);
        }
        else if (contact.normal.y < CeilingNormalThreshold)
        {
            HandleCeilingContact();
        }
        else if (Mathf.Abs(contact.normal.x) > WallNormalThreshold)
        {
            HandleWallContact(contact.normal.x, ref contactState);
        }
    }

    private void HandleGroundContact(ref ContactState contactState)
    {
        contactState.isGrounded = true;
        canAirJump = true;

        if (rb.linearVelocity.y < 0f)
        {
            SetVelocityY(0f);
        }
    }

    private void HandleCeilingContact()
    {
        if (rb.linearVelocity.y > 0f)
        {
            SetVelocityY(0f);
        }
    }

    private void HandleWallContact(float normalX, ref ContactState contactState)
    {
        int detectedWallDirection = normalX > 0f ? -1 : 1;
        int facingDirection = transform.localScale.x > 0f ? 1 : -1;

        if (facingDirection != detectedWallDirection)
        {
            return;
        }

        contactState.isTouchingWall = true;
        contactState.wallDirection = detectedWallDirection;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag(GroundTag))
        {
            return;
        }

        contactStates.Remove(collision.collider);
        UpdateContactState();
    }

    private void UpdateContactState()
    {
        isGrounded = false;
        isTouchingWall = false;

        foreach (ContactState contactState in contactStates.Values)
        {
            if (contactState.isGrounded)
            {
                isGrounded = true;
            }

            if (contactState.isTouchingWall)
            {
                isTouchingWall = true;
                wallDirection = contactState.wallDirection;
            }
        }
    }
}
