using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 5f;
    public float heightOffset = 5f;

    public float blendHeight = 3f;
    public float blendSpeed = 5f;

    private Vector3 offset;
    private float currentBlend;

    void Start()
    {
        offset = transform.position - player.position;
    }

    void LateUpdate()
    {
        Vector3 targetPosition = transform.position;

        targetPosition.z = player.position.z + offset.z;

        float followAmount = 0.3f;
        targetPosition.x = Mathf.Lerp(transform.position.x, player.position.x + offset.x, followAmount);

        Ray ray = new Ray(player.position + Vector3.up * 2f, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f))
        {
            float terrainY = hit.point.y + heightOffset;
            float playerY = player.position.y + heightOffset;

            float heightDiff = Mathf.Abs(player.position.y - hit.point.y);
            float targetBlend = Mathf.Clamp01(heightDiff / blendHeight);

            currentBlend = Mathf.Lerp(currentBlend, targetBlend, blendSpeed * Time.deltaTime);

            targetPosition.y = Mathf.Lerp(terrainY, playerY, currentBlend);
        }

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}