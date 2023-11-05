using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float HorizontalInput { get; private set; }
    public float VerticalInput { get; private set; }
    public bool ShootInput { get; private set; }

    private void Update()
    {
        // Read input values
        HorizontalInput = Input.GetAxis("Horizontal");
        VerticalInput = Input.GetAxis("Vertical");
        ShootInput = Input.GetButton("Fire1");
    }
}