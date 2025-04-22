using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    public GameObject SpawnAtPosition(Vector2 position)
    {
        if (enemyPrefab == null) return null;
        GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        return enemy;
    }
}
