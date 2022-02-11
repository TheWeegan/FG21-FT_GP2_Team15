using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;


public class Settings : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private CinemachineFreeLook cam;
    [SerializeField] private CinemachineVirtualCamera pauseCam;

    [Header("Zooming")]
    [SerializeField] private float maxZoomMultiplier = 2, minZoomMultiplier = 0.6f;
    [SerializeField] [Range(0.1f,1)] private float startZoomValue;
    [SerializeField] [Range(0.1f,1)] private float fireRecoilValue;
    [SerializeField] [Range(0.1f,1)] private float collectZoomValue;
    [SerializeField] [Range(0.1f,1)] private float recoilDelay;
    
    private float currentZoomMultiplier = 0.3f;
    [Header("Sensitivity")]
    [Tooltip("This value multiplied with the start sensitivity value = max sensitivity limit")]
    [SerializeField] [Range(2,5)] float maxSensMultiplier = 3;
    [Tooltip("This value multiplied with the start sensitivity value = min sensitivity limit")]
    [SerializeField] [Range(0.1f,1)] float minSensMultiplier = 0.25f;
    private float maxSpeedX, maxSpeedY, minSpeedX, minSpeedY; // camera sensitivity

    private CinemachineFreeLook.Orbit[] originalOrbits;
    private CinemachineBrain cinemachineBrain;

    private bool fire;
    private bool collect;

    public static Settings Instance; // singleton for now, might replace with ScriptableObject

    private void Awake()
    {
        if (cam == null)
        {
            cam = GameObject.Find("Freelook camera controller").GetComponent<CinemachineFreeLook>();
        }
        if (pauseCam == null)
        {
            pauseCam = GameObject.Find("Pause menu camera").GetComponent<CinemachineVirtualCamera>();
        }
        if (cinemachineBrain == null)
        {
            cinemachineBrain = GameObject.Find("Camera").GetComponentInChildren<CinemachineBrain>();
        }
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    private void Start()
    {
        GameManager.instance.collectibleCollected += CollectZoom;
        GameManager.instance.shootableShot += FireRecoil; 
        
        maxSpeedX = cam.m_XAxis.m_MaxSpeed * maxSensMultiplier;
        maxSpeedY = cam.m_YAxis.m_MaxSpeed * maxSensMultiplier;
        minSpeedX = cam.m_XAxis.m_MaxSpeed * minSensMultiplier;
        minSpeedY = cam.m_YAxis.m_MaxSpeed * minSensMultiplier;


        // this gathers height and radius of Top-, mid- and bottomRig of the camera.
        originalOrbits = new CinemachineFreeLook.Orbit[cam.m_Orbits.Length];
        for (int i = 0; i < cam.m_Orbits.Length; i++)
        {
            originalOrbits[i].m_Height = cam.m_Orbits[i].m_Height;
            originalOrbits[i].m_Radius = cam.m_Orbits[i].m_Radius;
        }


        if (cam.Follow == null)
            cam.Follow = GameObject.Find("Washing machine").transform;

        if (cam.LookAt == null)
            cam.LookAt = GameObject.Find("Washing machine").transform;

        CinemachineCore.GetInputAxis = GetAxisCustom;

        //currentZoomMultiplier = minZoomMultiplier + (maxZoomMultiplier - minZoomMultiplier) / 2;
        currentZoomMultiplier = maxZoomMultiplier * startZoomValue;

        UpdateOrbits();
    }
    


    void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            float change = Input.mouseScrollDelta.y < 0 ? 0.04f : -0.04f;
            currentZoomMultiplier = Mathf.Clamp(currentZoomMultiplier + change, minZoomMultiplier, maxZoomMultiplier);
            UpdateOrbits();
        }
    }

    public int GetScaledMouseSens()
    {
        float percentOfMax = Mathf.InverseLerp(minSpeedX, maxSpeedX, cam.m_XAxis.m_MaxSpeed);
        int zeroToNineScaledSens = Mathf.RoundToInt(Mathf.Lerp(0, 9, percentOfMax));

        return zeroToNineScaledSens;
    }
    public void UpdateCameraSpeed(int zeroToNineSensitivity)
    {
        float percentOfMax = Mathf.InverseLerp(0,9, zeroToNineSensitivity);
        float xSpeed = Mathf.Lerp(minSpeedX, maxSpeedX,  percentOfMax);
        float ySpeed = Mathf.Lerp(minSpeedY, maxSpeedY, percentOfMax);
        cam.m_XAxis.m_MaxSpeed = Mathf.Clamp(xSpeed, minSpeedX, maxSpeedX);
        cam.m_YAxis.m_MaxSpeed = Mathf.Clamp(ySpeed, minSpeedY, maxSpeedY);
    }
    private void UpdateOrbits()
    {
        //StopAllCoroutines();
        for (int i = 0; i < cam.m_Orbits.Length; i++)
        {
            StartCoroutine(LerpOrbit(i));
        }
    }
    private IEnumerator LerpOrbit(int index)
    {
        float startHeight = cam.m_Orbits[index].m_Height;
        float startRadius = cam.m_Orbits[index].m_Radius;
        float startTime = Time.time;
        float duration = 0.25f;

        while (Time.time - startTime < duration)
        {
            float elapsedTime = Time.time - startTime;
            float t = elapsedTime / duration;
            cam.m_Orbits[index].m_Height = Mathf.Lerp(startHeight, originalOrbits[index].m_Height * currentZoomMultiplier, t);
            cam.m_Orbits[index].m_Radius = Mathf.Lerp(startRadius, originalOrbits[index].m_Radius * currentZoomMultiplier, t);
            yield return new WaitForEndOfFrame();
        }
    }

    private float GetAxisCustom(string axisName)
    {
        if (cinemachineBrain.IsBlending)
            return 0;
        return Input.GetAxis(axisName);
    }
    
    //Added recoil and zoom levels when collecting and firing.
    private void CollectZoom()
    {
        if (!collect)
        {
            collect = true;
            currentZoomMultiplier = Mathf.Clamp(currentZoomMultiplier - collectZoomValue, minZoomMultiplier, maxZoomMultiplier);
            UpdateOrbits();
        
            StartCoroutine(ResetZoom());
        }
    }

    private void FireRecoil()
    {
        fire = true;
        currentZoomMultiplier = Mathf.Clamp(currentZoomMultiplier + fireRecoilValue, minZoomMultiplier, maxZoomMultiplier);
        UpdateOrbits();
        
        StartCoroutine(ResetZoom());
    }
    
    private IEnumerator ResetZoom()
    {
        yield return new WaitForSeconds(recoilDelay);
        if (fire)
        {
            currentZoomMultiplier = Mathf.Clamp(currentZoomMultiplier - fireRecoilValue, minZoomMultiplier, maxZoomMultiplier);
            fire = false;
            UpdateOrbits();
        }

        if (collect)
        {
            currentZoomMultiplier = Mathf.Clamp(currentZoomMultiplier + collectZoomValue, minZoomMultiplier, maxZoomMultiplier);
            collect = false;
            UpdateOrbits();
        }
    }

    private void OnDestroy()
    {
        GameManager.instance.collectibleCollected -= CollectZoom;
        GameManager.instance.shootableShot -= FireRecoil;
    }
}
