using UnityEngine;

public class CameraObstruction : MonoBehaviour
{
    public Transform player;
    public LayerMask obstructionMask;

    private Renderer currentRenderer;
    private Color originalColor;

    void Update()
    {
        Ray ray = new Ray(transform.position, player.position - transform.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Vector3.Distance(transform.position, player.position), obstructionMask))
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();

            if (rend != null)
            {
                if (currentRenderer != rend)
                {
                    ResetObject();
                    currentRenderer = rend;
                    originalColor = rend.material.color;
                }

                Color c = rend.material.color;
                c.a = 0.3f;
                rend.material.color = c;
            }
        }
        else
        {
            ResetObject();
        }
    }

    void ResetObject()
    {
        if (currentRenderer != null)
        {
            currentRenderer.material.color = originalColor;
            currentRenderer = null;
        }
    }
}