using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float timeBetweenWaves = 15f;

    private int currentWave = 0;
    private float timer = 0f;
    private Camera cam;

    public float enterSpeed = 2f;
    public float screenPadding = 0.5f;

    void Start()
    {
        cam = Camera.main;
        SpawnNextWave(); // Start langsung wave pertama
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timeBetweenWaves)
        {
            SpawnNextWave();
            timer = 0f;
        }
    }

    void SpawnNextWave()
    {
        currentWave++;
        Debug.Log("Spawning wave: " + currentWave);

        switch (currentWave)
        {
            case 1:
                SpawnLineFormation(10);
                break;
            case 2:
                SpawnDoubleLineFormation(10);
                break;
            case 3:
                SpawnVFormation(15);
                break;
            case 4:
                SpawnCircleAroundPlayer(30);
                break;
            case 5:
                StartCoroutine(SpawnSplitAndCircleFormation());
                break;
            default:
                Debug.Log("All waves completed.");
                break;
        }
    }

    void SpawnLineFormation(int count)
    {
        Vector2 screenMin = cam.ViewportToWorldPoint(new Vector2(0, 1));
        Vector2 screenMax = cam.ViewportToWorldPoint(new Vector2(1, 1));
        float spacing = (screenMax.x - screenMin.x - 2 * screenPadding) / (count - 1);

        for (int i = 0; i < count; i++)
        {
            float x = screenMin.x + screenPadding + i * spacing;
            Vector2 targetPos = new Vector2(x, screenMin.y - 1f);
            Vector2 spawnPos = new Vector2(x, screenMin.y + 3f);

            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            AddMovement(enemy, targetPos);
        }
    }

    void SpawnDoubleLineFormation(int countPerLine)
    {
        Vector2 screenMin = cam.ViewportToWorldPoint(new Vector2(0, 1));
        Vector2 screenMax = cam.ViewportToWorldPoint(new Vector2(1, 1));
        float spacing = (screenMax.x - screenMin.x - 2 * screenPadding) / (countPerLine - 1);

        for (int j = 0; j < 2; j++)
        {
            for (int i = 0; i < countPerLine; i++)
            {
                float x = screenMin.x + screenPadding + i * spacing;
                Vector2 targetPos = new Vector2(x, screenMin.y - 1.5f - j * 1.5f);
                Vector2 spawnPos = new Vector2(x, screenMin.y + 3f);

                GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                AddMovement(enemy, targetPos);
            }
        }
    }

    void SpawnVFormation(int countPerV)
    {
        Vector2 centerTop = cam.ViewportToWorldPoint(new Vector2(0.5f, 1));
        Vector2 centerBottom = cam.ViewportToWorldPoint(new Vector2(0.5f, 0));
        float spacing = 1.2f;

        // V dari atas (ujung ke bawah)
        for (int i = 0; i < countPerV; i++)
        {
            int half = countPerV / 2;
            float offsetX = (i - half) * spacing;
            float offsetY = Mathf.Abs(i - half) * spacing * 0.5f;

            Vector2 targetPos = centerTop + new Vector2(offsetX, -2f - offsetY);
            Vector2 spawnPos = new Vector2(targetPos.x, centerTop.y + 3f);
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            AddMovement(enemy, targetPos);
        }

        // V dari bawah (ujung ke atas)
        for (int i = 0; i < countPerV; i++)
        {
            int half = countPerV / 2;
            float offsetX = (i - half) * spacing;
            float offsetY = Mathf.Abs(i - half) * spacing * 0.5f;

            Vector2 targetPos = centerBottom + new Vector2(offsetX, 2f + offsetY);
            Vector2 spawnPos = new Vector2(targetPos.x, centerBottom.y - 3f);
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            AddMovement(enemy, targetPos);
        }
    }

    void SpawnCircleAroundPlayer(int count)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector2 center = player.transform.position;
        float radius = 5f;
        List<GameObject> enemies = new List<GameObject>();

        // Spawn musuh dalam satu baris di atas layar
        Vector2 screenMin = cam.ViewportToWorldPoint(new Vector2(0, 1));
        Vector2 screenMax = cam.ViewportToWorldPoint(new Vector2(1, 1));
        float spacing = (screenMax.x - screenMin.x - 2 * screenPadding) / (count - 1);

        for (int i = 0; i < count; i++)
        {
            float x = screenMin.x + screenPadding + i * spacing;
            Vector2 spawnPos = new Vector2(x, screenMin.y + 3f);        // luar layar atas
            Vector2 targetPos = new Vector2(x, screenMin.y - 2f);       // masuk ke area tengah bawah
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            AddMovement(enemy, targetPos);
            enemies.Add(enemy);
        }


        StartCoroutine(StartCircleAfterDelay(enemies, player.transform, 2f, radius));
    }

    IEnumerator StartCircleAfterDelay(List<GameObject> enemies, Transform player, float delay, float radius)
    {
        yield return new WaitForSeconds(delay);

        int total = enemies.Count;

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] != null)
            {
                float angle = (360f / enemies.Count) * i;
                StartCoroutine(CirclePlayerRoutine(enemies[i], player, radius, angle));
            }
        }


    }


    IEnumerator SpawnSplitAndCircleFormation()
    {
        Vector2 screenMin = cam.ViewportToWorldPoint(new Vector2(0, 1));
        Vector2 screenMax = cam.ViewportToWorldPoint(new Vector2(1, 1));
        float spacing = (screenMax.x - screenMin.x - 2 * screenPadding) / 9;

        List<GameObject> enemies = new List<GameObject>();

        // Baris 0  lingkaran kecil (radius 2)
        // Baris 1  lingkaran tengah (radius 3)
        // Baris 2  lingkaran besar (radius 4)
        Vector2 center = new Vector2(0, 0); // sementara, nanti dihitung dari rata-rata posisi enemy baris
        List<Vector2> rowCenters = new List<Vector2>() { Vector2.zero, Vector2.zero, Vector2.zero };
        int[] rowCount = new int[3];

        for (int row = 0; row < 3; row++)
        {
            for (int i = 0; i < 10; i++)
            {
                float x = screenMin.x + screenPadding + i * spacing;
                Vector2 spawnPos = new Vector2(x, screenMin.y + 3f);
                Vector2 targetPos = new Vector2(x, screenMin.y - 2f - row * 1.5f);

                GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                AddMovement(enemy, targetPos);
                enemies.Add(enemy);

                rowCenters[row] += targetPos;
                rowCount[row]++;
            }
        }

        // Tunggu musuh masuk ke posisi
        yield return new WaitForSeconds(3f);

        float[] radii = new float[] { 2f, 3f, 4f };
        int index = 0;

        for (int row = 0; row < 3; row++)
        {
            Vector2 ringCenter = rowCenters[row] / rowCount[row];
            float ringRadius = radii[row];

            // Ambil enemy di baris ini (10 per baris)
            for (int i = 0; i < 10; i++)
            {
                if (enemies[index] == null) { index++; continue; }

                GameObject enemy = enemies[index];
                float angle = (360f / 10) * i;
                Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * ringRadius;
                Vector3 targetPos = new Vector3(ringCenter.x, ringCenter.y, 0) + offset;


                // Smooth transisi ke titik lingkaran
                StartCoroutine(TransitionToCircle(enemy, targetPos, ringCenter, ringRadius));
                index++;
            }
        }
    }



    IEnumerator TransitionToCircle(GameObject enemy, Vector3 targetPos, Vector3 center, float radius)
    {
        float duration = 2f;
        float elapsed = 0f;
        Vector3 startPos = enemy.transform.position;

        while (elapsed < duration)
        {
            if (enemy == null) yield break;
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            enemy.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        // Setelah transisi selesai, kasih komponen rotasi lokal
        var rotator = enemy.AddComponent<LocalCircleRotator>();
        rotator.centerPoint = center;
        rotator.radius = radius;
    }



    IEnumerator CirclePlayerRoutine(GameObject enemy, Transform player, float radius = 4f, float angle = 0f)
    {
        float transitionDuration = 2f;

        // Hitung posisi target di lingkaran
        float rad = angle * Mathf.Deg2Rad;
        Vector3 targetOffset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;
        Vector3 targetPos = player.position + targetOffset;

        Vector3 startPos = enemy.transform.position;
        float elapsed = 0f;

        // Transisi smooth ke titik lingkaran
        while (elapsed < transitionDuration)
        {
            if (enemy == null || player == null) yield break;

            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration);
            enemy.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        // Setelah selesai transisi, aktifkan "ikut player" dalam formasi
        FollowPlayerInCircle follower = enemy.AddComponent<FollowPlayerInCircle>();
        follower.Initialize(player);
    }






    void AddMovement(GameObject enemy, Vector2 targetPos)
    {
        EnemyMovement move = enemy.AddComponent<EnemyMovement>();
        move.targetPosition = targetPos;
        move.moveSpeed = enterSpeed;
    }
}
