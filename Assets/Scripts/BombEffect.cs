using UnityEngine;

public class BombEffect : MonoBehaviour
{
    public float radius = 2.5f;
    public float duration = 0.5f;

    void Start()
    {
        // Hancurkan semua musuh di sekitar
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Destroy(hit.gameObject);
            }
        }

        // Hancurkan efek setelah delay
        Destroy(gameObject, duration);
    }
}
