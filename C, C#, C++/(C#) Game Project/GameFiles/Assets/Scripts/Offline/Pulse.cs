using UnityEngine;

public class PulsateEffect : MonoBehaviour
{
    public float pulseSpeed = 2f;
    public float scaleAmount = 0.1f;

    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * scaleAmount;
        transform.localScale = initialScale * scale;
    }
}
