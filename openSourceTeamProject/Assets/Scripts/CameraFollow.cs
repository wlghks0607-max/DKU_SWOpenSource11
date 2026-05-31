using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float xSmooth = 10f;
    public float ySmooth = 3f;
    public float lockYThreshold = -4f; // 이 높이 아래면 Y 고정


    void LateUpdate()
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Lerp(
            pos.x,
            target.position.x,
            xSmooth * Time.deltaTime
        );

        if (target.position.y > lockYThreshold)
        {
            pos.y = Mathf.Lerp(
                pos.y,
                target.position.y,
                ySmooth * Time.deltaTime
            );
        }

        transform.position = pos;
    }
}