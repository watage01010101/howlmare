using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public float lookOffsetY = 3f;      // 上下の視点移動量
    public float lookSpeed = 50f;       // 視点の追従速度
    public float inputDelay = 0.3f;

    private float inputStartTime;
    private bool isInputActive;
    private float currentOffsetY;

    void LateUpdate()
    {
        if (target == null) return;

        // ★ 先に入力を取得する（これが重要）
        float inputY = Input.GetAxisRaw("Vertical");

        // 入力開始を検知
        if (inputY != 0)
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

        // 一定時間経過後にのみ反映
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
}
