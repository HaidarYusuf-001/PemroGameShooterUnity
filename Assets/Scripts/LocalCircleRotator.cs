using UnityEngine;

public class LocalCircleRotator : MonoBehaviour
{
    public Vector3 centerPoint;
    public float radius = 3f;
    public float angularSpeed = 30f; // derajat per detik
    private float angle;

    void Start()
    {
        Vector3 offset = transform.position - centerPoint;
        angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
    }

    void Update()
    {
        angle += angularSpeed * Time.deltaTime;
        float rad = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;
        transform.position = centerPoint + offset;
    }
}
