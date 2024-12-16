using UnityEngine;
using UnityTemplateProjects;

public class CameraOrbit : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed = 5f;
    private Vector3 initialOffset;
    private SimpleCameraController controller;

    void Start()
    {
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
