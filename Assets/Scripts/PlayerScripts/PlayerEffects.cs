using System.Collections;
using UnityEngine;

namespace PlayerScripts
{
    public class PlayerEffects : MonoBehaviour
    {
        [SerializeField] private GameObject mouseButtonLeftEffect;
        [SerializeField] private GameObject mouseButtonRightEffect;
        [SerializeField] private GameObject warningParticlesObject;
        [SerializeField] private GameObject fireEffect;
        [SerializeField] private ParticleSystem victoryEffect;

        [SerializeField] [Range(0,3)] private float warningParticlesHeight = 1;
        [SerializeField] [Range(0,3)] private float mouseHintHeight = 2;
        [SerializeField] [Range(1, 5)] private float hintTimerRight = 3;
        [SerializeField] [Range(1, 5)] private float hintTimerLeft = 3;
        private Transform washingMachineTransform;

        private bool disableMouseRightHint;
        private bool disableMouseLeftHint;
        private bool shootablePickup;
        private bool gameStarted;
        private void Awake()
        {
            washingMachineTransform = transform.Find("Washing machine");
        }
        private void Start()
        {
            DestructibleObject.objectDestroyedEvent += DisableSpawnMouseRightHint;
            GameManager.instance.failedSuctionEvent += SpawnWarningEffect;
            GameManager.instance.collectibleCollected += DisableMouseLeftHint;
            GameManager.instance.shootableCollected += ShootablePickup;
            GameManager.instance.shootableShot += ShootableShot;
            GameManager.instance.startGame += GameStarted;
            GameManager.instance.victory += Victory;

            StartCoroutine(LeftMouseHintRoutine());
            StartCoroutine(RightMouseHintRoutine());
        }
        private IEnumerator RightMouseHintRoutine()
        {
            yield return new WaitWhile(() => !shootablePickup);
            yield return new WaitForSeconds(hintTimerRight);
            if (!disableMouseRightHint)
            {
                SpawnMouseRightHint();
            }
            DestructibleObject.objectDestroyedEvent -= DisableSpawnMouseRightHint;
        }
        private IEnumerator LeftMouseHintRoutine()
        {
            yield return new WaitWhile(() => !gameStarted);
            yield return new WaitForSeconds(hintTimerLeft);
            if (!disableMouseLeftHint)
            {
                SpawnMouseLeftHint();
            }
            GameManager.instance.collectibleCollected -= DisableMouseLeftHint;

        }
        private void DisableMouseLeftHint()
        {
            disableMouseLeftHint = true;
        }

        public void SpawnWarningEffect()
        {
            GameObject warningObject = Instantiate(warningParticlesObject, 
                washingMachineTransform.position + washingMachineTransform.transform.up *warningParticlesHeight, Quaternion.identity);
            warningObject.GetComponent<ParticleSystem>().Play();
            
            
        }
        private void SpawnMouseLeftHint() 
        {
            GameObject leftHint = Instantiate(mouseButtonLeftEffect, 
                washingMachineTransform.position + washingMachineTransform.transform.up *mouseHintHeight, Quaternion.identity);
            leftHint.GetComponent<ParticleSystem>().Play();
            AudioManager.PlayNotificationSound();
        }
    
        private void SpawnMouseRightHint()
        {
            if (!disableMouseRightHint)
            {
                GameObject rightHint = Instantiate(mouseButtonRightEffect, 
                    washingMachineTransform.position + washingMachineTransform.transform.up *mouseHintHeight, Quaternion.identity);
                rightHint.GetComponent<ParticleSystem>().Play();
                AudioManager.PlayNotificationSound();
            }
        }
        private void Victory() 
        {
            ParticleSystem victory = Instantiate(victoryEffect, washingMachineTransform.transform);
            victory.GetComponent<ParticleSystem>().Play();
        }
        public void DisableSpawnMouseRightHint()
        {
            disableMouseRightHint = true;
        }
        private void ShootablePickup()
        {
            shootablePickup = true;
            AudioManager.Instance.PlayAbsorbSound(transform.position); 
        }
        private void GameStarted()
        {
            gameStarted = true;
        }
        private void ShootableShot()
        {
            AudioManager.Instance.PlayShootingSound(transform.position); 
            Instantiate(fireEffect,  washingMachineTransform.position + washingMachineTransform.transform.forward, Quaternion.identity);
        }
        private void OnDisable()
        {
            GameManager.instance.failedSuctionEvent -= SpawnWarningEffect;
        }
    }
}
