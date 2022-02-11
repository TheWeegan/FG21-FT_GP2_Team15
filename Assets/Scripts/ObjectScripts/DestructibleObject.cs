using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DestructibleObject : MonoBehaviour
{
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] float objectMachineDurability;
    [SerializeField] float objectProjectileDurability;

    public static Action objectDestroyedEvent;  

    public float GetObjectMachineDurability { get { return objectMachineDurability; } }
    public float GetObjectProjectileDurability { get { return objectProjectileDurability; } }
    public ParticleSystem GetParticleSystem { get { return particleSystem; } }

    private void OnEnable()
    {
        DestructibleObjectHandler.DestructibleObjects.Add(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Shootable"))
        {
            Rigidbody rigidBody = collision.gameObject.GetComponent<Rigidbody>();
            DestructibleObjectHandler.InteractedWithObject(this, rigidBody);
            objectDestroyedEvent?.Invoke();
        }
    }
}
