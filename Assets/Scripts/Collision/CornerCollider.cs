using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the ground collision sound of the player with 
/// a collider in each corner
/// </summary>
public class CornerCollider : MonoBehaviour
{
    float minVelocitySoundLimit = 0.15f;
    float maxVelocitySoundLimit = 4f;

    bool collisionSoundCooldown = false;
    float collisionCooldown = 0.1f;


    private void OnCollisionEnter(Collision collision)
    {
        if (collisionSoundCooldown) return;
        float velocity = Mathf.Abs(collision.relativeVelocity.y);

        if (velocity > minVelocitySoundLimit)
        {
            float t = velocity / maxVelocitySoundLimit;
            float velocityOut = Mathf.Lerp(0.02f, 1, t);

            AudioManager.Instance.PlayCollisionSound(velocityOut, transform.position, true);
            StartCoroutine(StartCollisionCooldown());
        }
    }

    private IEnumerator StartCollisionCooldown()
    {
        collisionSoundCooldown = true;

        yield return new WaitForSeconds(collisionCooldown);
        collisionSoundCooldown = false;
    }
}
