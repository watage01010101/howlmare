using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerController
{
    private void HandleInput()
    {
        moveInput = GetHorizontalInput();
        isJumpHeld = IsJumpPressed();

        if (moveInput != 0f)
        {
            lastMoveTime = Time.time;
        }

        HandleRunInput();

        if (WasJumpPressedThisFrame())
        {
            jumpRequest = true;
        }

        if (WasJumpReleasedThisFrame())
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

        bool isSprintPressed = IsSprintPressed();

        if (WasSprintReleasedThisFrame())
        {
            isDoubleTapRunning = false;
        }

        if (!isSprintPressed && Time.time - lastMoveTime > runGraceTime)
        {
            isDoubleTapRunning = false;
        }

        isRunning = isSprintPressed || isDoubleTapRunning;
    }

    private int GetPressedHorizontalDirection()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return 0;
        }

        if (keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame)
        {
            return -1;
        }

        if (keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame)
        {
            return 1;
        }

        return 0;
    }

    private float GetHorizontalInput()
    {
        float input = 0f;
        Keyboard keyboard = Keyboard.current;

        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            {
                input -= 1f;
            }

            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            {
                input += 1f;
            }
        }

        Gamepad gamepad = Gamepad.current;

        if (gamepad != null)
        {
            input += gamepad.leftStick.x.ReadValue();
        }

        return Mathf.Clamp(input, -1f, 1f);
    }

    private bool IsJumpPressed()
    {
        Keyboard keyboard = Keyboard.current;
        Gamepad gamepad = Gamepad.current;

        return (keyboard != null && keyboard.spaceKey.isPressed)
            || (gamepad != null && gamepad.buttonSouth.isPressed);
    }

    private bool WasJumpPressedThisFrame()
    {
        Keyboard keyboard = Keyboard.current;
        Gamepad gamepad = Gamepad.current;

        return (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
            || (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame);
    }

    private bool WasJumpReleasedThisFrame()
    {
        Keyboard keyboard = Keyboard.current;
        Gamepad gamepad = Gamepad.current;

        return (keyboard != null && keyboard.spaceKey.wasReleasedThisFrame)
            || (gamepad != null && gamepad.buttonSouth.wasReleasedThisFrame);
    }

    private bool IsSprintPressed()
    {
        Keyboard keyboard = Keyboard.current;
        Gamepad gamepad = Gamepad.current;

        return (keyboard != null
                && (keyboard.leftCtrlKey.isPressed
                    || keyboard.rightCtrlKey.isPressed
                    || keyboard.leftShiftKey.isPressed))
            || (gamepad != null && gamepad.leftStickButton.isPressed);
    }

    private bool WasSprintReleasedThisFrame()
    {
        Keyboard keyboard = Keyboard.current;
        Gamepad gamepad = Gamepad.current;

        return (keyboard != null
                && (keyboard.leftCtrlKey.wasReleasedThisFrame
                    || keyboard.rightCtrlKey.wasReleasedThisFrame
                    || keyboard.leftShiftKey.wasReleasedThisFrame))
            || (gamepad != null && gamepad.leftStickButton.wasReleasedThisFrame);
    }
}
