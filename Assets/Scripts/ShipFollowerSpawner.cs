using UnityEngine;
using System.Collections.Generic;

public class ShipFollowerSpawner : MonoBehaviour
{
    public GameObject followerPrefab;
    public float spawnInterval = 3f;

    private float timer;
    private int currentIndex = 0;
    private List<Vector2> directions;
    private Transform player;
    private bool spawningActive = false; // <-- flag ini penting

    private List<GameObject> activeFollowers = new List<GameObject>();

    void Start()
    {
        // Siapkan arah 8 penjuru
        directions = new List<Vector2>()
        {
            Vector2.right,                     // Timur
            Vector2.left,                      // Barat
            Vector2.up,                        // Utara
            Vector2.down,                      // Selatan
            new Vector2(1, 1).normalized,      // Timur Laut
            new Vector2(-1, 1).normalized,     // Barat Laut
            new Vector2(1, -1).normalized,     // Tenggara
            new Vector2(-1, -1).normalized     // Barat Daya
        };
    }

    void Update()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                return;
            }
        }

        if (currentIndex >= directions.Count) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnFollower();
            timer = spawnInterval;
        }
    }


    void SpawnFollower()
    {
        Vector2 spawnOffset = directions[currentIndex] * 1.5f;
        Vector2 spawnPos = (Vector2)player.position + new Vector2(0, -6f); // Muncul dari bawah

        GameObject follower = Instantiate(followerPrefab, spawnPos, Quaternion.identity);
        ShipFollower sf = follower.GetComponent<ShipFollower>();
        if (sf != null)
        {
            sf.player = player;
            sf.offset = spawnOffset;
            sf.isEntering = true;
        }

        activeFollowers.Add(follower);
        currentIndex++;
    }

    public void ResetFollowers()
    {
        foreach (var follower in activeFollowers)
        {
            if (follower != null)
            {
                follower.GetComponent<ShipFollower>()?.StartEscape();
            }
        }

        activeFollowers.Clear();
        currentIndex = 0;
        timer = 1f;
        spawningActive = false; // stop dulu sampai player respawn
        player = null; // Reset player untuk memastikan dicari ulang saat Update

    }

    public void StartSpawning(Transform playerTransform)
    {
        player = playerTransform;
        currentIndex = 0;
        timer = 1f;
        spawningActive = true; // aktifkan spawn
    }
}
