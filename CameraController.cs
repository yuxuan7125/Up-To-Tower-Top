using System.Drawing;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerController playerController;
    public float radius = 6f;
    Vector3 center;

    private void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        center = playerController.center;
    }

    private void Update()
    {

        float curAngle = playerController.curAngle;
        float X = radius * Mathf.Cos(curAngle);
        float Z = radius * Mathf.Sin(curAngle);
        transform.position = center + new Vector3(X, transform.position.y, Z);
        transform.LookAt(new Vector3(center.x, transform.position.y, center.z));
        
    }
}
