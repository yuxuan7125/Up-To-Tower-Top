using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerInputActions inputActions;
    Rigidbody rb;

    bool isGrounded;
    bool jumpRequested;
    bool moveRequested;
    float moveInput;


    [Header("Movement")]
    public Vector3 center = new Vector3(0, 0, 0);
    public float radius = 3;
    public float moveSpeed = 1;
    float curAngle;   // 弧度
    public float jumpForce = 5;
    public float inwardStrength = 0.1f;

    [Header("Ground Detection")]
    public float maxGroundAngle = 45f;

    [Header("Hit Obstacle")]
    public float rayDistance = 0.1f;
    public float rayHeight = 0.1f;



    private void Start()
    {
        Debug.Log("遊戲開始");
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
        moveInput = inputActions.Player.Move.ReadValue<float>();
        if (moveInput!=0)
        {
            moveRequested = true;
        }

        // 按下跳躍鍵，而且目前可以跳躍
        if (inputActions.Player.Jump.WasPressedThisFrame() && isGrounded)
        {
            jumpRequested = true;
        }
    }

    private void FixedUpdate()
    {
        //移動判斷
        if (moveRequested)
        {
            moveRequested = false;

            //修正當前軌道
            rb.linearVelocity = new Vector3((rb.linearVelocity.x - (rb.position.x-center.x)*inwardStrength) , rb.linearVelocity.y, (rb.linearVelocity.z) - (rb.position.z - center.z)*inwardStrength);

            // 計算當前弧度
            float curAngle = Mathf.Atan2((transform.position.z - center.z), (transform.position.x - center.x));
            float nextAngle = curAngle + moveInput * moveSpeed * Time.fixedDeltaTime;

            //判斷是否可以移動
            if (CanMove(nextAngle, moveInput))
            {
                // 計算下一個位置需移動之距離
                float deltaX = radius * (Mathf.Cos(nextAngle) - Mathf.Cos(curAngle));
                float deltaZ = radius * (Mathf.Sin(nextAngle) - Mathf.Sin(curAngle));

                // 移動
                rb.MovePosition(rb.position + new Vector3(deltaX, 0, deltaZ));
            }
        }

        //跳躍判斷
        if(jumpRequested)
        {
            jumpRequested = false;
            isGrounded = false;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
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

        Vector3 direction = new Vector3(
            -Mathf.Sin(nextAngle),
            0,
            Mathf.Cos(nextAngle)
        ) * Mathf.Sign(moveInput);

        float distance = transform.localScale.x / 2 + rayDistance;

        bool hit = Physics.Raycast(origin, direction, distance);

        return !hit;
    }
}
