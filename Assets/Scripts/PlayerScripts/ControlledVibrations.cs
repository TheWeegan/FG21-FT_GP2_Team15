using System.Collections;
using UnityEngine;

public class ControlledVibrations : MonoBehaviour
{
    [Header("STRENGTH")]
    [SerializeField] [Range(0,2)] float strengthMultiplier;

    [SerializeField] private float interval = 0.1f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform trans;
    [SerializeField] private float xForce;
    [SerializeField] private float zForce;
    [SerializeField] [Range(1, 2.5f)] private float maxLaundryMultiplication = 1.8f;

    private bool isActivated = false;
    private float laundryMultiplier = 1;

    public bool IsActivated { get => isActivated; set => isActivated = value; }

    private void Awake()
    {
        StartCoroutine(ApplyForces());
    }

    private IEnumerator ApplyForces()
    {
        yield return new WaitForSeconds(0.5f);
        int xCount =0, zCount = 0;
        while(true)
        {
            yield return new WaitForFixedUpdate();
            ApplyXForce(xCount++);
            yield return new WaitForSeconds(interval * 0.5f);
            yield return new WaitForFixedUpdate();
            ApplyZForce(zCount++);
            yield return new WaitForSeconds(interval * 0.5f);
        }
    }
    private void ApplyXForce(int count)
    {
        if (!isActivated) return;
        int directionMultiplier = count % 2 == 0 ? 1 : -1;
        rb.AddTorque(trans.right * directionMultiplier * xForce * strengthMultiplier * laundryMultiplier, ForceMode.Force);
    }
    private void ApplyZForce(int count)
    {
        if (!isActivated) return;
        int directionMultiplier = count % 2 == 0 ? 1 : -1;
        rb.AddTorque(trans.forward * directionMultiplier * zForce * strengthMultiplier * laundryMultiplier, ForceMode.Force);
    }

    public void UpdateLaundryMultiplier(int eatenObjectCount, int maxLimit)
    {
        float f = eatenObjectCount / maxLimit;
        laundryMultiplier = Mathf.Lerp(1, maxLaundryMultiplication, f);
    }
   
}
