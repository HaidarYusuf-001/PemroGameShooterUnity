using UnityEngine;

public class Explosion : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 0.25f); // Hapus diri sendiri setelah 0.5 detik
    }
}
