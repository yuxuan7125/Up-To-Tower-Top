using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerInputActions inputActions;
    Rigidbody rb;

    bool isGrounded;

    [Header("Movement")]
    public Vector3 center = new Vector3(0, 0, 0);
    public float radius = 3;
    public float moveSpeed = 1;
    public float startAngle = -90;   // 角度
    float curAngle;   // 弧度
    public float jumpForce = 5;

    [Header("Ground Detection")]
    public float maxGroundAngle = 45f;

    [Header("Hit Obstacle")]
    public float rayDistance = 0.1f;
    public float rayHeight = 0.1f;



    private void Start()
    {
        Debug.Log("遊戲開始");
        curAngle = startAngle * Mathf.PI / 180f;
    }
    private void Awake()
    {
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Update()
    {
        // 讀取左右移動輸入
        float moveInput = inputActions.Player.Move.ReadValue<float>();

        if (moveInput != 0)
        {
            // 更新圓周角度
            float nextAngle = curAngle + moveInput * moveSpeed * Time.deltaTime;

            if (CanMove(nextAngle, moveInput)) { curAngle = nextAngle; }

            // 計算下一個圓周位置
            float x = center.x + radius * Mathf.Cos(curAngle);
            float z = center.z + radius * Mathf.Sin(curAngle);

            // 移動 Rigidbody
            rb.MovePosition(new Vector3(x, rb.position.y, z));
        }

        // 按下跳躍鍵，而且目前可以跳躍
        if (inputActions.Player.Jump.WasPressedThisFrame() && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach(ContactPoint contact in collision.contacts)
        {
            float angle = Vector3.Angle(contact.normal, Vector3.up);
            if (angle <= maxGroundAngle) isGrounded = true;
        }
    }
        
    private void OnCollisionExit(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            float angle = Vector3.Angle(contact.normal, Vector3.up);
            if (angle <= maxGroundAngle) isGrounded = false;
        }
    }

    bool CanMove(float nextAngle, float moveInput)
    {
        Vector3 origin = transform.position + Vector3.up * rayHeight;
        Vector3 direction = new Vector3(-Mathf.Sin(nextAngle), 0, Mathf.Cos(nextAngle)) * Mathf.Sign(moveInput);
        float distance = transform.localScale.x / 2 + rayDistance;
        return !Physics.Raycast(origin, direction, distance);
    }
}
