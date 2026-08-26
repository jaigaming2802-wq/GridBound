using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float gridSize = 1f;

    private Rigidbody2D rb;
    private PlayerInputActions inputActions;

    private Vector2 direction;
    private Vector2 targetposition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        direction = Vector2.zero;

        if (inputActions.Player.MoveUp.WasPressedThisFrame())
        {
            direction = Vector2.up;
        }
        else if (inputActions.Player.MoveDown.WasPressedThisFrame())
        {
            direction = Vector2.down;
        }
        else if (inputActions.Player.MoveLeft.WasPressedThisFrame())
        {
            direction = Vector2.left;
        }
        else if (inputActions.Player.MoveRight.WasPressedThisFrame())
        {
            direction = Vector2.right;
        }

        if (direction == Vector2.zero)
            return;

        targetposition = rb.position + direction * gridSize;

        Collider2D Hit = Physics2D.OverlapBox(
            targetposition,
            new Vector2(0.8f, 0.8f),
            0f
        );

        if (Hit == null)
        {
            rb.MovePosition(targetposition);
            return;
        }

        BoxMovement box = Hit.GetComponent<BoxMovement>();

        if (box == null)
            return;

        RaycastHit2D[] boxHits = Physics2D.RaycastAll(
            box.transform.position,
            direction,
            gridSize
        );

        foreach (RaycastHit2D hitbox in boxHits)
        {
            if (hitbox.collider == null)
                continue;

            if (hitbox.collider.gameObject == box.gameObject)
                continue;

            if (hitbox.collider.CompareTag("Blocks"))
            {
                return;
            }
        }

        box.MoveBox(direction, gridSize);
        rb.MovePosition(targetposition);
    }

    public void ResetPlayer(Vector2 startPosition)
    {
        rb.position = startPosition;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
}