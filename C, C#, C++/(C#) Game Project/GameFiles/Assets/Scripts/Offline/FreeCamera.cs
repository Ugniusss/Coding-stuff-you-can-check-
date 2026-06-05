using UnityEngine;

public class FreeCamera : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float edgeSize = 10f;
    public Vector2 xLimits;
    public Vector2 zLimits;

    void Update()
    {
        Vector3 pos = transform.position;

        if (Input.GetKey(KeyCode.D)) pos.z += moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.A)) pos.z -= moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.W)) pos.x -= moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S)) pos.x += moveSpeed * Time.deltaTime;

        pos.x = Mathf.Clamp(pos.x, xLimits.x, xLimits.y);
        pos.z = Mathf.Clamp(pos.z, zLimits.x, zLimits.y);

        transform.position = pos;
    }
}
