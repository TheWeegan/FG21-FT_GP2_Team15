using UnityEngine;

public class SuctionResistance : MonoBehaviour
{   
    [Range(0.5f, 5)]
    public float suctionResistance = 1.5f;
    public Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = suctionResistance;
        }
        
    }
}