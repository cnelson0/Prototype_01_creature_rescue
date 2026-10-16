using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    public static FirstPersonController Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform carryPoint;
    [Tooltip("Where the player respawns. If empty, uses the starting position.")]
    [SerializeField] private Transform respawnPoint;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float gravity = -20f;

    [Header("Death")]
    [SerializeField] private float invulnerableSeconds = 1f;

    ///The creature currently being carried.
    public Creature CarriedCreature { get; private set; }

    /// The creature the player is close enough to pick up.
    public Creature CreatureInReach { get; set; }

    public Transform CarryPoint => carryPoint;

    private CharacterController controller;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private float pitch;
    private float verticalVelocity;
    private float invulnerableUntil;

    private void Awake()
    {
        Instance = this;
        controller = GetComponent<CharacterController>();

        Transform spawn = respawnPoint != null ? respawnPoint : transform;
        spawnPosition = spawn.position;
        spawnRotation = spawn.rotation;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameManager.Instance.StateChanged += OnStateChanged;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.StateChanged -= OnStateChanged;
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing) return;

        Look();
        Move();

        // Main rescue interaction: press E near a creature to pick it up.
        if (SimpleInput.InteractPressed && CarriedCreature == null && CreatureInReach != null)
            CreatureInReach.PickUp(this);
    }

    private void Look()
    {
        Vector2 look = SimpleInput.Look * mouseSensitivity;
        transform.Rotate(0f, look.x, 0f);

        pitch = Mathf.Clamp(pitch - look.y, -80f, 80f);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void Move()
    {
        Vector2 input = SimpleInput.Move;
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        move = Vector3.ClampMagnitude(move, 1f) * moveSpeed;

        if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -1f;
        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }

    // ---------- Carrying ----------

    public void SetCarried(Creature creature)
    {
        CarriedCreature = creature;
        CreatureInReach = null;
    }

    /// Called by the SafeZone when the player arrives with a creature.
    public void DeliverCarried()
    {
        if (CarriedCreature == null) return;
        Creature c = CarriedCreature;
        CarriedCreature = null;
        c.Deliver();
    }

    // ---------- Death and respawn ----------

    ///Called by Hazard scripts.
    public void Die(string reason)
    {
        if (Time.time < invulnerableUntil) return;
        invulnerableUntil = Time.time + invulnerableSeconds;

        // Dropped creature goes back to where it was found.
        if (CarriedCreature != null)
        {
            CarriedCreature.ReturnHome();
            CarriedCreature = null;
        }

        GameManager.Instance.LoseLife(reason);

        if (GameManager.Instance.State == GameState.Playing)
            Respawn();
    }

    private void Respawn()
    {
        controller.enabled = false;               // must be off to set position
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;
        controller.enabled = true;

        verticalVelocity = 0f;
        pitch = 0f;
        cameraTransform.localRotation = Quaternion.identity;
        CreatureInReach = null;
    }

    private void OnStateChanged(GameState state)
    {
        if (state == GameState.Playing) return;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
