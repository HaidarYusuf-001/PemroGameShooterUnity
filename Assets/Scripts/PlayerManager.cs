using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform spawnPoint;
    public float respawnDelay = 1f;

    [Header("Ship Follower")]
    public ShipFollowerSpawner followerSpawner;

    private GameObject currentPlayer;
    private float respawnTimer;

    void Start()
    {
        RespawnPlayer();
    }

    void Update()
    {
        if (currentPlayer == null)
        {
            respawnTimer -= Time.deltaTime;
            if (respawnTimer <= 0f)
            {
                RespawnPlayer();
            }
        }
    }

    public void PlayerDied()
    {
        respawnTimer = respawnDelay;
        followerSpawner?.ResetFollowers();
    }

    public void PlayerOutOfBounds(GameObject playerGO)
    {
        if (currentPlayer == playerGO)
        {
            Destroy(currentPlayer);
            currentPlayer = null;
            PlayerDied();
        }
    }

    void RespawnPlayer()
    {
        currentPlayer = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);

        var controller = currentPlayer.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.ActivateInvincibility();
        }

        followerSpawner?.StartSpawning(currentPlayer.transform);
    }
}
