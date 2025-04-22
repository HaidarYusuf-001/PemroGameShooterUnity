using UnityEngine;

public class FollowPlayerInCircle : MonoBehaviour
{
    public Transform target; // Player
    private Vector3 offset;  // Jarak tetap ke player

    public void Initialize(Transform player)
    {
        target = player;
        offset = transform.position - player.position;
    }

    void Update()
    {
        if (target == null) return;
        transform.position = target.position + offset;
    }
}
