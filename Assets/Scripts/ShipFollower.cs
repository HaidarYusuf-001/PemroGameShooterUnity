using UnityEngine;

public class ShipFollower : MonoBehaviour
{
    public Transform player;
    public Vector2 offset;                 // Posisi target relatif terhadap player
    public float followSpeed = 5f;
    public bool isEntering = false;

    private bool escaping = false;
    private Vector2 escapeDirection;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireInterval = 1f;
    private float fireTimer;

    public float rotationSpeed = 200f; // kecepatan rotasi ke arah musuh

    private GameObject cachedTarget;
    private float scanCooldown = 5f; // cek musuh tiap 5 detik
    private float scanTimer = 0f;


    void Update()
    {
        if (player == null && !escaping) return;

        if (escaping)
        {
            transform.Translate(escapeDirection * followSpeed * Time.deltaTime);
            if (Mathf.Abs(transform.position.x) > 30 || Mathf.Abs(transform.position.y) > 30)
            {
                Destroy(gameObject);
            }
        }
        else if (isEntering)
        {
            Vector2 targetPosition = (Vector2)player.position + offset;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                isEntering = false;
            }
        }
        else
        {
            Vector2 targetPosition = (Vector2)player.position + offset;
            transform.position = Vector2.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }

        if (!isEntering && !escaping)
        {
            scanTimer -= Time.deltaTime;
            if (scanTimer <= 0f)
            {
                cachedTarget = FindNearestEnemy();
                scanTimer = scanCooldown;
            }

            if (cachedTarget != null)
            {
                RotateToTarget(cachedTarget.transform.position);
            }

            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f)
            {
                Shoot();
                fireTimer = fireInterval;
            }
        }

    }

    void RotateToTarget(Vector3 targetPos)
    {
        Vector2 direction = (targetPos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }


    GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearest = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                nearest = enemy;
            }
        }

        return nearest;
    }

    public void StartEscape()
    {
        escaping = true;

        if (player != null)
        {
            escapeDirection = (transform.position - player.position).normalized;
        }
        else
        {
            escapeDirection = Vector2.up;
        }
    }

    private ShieldController shield;

    void Start()
    {
        shield = GetComponentInChildren<ShieldController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet") || collision.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);

            if (shield != null && shield.AbsorbHit())
            {
                return;
            }

            Destroy(gameObject);
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = firePoint.up * 8f;
        }
    }
}
