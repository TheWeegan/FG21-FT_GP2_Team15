using UnityEngine;
using UnityEngine.UI;

public class MessCountScript : MonoBehaviour
{
    [SerializeField] int scoreAmount = 1;
    [SerializeField] [Range(0.1f, 10f)] float forceThreshold;
    
    float messScore = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out HasCollidedScript hasCollidedScript))
        {
            if (!hasCollidedScript.HasCollided)
            {
                float objectForce = collision.rigidbody.velocity.magnitude;
                if(objectForce >= forceThreshold)
                {
                    messScore += scoreAmount;

                    GameManager.instance.MessScore(messScore);
                }
                hasCollidedScript.HasCollided = true;
            }
        }
    }
}
