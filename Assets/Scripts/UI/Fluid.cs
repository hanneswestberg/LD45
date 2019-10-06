using UnityEngine;
using UnityEngine.UI;

public class Fluid : MonoBehaviour
{
    public string textureName = "_MainTex";
    public float speed = 0.5f;
    public Vector2 wave_shift;

    Vector2 uvOffset = Vector2.zero;

    private Image img;

    private void Start()
    {
        img = GetComponent<Image>();
    }

    void LateUpdate()
    {
        uvOffset.x = Mathf.Sin(Time.time * speed + wave_shift.x);
        uvOffset.y = Mathf.Cos(Time.time * speed + wave_shift.y);
        img.material.SetTextureOffset(textureName, uvOffset);
    }
}
