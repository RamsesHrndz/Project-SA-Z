using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 1.8f;
    [SerializeField] private float run_speed = 4.8f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastMoveDir;
    private Animator animator;
    private bool movementDisabled = false;
    private bool isRunning = false;
    private bool AttackPressed = false;
    private float attackTimer;

    public float cooldown = 0.5f;
    private float timer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (movementDisabled)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (AttackPressed)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (moveInput != Vector2.zero)
        {
            float currentSpeed = isRunning ? run_speed : speed;
            rb.linearVelocity = moveInput.normalized * currentSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void Update()
    {
        if (movementDisabled)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }

        if (AttackPressed)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                AttackFinished();
            }
            return;
        }

        bool isMoving = moveInput != Vector2.zero;
        animator.SetBool("isWalking", isMoving);
        animator.SetBool("isRunning", isMoving && isRunning);
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);

        if (moveInput != Vector2.zero)
            lastMoveDir = moveInput;

        if (context.canceled)
        {
            animator.SetFloat("LastInputX", lastMoveDir.x);
            animator.SetFloat("LastInputY", lastMoveDir.y);
        }
    }

    public void Attack(InputAction.CallbackContext context) {
        if (AttackPressed || timer > 0) return;
        timer = cooldown;
        attackTimer = 0.5f;
        animator.SetBool("isAttack", true);
        animator.SetFloat("LastInputX", lastMoveDir.x);
        animator.SetFloat("LastInputY", lastMoveDir.y);
        animator.SetTrigger("Melee");
        AttackPressed = true;
        rb.linearVelocity = Vector2.zero;
    }

    public void AttackFinished()
    {
        animator.SetBool("isAttack", false);
        AttackPressed = false;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject go in allObjects)
        {
            Interactable interactable = go.GetComponent<Interactable>();
            if (interactable != null && interactable.CanInteract())
            {
                interactable.Interact();
                return;
            }
        }

        Attack(context);
    }

    public void Running(InputAction.CallbackContext context) {
        if (context.performed)
        {
            isRunning = true;
        }
        else if (context.canceled)
        {
            isRunning = false;
        }
    }

    public void DisableMovement()
    {
        movementDisabled = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
    }

    public void EnableMovement()
    {
        movementDisabled = false;
    }
}
