using UnityEngine;

namespace PlayerScripts
{
    public class AnimationController : MonoBehaviour
    {
        private Animator animator;

        private static readonly int eating = Animator.StringToHash("isEating");
        private static readonly int holding = Animator.StringToHash("holdingObject");
        private static readonly int fin = Animator.StringToHash("victoryTrigger");
        private static readonly int firing = Animator.StringToHash("shootingObjectTrigger");
        private static readonly int deActivate = Animator.StringToHash("deActivateTrigger");
        private static readonly int paused = Animator.StringToHash("paused");
        

        void Start()
        {   
            animator = GetComponent<Animator>();

            GameManager.instance.shootableCollected += Holding;
            GameManager.instance.shootableShot += ShotsFiredEvent;
            GameManager.instance.victory += VictoryEvent;
            GameManager.instance.deActivate += InstantIdle;
            GameManager.instance.startGame += Unpause;
        }

        private void Unpause()
        {
            animator.SetBool(paused, false);
        }

        private void Update()
        {
            if (PlayerEatingMode.IsEating)
            {
                animator.SetBool(eating, true);
            }

            if (!PlayerEatingMode.IsEating)
            {
                animator.SetBool(eating, false);
            }
        }


        private void ShotsFiredEvent() //sets shotsfired to true for next fixed update to animate.
        {
            animator.SetTrigger(firing);
            animator.SetBool(holding, false);
        }

        private void VictoryEvent()
        {
            animator.SetTrigger(fin);
        }
        
        private void OnDestroy()
        {
            GameManager.instance.shootableShot -= ShotsFiredEvent;
        }

        private void Holding()
        {
            animator.SetBool(holding, true);
        }

        private void InstantIdle() //used for going back to normal size to avoid warping menu with animations.
        {
            animator.SetTrigger(deActivate);
            animator.SetBool(paused, true);
        }

    }
}
