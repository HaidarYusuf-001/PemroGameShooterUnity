using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    public Vector2 targetPosition;
    public float moveSpeed = 2f;

    private void Start()
    {
        StartCoroutine(MoveToTarget());
    }

    IEnumerator MoveToTarget()
    {
        while (Vector2.Distance(transform.position, targetPosition) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;

        // Matikan gerakan enemy setelah sampai
        Enemy enemy = GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.isActiveMovement = false;
        }

        Destroy(this); // Hapus script EnemyMovement
    }

}
