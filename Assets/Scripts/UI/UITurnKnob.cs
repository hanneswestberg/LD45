using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITurnKnob : MonoBehaviour
{
    [SerializeField]
    private RectTransform knob;

    private float clampValue = 28;



    public void OnMouseDrag() {

        // Rotate knob towards the cursor
        var cameraPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var dirToCamera = new Vector2(knob.position.x, knob.position.y) -  new Vector2(cameraPos.x, cameraPos.y);
        var angle = (dirToCamera.x < 0) ? 180f - Vector2.Angle(Vector2.down, dirToCamera) : 180f + Vector2.Angle(Vector2.down, dirToCamera);

        knob.localEulerAngles = new Vector3(0f, 0f, Mathf.Clamp(Mathf.RoundToInt(angle/clampValue) * clampValue, 40f, 320f));

        UIManager.instance.UserInputRatioDark = (320f - knob.localEulerAngles.z) / clampValue;
    }
}
