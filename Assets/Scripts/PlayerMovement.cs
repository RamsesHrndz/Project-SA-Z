using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 1.8f;
    [SerializeField] private float run_speed = 4.5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastMoveDir;
    private Animator animator;
    private bool movementDisabled = false;
    private bool isRunning = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (movementDisabled)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        isRunning = Keyboard.current != null && Keyboard.current.upArrowKey.isPressed;
        rb.linearVelocity = moveInput.normalized * (isRunning ? run_speed : speed);

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

    public void OnInteract(InputAction.CallbackContext context)
    {
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

        Debug.Log("No hay objeto interactuable disponible");
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
