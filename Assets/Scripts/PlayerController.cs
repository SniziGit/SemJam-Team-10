using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject player;
    public enum PlayerType { Aster, Roy }
    public PlayerType playerType;

    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 10f;// Dani the characters should not jump btw
    [SerializeField] bool canJump = true;


    private Rigidbody2D rb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = this.gameObject; // Assign the current GameObject to the player variable
        rb = player.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerType == PlayerType.Aster)
        {
            HandleAsterControls();
        }
        else if (playerType == PlayerType.Roy)
        {
            HandleRoyControls();
        }
    }

    //Aster controls
    private void HandleAsterControls()
    {
        //Aster controls
        if (Input.GetKey(KeyCode.W) && canJump)
        {
            rb.AddForce(Vector2.up * jumpForce);
            canJump = false; // Disable jumping until the player lands again
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(Vector2.left * moveSpeed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(Vector2.right * moveSpeed);
        }
    }

    //Roy controls
    public void HandleRoyControls()
    {
        //Roy controls
        if (Input.GetKey(KeyCode.UpArrow) && canJump)
        {
            rb.AddForce(Vector2.up * jumpForce);
            canJump = false; // Disable jumping until the player lands again
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rb.AddForce(Vector2.left * moveSpeed);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rb.AddForce(Vector2.right * moveSpeed);
        }
    }
}
