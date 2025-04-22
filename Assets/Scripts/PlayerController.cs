using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float acceleration = 5f;
    public float maxSpeed = 10f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float fireCooldown = 0.25f;
    private float lastFireTime;

    [Header("Bomb")]
    public GameObject bombPrefab;

    private Vector3 startPosition;       // Untuk menyimpan posisi awal player
    private float offScreenTimer = 0f;   // Timer saat player keluar layar
    private bool isOffScreen = false;    // Status apakah player di luar layar

    private bool isInvincible = false;
    private float invincibleTimer = 0f;
    public float invincibleDuration = 1f; // bisa diatur via Inspector

    private SpriteRenderer sr;
    private float blinkTimer = 0f;
    public float blinkInterval = 0.1f;

    public ShieldController shield;


    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        CheckOffScreen();
        RotateTowardsMouse();


        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        if (Input.GetKey(KeyCode.Space) && Time.time > lastFireTime + fireCooldown)
        {
            Shoot();
            lastFireTime = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            LaunchBomb();
        }

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            blinkTimer -= Time.deltaTime;
            if (blinkTimer <= 0f && sr != null)
            {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a == 1f ? 0.2f : 1f);
                blinkTimer = blinkInterval;
            }

            if (invincibleTimer <= 0f)
            {
                isInvincible = false;

                // Kembalikan transparansi normal
                if (sr != null)
                {
                    Color c = sr.color;
                    c.a = 1f;
                    sr.color = c;
                }
            }
        }

    }

    void RotateTowardsMouse()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseWorldPos - transform.position);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f); // -90f jika sprite default-nya menghadap atas
    }


    void CheckOffScreen()
    {
        Camera cam = Camera.main;
        Vector3 screenPoint = cam.WorldToViewportPoint(transform.position);

        bool isOutside = screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1;

        if (isOutside)
        {
            // Cek apakah benar-benar keluar dari batas safe area 
            Vector3 camPos = cam.transform.position;
            float maxDistance = 2f; // boleh diubah sesuai batas nyaman

            if (Vector3.Distance(transform.position, camPos) > maxDistance)
            {
                offScreenTimer += Time.deltaTime;

                if (offScreenTimer >= 1f)
                {
                    var pm = FindObjectOfType<PlayerManager>();
                    if (pm != null)
                    {
                        pm.PlayerOutOfBounds(this.gameObject);
                    }

                    if (explosionPrefab != null)
                    {
                        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                    }
                }
            }
            else
            {
                offScreenTimer = 0f;
            }
        }
        else
        {
            isOffScreen = false;
            offScreenTimer = 0f;
        }
    }



    void RespawnPlayer()
    {
        transform.position = startPosition;
        isOffScreen = false;
        offScreenTimer = 0f;


        // Reset velocity
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        shield.ResetShield();
    }

    public void ActivateInvincibility()
    {
        isInvincible = true;
        invincibleTimer = invincibleDuration;
        blinkTimer = 0f;

        // Transparan
        if (sr != null)
        {
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;
            sr.enabled = true;
        }
    }



    void FixedUpdate()
    {
        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(moveInput * acceleration, ForceMode2D.Force);
        }
    }

    void LaunchBomb()
    {
        if (bombPrefab != null && bulletSpawnPoint != null)
        {
            Instantiate(bombPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        }
    }


    void Shoot()
    {
        if (bulletPrefab != null && bulletSpawnPoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = bulletSpawnPoint.up * 10f; // arah tembak mengikuti bulletSpawnPoint
            }
        }
    }


    public GameObject explosionPrefab; // Drag prefab dari Inspector

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isInvincible) return;

        if (collision.CompareTag("EnemyBullet") || collision.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject); // Hancurkan peluru musuh

            if (shield != null && shield.AbsorbHit())
            {
                // Serangan ditahan shield
                return;
            }

            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity); // Tampilkan ledakan
            }

            Destroy(gameObject); // Hancurkan player
            GameObject.FindObjectOfType<PlayerManager>()?.PlayerDied();
        }
    }

}
