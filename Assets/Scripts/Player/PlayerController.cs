using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    //Components
    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerStats stats;
    [SerializeField] private WeaponController weapon;
    [SerializeField] private InputReader input;
    [SerializeField] private PlayerUI ui;

    //Movement
    [Header("Movement")]
    private Vector2 movementInput;
    private bool isSprinting = false;
    private float moveSpeedWhileDrawingMod = 0.5f;
    private float facingDirection = 1f; // 1 = right, -1 = left
    private float dodgeTimer = 0;
    public event UnityAction DodgePerformed = delegate { };

    private void Awake()
    {
        //Get all the componenets attached to the player
        rb = GetComponent<Rigidbody>();
        stats = GetComponent<PlayerStats>();
        weapon = GetComponent<WeaponController>();
        ui = GetComponent<PlayerUI>();
        //input = GameManager.Instance.InputReader;

        //Debugging in case components are missing
        if (rb == null) { Debug.LogWarning("PlayerController is missing Rigidbody reference!"); }
        if (stats == null) { Debug.LogWarning("PlayerController is missing PlayerStats reference!"); }
        if (weapon == null) { Debug.LogWarning("PlayerController is missing WeaponController reference!"); }
        if (ui == null) { Debug.LogWarning("PlayerController is missing PlayerUI reference!"); }
        if (input == null) { Debug.LogWarning("PlayerController is missing InputReader reference!"); }
    }

    //Enable the input system
    private void Start()
    {
        input.EnablePlayerActions();
    }

    //Register to input events
    private void OnEnable()
    {
        input.Move += MovePlayer;
        input.Sprint += HandleSprint;
        input.Dodge += Dodge;
    }

    //Unregister from input events
    private void OnDisable()
    {
        input.Move -= MovePlayer;
        input.Sprint -= HandleSprint;
        input.Dodge -= Dodge;
        input.DisablePlayerActions();
    }

    private void OnDestroy()
    {
        input.DisablePlayerActions();
    }

    void Update()
    {
        // Update dodge cooldown
        if (dodgeTimer < stats.DodgeCooldown)
        {
            dodgeTimer += Time.deltaTime;
        }
    }

    private void MovePlayer(Vector2 moveValue)
    {
        movementInput = moveValue;

        //Update the facing direction
        if (movementInput.x != 0)
        {
            facingDirection = Mathf.Sign(movementInput.x);
        }

        // Flip character based on direction
        //if (movementInput.x != 0)
        //{
        //    Vector3 scale = transform.localScale;
        //    scale.x = Mathf.Sign(movementInput.x) * Mathf.Abs(scale.x);
        //    transform.localScale = scale;
        //}
    }

    private void HandleSprint(bool isPressed)
    {
        isSprinting = isPressed;
    }

    private void FixedUpdate()
    {
        //Double the movement speed if sprinting
        float speed = isSprinting ? stats.MovementSpeed * 2 : stats.MovementSpeed;

        // Reduce speed if weapon is drawing
        if (weapon.IsDrawing)
        {
            speed *= moveSpeedWhileDrawingMod;
        }

        // Apply velocity based on movement input
        Vector3 velocity = new Vector3(movementInput.x * speed, rb.linearVelocity.y, rb.linearVelocity.z);
        rb.linearVelocity = velocity;
    }

    private void Dodge()
    {
        //Check that dodge isn't on cooldown
        if (dodgeTimer < stats.DodgeCooldown)
            return;

        //Call the dodge event
        DodgePerformed?.Invoke();

        //Determine which direction to dodge
        float dodgeDirection;
        if (movementInput.x < 0)
            dodgeDirection = -1f;
        else
            dodgeDirection = facingDirection;

        //Dodge and reset the cooldown
        Vector3 force = new Vector3(dodgeDirection * stats.DodgeForce, 0f, 0f);
        rb.AddForce(force, ForceMode.Impulse);
        dodgeTimer = 0;
    }

}
