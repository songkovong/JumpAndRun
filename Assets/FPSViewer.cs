using TMPro;
using UnityEngine;

public class FPSViewer : MonoBehaviour
{
    public TMP_Text frameText;

    float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        frameText.text = $"FPS: {Mathf.Ceil(fps)}";
    }
}
