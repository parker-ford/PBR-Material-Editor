using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public float orbitSpeed = 10f;
    public Vector3 orbitCenter = Vector3.zero;

    private void Update()
    {
        float angle = orbitSpeed * Time.deltaTime;
        Vector3 offset = transform.position - orbitCenter;
        Quaternion rotation = Quaternion.Euler(0, angle, 0);

        transform.position = orbitCenter + rotation * offset;

        transform.LookAt(orbitCenter);
    }
}
