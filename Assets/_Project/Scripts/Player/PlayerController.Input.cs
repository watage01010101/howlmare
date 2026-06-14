using UnityEngine;

public partial class PlayerController
{
    private void HandleInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput != 0)
        {
            lastMoveTime = Time.time;
        }

        HandleRunInput();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpRequest = true;
        }
    }

    private void HandleRunInput()
    {
        int direction = GetPressedHorizontalDirection();

        if (direction != 0)
        {
            if (direction == lastDirection && Time.time - lastTapTime < doubleTapTime)
            {
                isRunning = true;
            }

            lastDirection = direction;
            lastTapTime = Time.time;
        }

        bool isCtrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        bool doubleTapRunning = isRunning;

        isRunning = false;

        if (isCtrlPressed)
        {
            isRunning = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
        {
            isRunning = false;
        }
        else if (doubleTapRunning)
        {
            isRunning = true;
        }

        if (!isCtrlPressed && Time.time - lastMoveTime > runGraceTime)
        {
            isRunning = false;
        }
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
