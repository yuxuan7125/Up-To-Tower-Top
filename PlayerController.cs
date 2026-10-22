using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerInputActions inputActions;
    Rigidbody rb;

    bool isGrounded;
    bool jumpRequested = false;
    bool moveRequested = false;
    float moveInput;
    bool isOffTrack = false;


    [Header("Movement")]
    public Vector3 center = new Vector3(0, 0, 0);
    public float radius = 3;
    public float moveSpeed = 1;
    public float curAngle;   // 弧度
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
        curAngle = Mathf.Atan2((transform.position.z - center.z), (transform.position.x - center.x));
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
        // 移動判斷
        moveInput = inputActions.Player.Move.ReadValue<float>();
        if (moveInput!=0)
        {
            moveRequested = true;
        }

        // 跳躍判斷
        if (inputActions.Player.Jump.WasPressedThisFrame() && isGrounded)
        {
            jumpRequested = true;
        }

        // 偏離軌道判斷
        float dx = transform.position.x - center.x;
        float dz = transform.position.z - center.z;
        float curRadius = Mathf.Sqrt(dx * dx + dz * dz);
        if(curRadius > radius)
        {
            isOffTrack = true;
        }

        // 
        transform.rotation = Quaternion.Euler(transform.rotation.x, -curAngle * Mathf.Rad2Deg, transform.rotation.z);
    }

    private void FixedUpdate()
    {

        // 計算當前弧度
        curAngle = Mathf.Atan2((transform.position.z - center.z), (transform.position.x - center.x));

        //移動請求
        if (moveRequested)
        {
            moveRequested = false;
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

        //跳躍請求
        if(jumpRequested)
        {
            jumpRequested = false;
            isGrounded = false;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }

        //軌道修正
        if (isOffTrack)
        {
            isOffTrack = false;
            rb.MovePosition(rb.position - new Vector3( Mathf.Cos(curAngle), 0, Mathf.Sin(curAngle) ) * Time.fixedDeltaTime);
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
