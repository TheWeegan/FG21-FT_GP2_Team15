using System.Collections;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    public bool isGrounded;
    private Rigidbody rb;
    
    private float minVelocitySoundLimit = 0.1f;
    private float maxVelocitySoundLimit = 5f;

    private bool collisionSoundCooldown = false;
    private float collisionCooldown = 0.1f;

    private const string eatableTag = "Eatable";
    private const string shootableTag = "Shootable";

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        AddComponentsToRigidbodies();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
            isGrounded = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
            isGrounded = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out DestructibleObject distructableObject))
        {
            DestructibleObjectHandler.InteractedWithObject(distructableObject, rb);
        }
        if (collisionSoundCooldown || collision.collider.CompareTag(eatableTag) || collision.collider.CompareTag(shootableTag)) return;

        if (collision.relativeVelocity.magnitude > minVelocitySoundLimit)
        {
            float t = collision.relativeVelocity.magnitude / maxVelocitySoundLimit;
            float velocityOut = Mathf.Lerp(0, 1, t);

            AudioManager.Instance.PlayCollisionSound(velocityOut, transform.position);
            StartCoroutine(StartCollisionSoundCooldown());
        }
    }
    private IEnumerator StartCollisionSoundCooldown()
    {
        collisionSoundCooldown = true;

        yield return new WaitForSeconds(collisionCooldown);
        collisionSoundCooldown = false;
    }

    private void AddComponentsToRigidbodies()
    {
        Rigidbody[] allRigidBodies = FindObjectsOfType<Rigidbody>();
        int maxScore = 0;

        for (int i = 0; i < allRigidBodies.Length; i++)
        {
            Rigidbody rigidBody = allRigidBodies[i];
            if (rigidBody == rb || rigidBody.CompareTag(eatableTag) || IsChildrensRigidbody(rigidBody))
            {
                continue;
            }
            maxScore += 1;
            allRigidBodies[i].gameObject.AddComponent<HasCollidedScript>();
        }
        GameManager.instance.MaxScore = maxScore;
    }

    private bool IsChildrensRigidbody(Rigidbody rigidBody)
    {
        Rigidbody[] childrenRigidboides = gameObject.GetComponentsInChildren<Rigidbody>();
        for (int j = 0; j < childrenRigidboides.Length; j++)
        {
            if (rigidBody == childrenRigidboides[j])
            {
                return true;
            }
        }
        return false;
    }
}
