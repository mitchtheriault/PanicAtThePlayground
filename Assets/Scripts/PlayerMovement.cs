using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float rotationSpeed = 720f;

    private CharacterController controller;
    private Animator animator;

    

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Get input from player
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // Movement direction
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical);

        // Determine speed
        float currentSpeed = (isRunning ? runSpeed : walkSpeed) * moveDirection.magnitude;

        // Move the character
        controller.SimpleMove(moveDirection.normalized * currentSpeed);

        // Rotate the character to face movement direction
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Update Animator parameters
        animator.SetFloat("Speed", moveDirection.magnitude);
        animator.SetBool("IsRunning", isRunning);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Punch();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Punch();
        }


    }

    private void Punch()
    {
        animator.SetTrigger("Punch");
    }

}