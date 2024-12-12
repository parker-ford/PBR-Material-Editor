using UnityEngine;
using UnityTemplateProjects;

public class CameraOrbit : MonoBehaviour
{
    // public float orbitSpeed = 10f;
    // public Vector3 orbitCenter = Vector3.zero;

    // private void Update()
    // {
    //     float angle = orbitSpeed * Time.deltaTime;
    //     Vector3 offset = transform.position - orbitCenter;
    //     Quaternion rotation = Quaternion.Euler(0, angle, 0);

    //     if (Input.GetKey(KeyCode.LeftArrow))
    //     {
    //         transform.position = orbitCenter + rotation * offset;

    //         transform.LookAt(orbitCenter);
    //     }
    //     if (Input.GetKey(KeyCode.RightArrow))
    //     {
    //         transform.position = orbitCenter - rotation * offset;

    //         transform.LookAt(orbitCenter);
    //     }

    // }

    public Transform target; // Reference to the sphere
    public float rotationSpeed = 5f;
    private Vector3 initialOffset;
    private SimpleCameraController controller;

    void Start()
    {
        // Calculate the initial offset from the sphere to the camera
        initialOffset = transform.position - target.position;
        controller = Camera.main.GetComponent<SimpleCameraController>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            controller.enabled = false;
            transform.RotateAround(target.position, Vector3.up, rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            controller.enabled = false;
            transform.RotateAround(target.position, Vector3.up, -rotationSpeed * Time.deltaTime);
        }

        Vector3 newPosition = target.position + Quaternion.Euler(0, transform.eulerAngles.y, 0) * initialOffset;
        transform.position = newPosition;
        controller.enabled = true;
    }
}
