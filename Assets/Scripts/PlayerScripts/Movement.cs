using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float jumpForce = 100;
    [SerializeField] private bool enableJump = false;
    [Tooltip("Carrying the max amount of shootable objects will multiply the speed with this value")]
    [SerializeField] [Range(0.2f, 1f)] private float magazineMultiplicationLimit = 0.95f;
    private float laundryMultiplier = 1;
    private bool jumpCooldown = false;

    private CollisionHandler collision;

    private void Awake()
    {
        collision = GetComponentInChildren<CollisionHandler>();
    }


    public void Move(Vector3 direction, Rigidbody rb)
    {
        Vector3 newDirection = rb.transform.forward * direction.y;
        newDirection.y =0;
        
        rb.velocity += newDirection * moveSpeed * Time.fixedDeltaTime * laundryMultiplier;

        float localForwardVelocity = Vector3.Dot(rb.velocity, rb.transform.forward);

        if (direction.y > 0 && localForwardVelocity < 0) 
            rb.velocity += rb.transform.forward;

    }
    public void Rotate(Rigidbody rb, Vector3 moveDirection)
    {
        float direction = moveDirection.x;

        if (moveDirection.y < 0) direction *= -1; // changes rotation direction when going backwards 

        rb.transform.Rotate(rb.transform.up, rotationSpeed * direction * Time.fixedDeltaTime);
    }

    public void Jump(Rigidbody rb)
    {
        if (!jumpCooldown && enableJump)
            StartCoroutine(JumpRoutine(rb));
    }

    private IEnumerator JumpRoutine(Rigidbody rb)
    {
        jumpCooldown = true;
        float startTime = Time.time;
        float timeLimit = 1;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Force);

        yield return new WaitWhile(() => collision.isGrounded && Time.time - startTime < timeLimit);

        rb.angularVelocity = Vector3.zero;

        yield return new WaitWhile(() => !collision.isGrounded);
        // add additional cooldown here
        jumpCooldown = false;
    }
    public void UpdateLaundryMultiplier(int eatenObjectCount, int maxLimit)
    {
        float f = eatenObjectCount / maxLimit;
        laundryMultiplier = Mathf.Lerp(1, magazineMultiplicationLimit, f);
    }
}
