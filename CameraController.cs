using System.Drawing;
using Unity.AppUI.UI;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerController playerController;
    public float distance = 6f;
    float radius;
    Vector3 center;

    private void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        center = playerController.center;
        radius = playerController.radius + distance;
    }

    private void LateUpdate()
    {
        Vector3 playerPos = playerController.transform.position;

        float curAngle = Mathf.Atan2((playerPos.z - center.z), (playerPos.x - center.x));
        float X = radius * Mathf.Cos(curAngle);
        float Z = radius * Mathf.Sin(curAngle);
        transform.position = center + new Vector3(X, transform.position.y, Z);
        transform.LookAt(new Vector3(center.x, transform.position.y, center.z));
        
    }
}
