using System.Drawing;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    Rigidbody rb;
    public PlayerController playerController;
    public float rotateSpeed = 1;
    float radius;
    float curAngle;
    Vector3 center;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerController = FindFirstObjectByType<PlayerController>();
        radius = playerController.radius;
        center = playerController.center;
        curAngle = Mathf.Atan2((transform.position.z - center.z), (transform.position.x - center.x));
        transform.position = center + new Vector3(radius * Mathf.Cos(curAngle), transform.position.y, radius * Mathf.Sin(curAngle));
    }

    void FixedUpdate()
    {
        float nextAngle = curAngle + rotateSpeed * Time.fixedDeltaTime;
        float deltaX = radius * (Mathf.Cos(nextAngle) - Mathf.Cos(curAngle));
        float deltaZ = radius * (Mathf.Sin(nextAngle) - Mathf.Sin(curAngle));
        rb.MovePosition(rb.position + new Vector3(deltaX, 0, deltaZ));
        curAngle = nextAngle;

        transform.LookAt(new Vector3(center.x, transform.position.y, center.z));
    }
}
