using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObjectHandler
{
    public static DestructibleObjectHandler instance;

    static List<GameObject> destructibleObjects = new List<GameObject>();
    
    public static List<GameObject> DestructibleObjects { set { destructibleObjects = value; } get { return destructibleObjects; } }

    public static void InteractedWithObject(DestructibleObject destructibleObject, Rigidbody rigidBody)
    {
        ParticleSystem tempParticleSystem;
        float durability = 0;
        float force = rigidBody.velocity.magnitude;

        if (rigidBody.gameObject.CompareTag("Shootable"))
        {
            durability = destructibleObject.GetObjectProjectileDurability;
        }
        else
        {
             durability = destructibleObject.GetObjectMachineDurability;
        }

        if (durability <= force && destructibleObject.gameObject.activeInHierarchy)
        {
            tempParticleSystem = ParticleSystem.Instantiate(destructibleObject.GetParticleSystem);
            tempParticleSystem.transform.position = destructibleObject.transform.position;
            tempParticleSystem.Play();

            /*Only temporary, might change what happens with the object,
            for example changing the mesh to borken wooden parts or something*/
            destructibleObject.gameObject.SetActive(false);
        }
    }
}
