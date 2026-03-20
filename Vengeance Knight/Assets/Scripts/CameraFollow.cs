using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 5f;

    public float heightOffset = 5f;

    private Vector3 offset;

    void Start()
    {
        // Keep your original offset (THIS preserves horizontal lock)
        offset = transform.position - player.position;
    }

    void LateUpdate()
{
    Vector3 targetPosition = transform.position;

    // ✅ Main sideways follow (your working axis)
    targetPosition.z = player.position.z + offset.z;

    // ✅ NEW: allow slight movement in X (path follow)
    float followAmount = 0.3f; // tweak this (0 = locked, 1 = full follow)
    targetPosition.x = Mathf.Lerp(transform.position.x, player.position.x + offset.x, followAmount);

    // ✅ Terrain height (your working fix)
    float terrainHeight = Terrain.activeTerrain.SampleHeight(player.position);
    targetPosition.y = terrainHeight + heightOffset;

    transform.position = Vector3.Lerp(
        transform.position,
        targetPosition,
        smoothSpeed * Time.deltaTime
    );
}
}