using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Configuracion IA")]
    public bool isAI = false;

    public float moveSpeed;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator anim;
    private bool isAiming = false;
    private Vector2 aimDirection;

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

        if (isAiming)
        {
            if (aimDirection != Vector2.zero)
            {
                float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

        else if (isMoving)
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
        if (isAI) return;
        moveInput = value.Get<Vector2>();
        moveInput.Normalize();
    }
    public void SetMoveInput(Vector2 newMoveInput)
    {
        if (isAI)
        {
            moveInput = newMoveInput;
            moveInput.Normalize();
        }
    }
    void FixedUpdate()
    {
        // Movemos al personaje
        rb.linearVelocity = moveInput * moveSpeed;
    }
    public void SetAimDirection(Vector2 direction)
    {
        isAiming = true;
        if (direction != Vector2.zero)
        {
            aimDirection = direction.normalized;
        }
    }

    public void ClearAimDirection()
    {
        isAiming = false;
    }
}
