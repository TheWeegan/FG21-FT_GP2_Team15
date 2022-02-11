using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerScripts
{
    public class FireProjectile : MonoBehaviour
    {
        private readonly List<GameObject> shootableObjects = new List<GameObject>();
        private Transform fireDirection;
        private Action<int, int> magazineSizeChangeEvent;
        
        private int currentMagazineSize;
        private float startDistance = 0.05f; //distance from the object spawner to avoid player collision.
        private float absorbingShootableDelay;

        [Header("Firepower")] [Range(1f, 15f)] public float firePower = 20;
        [Header("Recoil")] [Range(0, 5f)] public float startRecoil = 3f;
        [Tooltip("Adds additional recoil for every extra item currently in magazine.")]
        public float multiShotRecoil = 1;

        [Header("Magazine size")]
        [Tooltip("Sets how many shootable objects that can be stored before blocking suction.")]
        public int maxMagazineSize = 5;

        private void Start()
        {
            fireDirection = GameObject.Find("Hole")?.GetComponent<Transform>();
            magazineSizeChangeEvent += transform.root.GetComponent<ControlledVibrations>().UpdateLaundryMultiplier;
            magazineSizeChangeEvent += transform.root.GetComponent<Movement>().UpdateLaundryMultiplier;
        }
        void Update()
        {
            absorbingShootableDelay += Time.deltaTime; //slight delay to avoid absorbing same thing twice.

            if (Input.GetKeyDown(KeyCode.Mouse1) && !PlayerEatingMode.IsEating)
            {
                FireListedObject();
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Shootable") && PlayerEatingMode.IsEating)
            {
                if (currentMagazineSize < maxMagazineSize && absorbingShootableDelay >= 0.05f)
                {
                    currentMagazineSize++;
                    shootableObjects.Add(collision.gameObject);
                    collision.gameObject.SetActive(false);
                    absorbingShootableDelay = 0;

                    GameManager.instance.ShootableCollected();
                    magazineSizeChangeEvent?.Invoke(currentMagazineSize, maxMagazineSize);
                }

                if (currentMagazineSize >= maxMagazineSize)
                {
                    GameManager.instance.BlockSuction();
                }
            }
        }

        private void FireListedObject()
        {
            float tempRecoil = startRecoil;

            for (int i = shootableObjects.Count - 1; i >= 0; i--) //fires all objects stored in 
            {
                shootableObjects[i].SetActive(true);
                shootableObjects[i].transform.position = fireDirection.position + fireDirection.forward * startDistance;
                shootableObjects[i].gameObject.GetComponent<Rigidbody>().AddForce(fireDirection.transform.forward
                    * firePower, ForceMode.VelocityChange);
                tempRecoil += multiShotRecoil;
            }

            if (currentMagazineSize > 0)
            {
                gameObject.GetComponent<Rigidbody>()
                    .AddForce(transform.forward * -tempRecoil, ForceMode.VelocityChange); //adds recoil after shooting.
                GameManager.instance.ShootableShot();
            }
            
            shootableObjects.Clear();
            currentMagazineSize = 0;
            magazineSizeChangeEvent?.Invoke(currentMagazineSize, maxMagazineSize);
        }
    }
}


