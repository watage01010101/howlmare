using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public float lookOffsetY = 3f;
    public float lookSpeed = 50f;
    public float inputDelay = 0.3f;

    private float inputStartTime;
    private bool isInputActive;
    private float currentOffsetY;

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        float inputY = GetVerticalLookInput();

        if (inputY != 0f)
        {
            if (!isInputActive)
            {
                isInputActive = true;
                inputStartTime = Time.time;
            }
        }
        else
        {
            isInputActive = false;
        }

        float targetOffsetY = 0f;

        if (isInputActive && Time.time - inputStartTime > inputDelay)
        {
            targetOffsetY = inputY * lookOffsetY;
        }

        currentOffsetY = Mathf.Lerp(
            currentOffsetY,
            targetOffsetY,
            lookSpeed * Time.deltaTime
        );

        Vector3 targetPosition = new Vector3(
            target.position.x,
            target.position.y + currentOffsetY,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }

    private float GetVerticalLookInput()
    {
        float inputY = 0f;
        Keyboard keyboard = Keyboard.current;

        if (keyboard != null)
        {
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            {
                inputY += 1f;
            }

            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            {
                inputY -= 1f;
            }
        }

        Gamepad gamepad = Gamepad.current;

        if (gamepad != null)
        {
            inputY += gamepad.leftStick.y.ReadValue();
        }

        return Mathf.Clamp(inputY, -1f, 1f);
    }
}
