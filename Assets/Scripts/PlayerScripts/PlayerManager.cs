using System.Collections;
using UnityEngine;
using Enums;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Transform centerOfMassObject; // for auto-balance testing
    [SerializeField] private float gravity;
    private PlayerInput input;
    private Movement movement;
    private Rigidbody rb;
    private CollisionHandler collision;
    private ControlledVibrations vibrations;
    private float maxSpeed = 7;

    private bool isActivated = false;

    #region Initialization
    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        movement = GetComponent<Movement>();
        rb = GetComponentInChildren<Rigidbody>();
        collision = GetComponentInChildren<CollisionHandler>();
        vibrations = GetComponentInChildren<ControlledVibrations>();

        StartCoroutine(BalanceCenterOfMass());
    }

    private void Start()
    {
        if (CameraStateManager.GetPerspective() != CameraPerspective.thirdPerson)
        {
            DeActivate();
        }
        else
        {
            Activate();
        }
        AudioManager.Instance.InitializePlayer(rb.transform);
    }
    #endregion

    public void Activate()
    {
        isActivated = true;
        vibrations.IsActivated = true;
        input.IsActivated = true;
        GameManager.instance.DelayedStart();
    }
    public void DeActivate()
    {
        isActivated = false;
        vibrations.IsActivated = false;
        input.IsActivated = false;
        GameManager.instance.DeActivate();
    }
    void Update()
    {
        if (!isActivated) return;

        float speedValue = Mathf.Clamp01(rb.velocity.magnitude / maxSpeed);

        if (AudioManager.Instance != null)
            AudioManager.Instance.UpdateSpeed(speedValue);

        Ability pendingAbility = input.CheckAbilityInput();
        if (pendingAbility == Ability.jump)
        {
            movement.Jump(rb);
        }
        //centerOfMassObject.position = rb.worldCenterOfMass; - for auto-balance testing
    }
    private void FixedUpdate()
    {
        if (!isActivated) return;

        Vector3 moveDirection = input.GetMoveDirection();

        if (moveDirection.x != 0 && collision.isGrounded)
        {
            movement.Rotate(rb, moveDirection);
        }

        if (moveDirection.y != 0 && collision.isGrounded)
        {
            movement.Move(moveDirection, rb);
        }

        float xAngle = rb.transform.eulerAngles.x > 180 ? rb.transform.eulerAngles.x - 360 : rb.transform.eulerAngles.x;
        if (Mathf.Abs(xAngle) > 75)
        {
            float newValue = Mathf.Clamp(xAngle, -75, 75);
            rb.transform.eulerAngles = new Vector3(newValue, rb.transform.eulerAngles.y, rb.transform.eulerAngles.z);
        }

        if (!collision.isGrounded)
            rb.AddForce(Vector3.down * Time.fixedDeltaTime * gravity);
    }

    private IEnumerator BalanceCenterOfMass()
    {
        // Changes the center of mass depending on the angles/rotation of the player
        while (true)
        {
            float balanceRange = rb.transform.localScale.x * 0.8f;
            if (Input.anyKey) 
            {
                balanceRange *= 0.8f;
            }
            float xAngle = rb.transform.eulerAngles.x > 180 ? rb.transform.eulerAngles.x - 360 : rb.transform.eulerAngles.x;
            float zAngle = rb.transform.eulerAngles.z > 180 ? rb.transform.eulerAngles.z - 360 : rb.transform.eulerAngles.z;

            float zValue = xAngle > 0 ? Mathf.Lerp(0, -balanceRange, xAngle / 50) :
                           xAngle < 0 ? Mathf.Lerp(0, balanceRange, xAngle / -50) : 0;

            float xValue = zAngle > 0 ? Mathf.Lerp(0, balanceRange, zAngle / 50) :
                           zAngle < 0 ? Mathf.Lerp(0, -balanceRange, zAngle / -50) : 0;

            rb.centerOfMass = new Vector3(xValue, -balanceRange * 0.8f, zValue);

            yield return new WaitForEndOfFrame();
        }
    }

    #region Third person
    public void OnCameraSwap(CameraPerspective cameraPerspective)
    {
        rb.constraints = cameraPerspective == CameraPerspective.thirdPerson ?
             RigidbodyConstraints.None : RigidbodyConstraints.FreezeRotationX;

    }
    #endregion
}
