using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;

    [Header("Jump")]
    public float jumpForce = 7f;
    public bool isGrounded;

    [Header("Mouse Look")]
    public Transform cameraPivot;
    public float mouseSensitivity = 2f;
    public float maxPitch = 80f;

    [Header("Character Variables")]
    public int maxHP = 100;
    public int hp = 100;

    private Rigidbody rb;
    private float yaw;
    private float pitch;
    private Vector3 moveInput;
    private float currentSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        hp = maxHP;
        currentSpeed = walkSpeed;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mx = Input.GetAxis("Mouse X") * mouseSensitivity;
        float my = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mx;
        pitch -= my;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(x, 0f, z).normalized;

        if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed = runSpeed;
        else
            currentSpeed = walkSpeed;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Vector3 velocity = rb.linearVelocity;
            velocity.y = jumpForce;
            rb.linearVelocity = velocity;
            isGrounded = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void FixedUpdate()
    {
        Vector3 moveWorld = transform.TransformDirection(moveInput) * currentSpeed;
        Vector3 newVelocity = new Vector3(moveWorld.x, rb.linearVelocity.y, moveWorld.z);
        rb.linearVelocity = newVelocity;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    public void TakeDamage(int amount)
    {
        hp -= amount;

        if (hp <= 0)
        {
            hp = 0;
            RestartGame();
        }
    }

    void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}