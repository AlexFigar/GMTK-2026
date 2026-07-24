using UnityEngine;

public class CharController : MonoBehaviour
{
    public float movementSpeed = 5.0f;
    public float jumpForce = 5.0f;

    //References
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    private Vector2 movement;
    private bool jumped;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocityX = movement.x * movementSpeed;

        if (jumped)
        {
            rb.linearVelocityY = jumpForce;
            jumped = false;
        }
    }

    public void Move(Vector2 moveVector)
    {
        movement = moveVector;
    }

        public void Jump()
    {
        jumped = true;
    }
}