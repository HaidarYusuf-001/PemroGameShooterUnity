using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 2f;

    [Header("Shooting")]
    public GameObject bulletPrefab;           // Prefab peluru musuh
    public Transform firePoint;               // Titik tembak (empty object di depan musuh)
    public float shootInterval = 2f;          // Interval waktu antar tembakan
    private float shootTimer;
    public bool isActiveMovement = true;

    private Transform player;

    void Start()
    {
        shootTimer = shootInterval;

        // Cari player berdasarkan tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {

        if (isActiveMovement)
        {
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.Self);
        }


        // Auto destroy jika keluar layar bawah
        if (transform.position.y < Camera.main.ViewportToWorldPoint(Vector3.zero).y - 1f)
        {
            Destroy(gameObject);
        }

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        // Rotasi menghadap ke player
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // Tembak ke arah player
        if (player != null)
        {
            shootTimer -= Time.deltaTime;

            if (shootTimer <= 0f)
            {
                Shoot();
                shootTimer = shootInterval;
            }
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = firePoint.up * 5f; // Tembak ke arah depan enemy (yang sudah menghadap player)
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Destroy(collision.gameObject); // Hancurkan peluru
            Destroy(gameObject);           // Hancurkan enemy
        }
    }
}
