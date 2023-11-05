using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Gun gun;
    [SerializeField] private Animator animator;

    private Rigidbody rb;
    private InputManager inputManager;
    private PlayerAvatar avatar;
    private Camera mainCamera;
    
    // Animator hashes
    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Forward = Animator.StringToHash("Forward");
    private static readonly int Backward = Animator.StringToHash("Backward");
    private static readonly int Right = Animator.StringToHash("Right");
    private static readonly int Left = Animator.StringToHash("Left");
    private static readonly int Shoot1 = Animator.StringToHash("Shoot");

    private void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        avatar = GetComponent<PlayerAvatar>();
        gun = GetComponentInChildren<Gun>();
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
    }

    private void Update()
    {
        if (avatar.IsDead)
        {
            animator.SetTrigger(Die);
            return;
        }

        Move();
        Shoot();
        Rotate();
    }

    private void Move()
    {
        // Handle player movement using InputManager
        float horizontalInput = inputManager.HorizontalInput;
        float verticalInput = inputManager.VerticalInput;
        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput);
        rb.velocity = movement * avatar.MaxSpeed;
        animator.SetFloat(Speed, rb.velocity.magnitude);

        //get if player is moving backward, forward, left or right
        float angle = Vector3.Angle(transform.forward, rb.velocity);
        float sign = Mathf.Sign(Vector3.Dot(transform.up, Vector3.Cross(transform.forward, rb.velocity)));
        float finalAngle = sign * angle;
        // SET ANIMATOR TRIGGER TO left, right, forward or backward
        if (finalAngle is > 45 and < 135 && rb.velocity.magnitude > 0.1f)
        {
            animator.SetTrigger(Forward);
        }
        else if (finalAngle is < -45 and > -135 && rb.velocity.magnitude > 0.1f)
        {
            animator.SetTrigger(Backward);
        }
        else if (finalAngle > 135 || finalAngle < -135 && rb.velocity.magnitude > 0.1f)
        {
            animator.SetTrigger(Right);
        }
        else if (rb.velocity.magnitude > 0.1f)
        {
            animator.SetTrigger(Left);
        }
    }

    private void Shoot()
    {
        // Handle player shooting using InputManager
        bool shootInput = inputManager.ShootInput;
        if (shootInput)
        {
            if (gun.TryToFire())
            {
                animator.SetTrigger(Shoot1);
            }
        }
    }

    private void Rotate()
    {
        //Debug.Log("Mouse position: " + Input.mousePosition);
        //distance camera player
        float distance = Vector3.Distance(mainCamera.transform.position, transform.position);
        // Get mouse position               
        Vector3 mousePosition =
            mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance));
        //Debug.Log("Mouse position world: " + mousePosition);
        // mousePosition.z = mousePosition.y;
        // mousePosition.y = 0;
        // Calculate direction
        Vector3 direction = (mousePosition - transform.position).normalized;

        //Debug.Log("Direction: " + direction);
        //Debug.Log("Mouse position: " + mousePosition);
        // Set player's rotation
        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, -angle, 0));
    }
}