using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject player;
    public enum PlayerType { Aster, Roy }
    public PlayerType playerType;

    [Header("Movement Settings")]
    [SerializeField] float maxSpeed = 8f;
    [SerializeField] float acceleration = 20f;
    [SerializeField] float jumpForce = 12f;
    [SerializeField] float groundFriction = 5f;
    [SerializeField] float airFriction = 0.5f;
    [SerializeField] float fallGravityMultiplier = 2f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private Vector2 moveInput;
    private float defaultGravityScale;

    void Start()
    {
        player = this.gameObject;
        rb = player.GetComponent<Rigidbody2D>();
        defaultGravityScale = rb.gravityScale;
        groundCheckRadius = groundCheck.gameObject.GetComponent<CircleCollider2D>().radius;
    }

    void Update()
    {
        HandleInput();
        CheckGrounded();
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    private void HandleInput()
    {
        moveInput = Vector2.zero;

        if (playerType == PlayerType.Aster)
        {
            if (Input.GetKey(KeyCode.W) && isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            if (Input.GetKey(KeyCode.A)) moveInput.x = -1f;
            if (Input.GetKey(KeyCode.D)) moveInput.x = 1f;
        }
        else if (playerType == PlayerType.Roy)
        {
            if (Input.GetKey(KeyCode.UpArrow) && isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            if (Input.GetKey(KeyCode.LeftArrow)) moveInput.x = -1f;
            if (Input.GetKey(KeyCode.RightArrow)) moveInput.x = 1f;
        }
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void ApplyMovement()
    {
        // Apply acceleration based on input
        if (moveInput.x != 0)
        {
            rb.AddForce(moveInput.x * acceleration * Vector2.right, ForceMode2D.Force);
        }

        // Apply friction
        float currentFriction = isGrounded ? groundFriction : airFriction;
        if (moveInput.x == 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * (1f - currentFriction * Time.fixedDeltaTime), rb.linearVelocity.y);
        }

        // Clamp horizontal speed
        rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocity.x, -maxSpeed, maxSpeed), rb.linearVelocity.y);

        // Apply fall gravity
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = defaultGravityScale * fallGravityMultiplier;
        }
        else
        {
            rb.gravityScale = defaultGravityScale;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
