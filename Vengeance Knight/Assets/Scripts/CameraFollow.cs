using UnityEngine;
 
public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 5f;
    public float heightOffset = 5f;
 
    private Vector3 offset;
 
    void Start()
    {
        offset = transform.position - player.position;
    }
 
    void LateUpdate()
    {
        Vector3 targetPosition = transform.position;
 
        // Follow player sideways
        targetPosition.z = player.position.z + offset.z;
 
        // Slight depth follow
        float followAmount = 0.3f;
        targetPosition.x = Mathf.Lerp(transform.position.x, player.position.x + offset.x, followAmount);
 

        if (Terrain.activeTerrain != null)
        {
            float terrainHeight = Terrain.activeTerrain.SampleHeight(player.position);
            targetPosition.y = terrainHeight + heightOffset;
        }
        else
        {
            // No terrain — just follow player height
            targetPosition.y = player.position.y + heightOffset;
        }
 
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}