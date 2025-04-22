using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject bombEffectPrefab; // assign BombEffect prefab dari Inspector
    public float speed = 10f;

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Buat efek ledakan di posisi bomb
            if (bombEffectPrefab != null)
            {
                Instantiate(bombEffectPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject); // hancurkan bomb
        }
    }
}
