using UnityEngine;

public class ShieldController : MonoBehaviour
{
    public SpriteRenderer shieldRenderer;
    public GameObject shieldVisual;
    public int maxHits = 3;
    private int hitsLeft;

    public Color greenShield = new Color(0f, 0.3747902f, 1f, 1f); // Neon purple
    public Color yellowShield = new Color(1.0f, 0.92f, 0.016f, 1f); // gold-ish yellow
    public Color redShield = new Color(1.0f, 0.1f, 0.1f, 1f);    // full red


    void Start()
    {
        ResetShield();
    }

    public void ResetShield()
    {
        hitsLeft = maxHits;
        UpdateShieldVisual();
        if (shieldVisual != null) shieldVisual.SetActive(true);
    }

    public bool AbsorbHit()
    {
        hitsLeft--;

        if (hitsLeft > 0)
        {
            UpdateShieldVisual();
            return true; // shield masih aktif
        }
        else if (hitsLeft == 0)
        {
            UpdateShieldVisual(); // update warna sebelum hancur total (optional)
            if (shieldVisual != null) shieldVisual.SetActive(false);
            return true; // shield menahan serangan keempat, tapi langsung hancur setelah ini
        }

        return false; // shield udah hancur, gak bisa tahan lagi
    }


    private void UpdateShieldVisual()
    {
        if (shieldRenderer == null) return;

        switch (hitsLeft)
        {
            case 3:
                shieldRenderer.color = greenShield;
                break;
            case 2:
                shieldRenderer.color = yellowShield;
                break;
            case 1:
                shieldRenderer.color = redShield;
                break;
            case 0:
                shieldRenderer.color = new Color(1f, 1f, 1f, 0f); // transparan / tidak terlihat
                break;
        }
    }
}
