using UnityEngine;

public class CrosshairFollowMouse : MonoBehaviour
{
    RectTransform crosshairRect;

    void Start()
    {
        crosshairRect = GetComponent<RectTransform>();
        Cursor.visible = false; // Sembunyikan kursor default
    }

    void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        crosshairRect.position = mousePosition;
    }
}
