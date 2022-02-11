using UnityEngine;

namespace PlayerScripts
{
    public class PlayerRecoil : MonoBehaviour
    {
        [Range(0f, 5f)]
        public float suctionRecoil;
        private Rigidbody rb;


        private void Start()
        {
            GameManager.instance.collectibleCollected += SuctionRecoil; //subscribes to collectables 
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }
        }

        private void SuctionRecoil()
        {
            rb.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * -suctionRecoil, ForceMode.VelocityChange);
        }

        private void OnDestroy()
        {
            GameManager.instance.collectibleCollected -= SuctionRecoil; //subscribes to collectables 
        }
    }
}
