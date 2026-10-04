using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public enum PlayerType { Aster, Roy }
    public PlayerType playerType;
    public Animator animator;

    [Header("Movement Settings")]
    [SerializeField] float maxSpeed = 8f;
    [SerializeField] float acceleration = 20f;
    [SerializeField] float groundFriction = 5f;
    [SerializeField] float airFriction = 0.5f;
    [SerializeField] float fallGravityMultiplier = 2f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] LayerMask groundLayer;

    [Header("Footstep Settings")]
    [SerializeField] GameObject footstepVFXPrefab;
    [SerializeField] float footstepInterval = 0.4f;
    [SerializeField] Transform footstepSpawnPoint;

    [Header("Death Settings")]
    [SerializeField] GameObject deathVFXPrefab;

    private Rigidbody2D rb;
    private bool isGrounded;
    private Vector2 moveInput;
    private float defaultGravityScale;
    private Vector3 startPos;
    private float footstepTimer;
    private bool wasMoving;
    private bool inputEnabled = true;

    void Start()
    {
        startPos = transform.position;
        rb =GetComponent<Rigidbody2D>();
        defaultGravityScale = rb.gravityScale;
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        HandleInput();
        CheckGrounded();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("PlayerController: OnTriggerEnter called with " + other.name);
        if (other.CompareTag("KillPlane"))
        {
            Debug.Log("Player fell off the map. Resetting position.");
            PlayDeath();
            transform.position = startPos;
            rb.linearVelocity = Vector3.zero;
        }
    }
    void FixedUpdate()
    {
        ApplyMovement();
    }

    private void HandleInput()
    {
        if (!inputEnabled)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = Vector2.zero;

        if (playerType == PlayerType.Aster)
        {
            if (Input.GetKey(KeyCode.A))
            {
                moveInput.x = -1f;
                transform.localScale = new Vector3(-1, 1, 1); // Flip sprite to face left
            }
            if (Input.GetKey(KeyCode.D))
            {
                moveInput.x = 1f;
                transform.localScale = new Vector3(1, 1, 1); // Flip sprite to face right
            }
        }
        else if (playerType == PlayerType.Roy)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                moveInput.x = -1f;
                transform.localScale = new Vector3(-1, 1, 1); // Flip sprite to face left
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                moveInput.x = 1f;
                transform.localScale = new Vector3(1, 1, 1); // Flip sprite to face right
            }
        }
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        animator.SetBool("IsGrounded", isGrounded);
    }

    private void ApplyMovement()
    {
        // Apply acceleration based on input
        bool isMoving = Mathf.Abs(moveInput.x) > 0;
        if (isMoving)
        {
            rb.AddForce(moveInput.x * acceleration * Vector2.right, ForceMode2D.Force);
            animator.SetFloat("Speed", 1);
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }

        // Handle footstep sounds and VFX
        if (isGrounded && isMoving)
        {
            footstepTimer += Time.fixedDeltaTime;
            if (footstepTimer >= footstepInterval)
            {
                footstepTimer = 0f;
                PlayFootstep();
            }
        }
        else
        {
            footstepTimer = footstepInterval;
        }
        wasMoving = isMoving;

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
    public void StopGravity()
    {
        StartCoroutine(StopGravityRoutine());
    }
    private IEnumerator StopGravityRoutine()
    {
       rb.gravityScale = 0f;
        yield return new WaitForSeconds(0.5f);
        rb.gravityScale = defaultGravityScale;
    }

    private void PlayFootstep()
    {
        // Play footstep sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayFootstepSound();
        }

        // Spawn footstep VFX
        if (footstepVFXPrefab != null)
        {
            Vector3 spawnPosition = footstepSpawnPoint != null ? footstepSpawnPoint.position : groundCheck.position;
            Instantiate(footstepVFXPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private void PlayDeath()
    {
        // Play death sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDeathSound();
        }

        // Spawn death VFX at current position before reset
        if (deathVFXPrefab != null)
        {
            Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
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

    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
    }
}
