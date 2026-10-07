using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private float moveSpeed;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        bool isMoving = moveInput.magnitude > 0.1f;

        if (anim != null)
        {
            anim.SetBool("IsRunning", isMoving);
        }

        if (isMoving)
        {
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg - 90f;

            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public void Initialize(float speedFromData)
    {
        moveSpeed = speedFromData;
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        moveInput.Normalize();
    }

    void FixedUpdate()
    {
        // Movemos al personaje
        rb.linearVelocity = moveInput * moveSpeed;
    }
}
