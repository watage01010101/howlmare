using UnityEngine;

public partial class PlayerController
{
    private void HandleInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        isJumpHeld = Input.GetKey(KeyCode.Space);

        if (moveInput != 0)
        {
            lastMoveTime = Time.time;
        }

        HandleRunInput();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpRequest = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            jumpReleased = true;
        }
    }

    private void HandleRunInput()
    {
        int direction = GetPressedHorizontalDirection();

        if (direction != 0)
        {
            if (direction == lastDirection && Time.time - lastTapTime < doubleTapTime)
            {
                isDoubleTapRunning = true;
            }

            lastDirection = direction;
            lastTapTime = Time.time;
        }

        bool isCtrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
        {
            isDoubleTapRunning = false;
        }

        if (!isCtrlPressed && Time.time - lastMoveTime > runGraceTime)
        {
            isDoubleTapRunning = false;
        }

        isRunning = isCtrlPressed || isDoubleTapRunning;
    }

    private int GetPressedHorizontalDirection()
    {
        int direction = 0;

        if (Input.GetKeyDown(KeyCode.A))
        {
            direction = -1;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            direction = 1;
        }

        return direction;
    }
}
