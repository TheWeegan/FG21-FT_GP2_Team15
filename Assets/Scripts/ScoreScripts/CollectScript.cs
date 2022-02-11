using PlayerScripts;
using UnityEngine;

namespace ScoreScripts
{
    [ExecuteAlways]
    public class CollectScript : MonoBehaviour
    {
        private bool collected;
    
        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player") && PlayerEatingMode.IsEating)
            {
                if (collected == false) 
                {
                    GameManager.instance.CollectiblesCollect();
                    AudioManager.Instance.PlaySuccessSound();
                    collected = true;
                    Destroy(gameObject,0.1f);
                }
            }
        }


        private void OnEnable()
        { 
            CollectibleManager.AllTheCollectibles.Add(this);
        }
        private void OnDisable()
        {
            CollectibleManager.AllTheCollectibles.Remove(this);
        }
    }
}
