using System;
using System.Collections;
using System.Collections.Generic;
using ScoreScripts;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class hud_controller : MonoBehaviour
{
    //Shootable Icons
    [Header("Shootable Icons")]
    [SerializeField] GameObject[] shootIcon;
    private int currentProjectiles = 0;
    [SerializeField] Sprite[] projectileImages;

    //Collected Shirts Number
    [Header("Shootable Icons")]
    private int maxShirts;
    private int currentShirts = 0;
    [SerializeField] GameObject collectedText;
    [SerializeField] GameObject maxShirtsText;

    //Messy meter
    [Header("Messy Meter")]
    private float maxMessiness;
    private int currentMessiness = 0;
    [SerializeField] GameObject messyMeterObject;
    private RectTransform messyMeter;
    [SerializeField] GameObject messyText;
    [SerializeField] string[] messyTextValues;

    //Stopwatch
    [Header("Stopwatch")]
    [SerializeField] GameObject stopwatchText;
    UIScripts.TimerController timerController;

    //HUD Elements
    [Header("HUD Elements")]
    [SerializeField] GameObject element_HUD;
    [SerializeField] GameObject element_StartScreen;
    [SerializeField] GameObject element_ObjectiveText;
    private bool showHUD = false;
    private bool inStartScreen = false;
    private bool shownObjective = false;
    private float textTimer = 0f;
    private bool canShowText = false;
    [SerializeField] private float textDelay = 2f;
    [SerializeField] private float textVisibleFor = 3f;

    GameManager gameManagerInstance;

    private void Start()
    {
        maxShirts = CollectibleManager.AllTheCollectibles.Count; //sets max to total collects in scene.
        messyMeter = messyMeterObject.GetComponent<RectTransform>();
        messyMeter.localScale = new Vector3(0f, 1f, 1f);

        maxShirtsText.GetComponent<TMPro.TextMeshProUGUI>().text = maxShirts.ToString();
        collectedText.GetComponent<TMPro.TextMeshProUGUI>().text = currentShirts.ToString();
        
        gameManagerInstance = GameManager.instance;
        gameManagerInstance.shootableCollected += UpdateShootablePickup;
        gameManagerInstance.shootableShot += UpdateShootableFired;
        gameManagerInstance.collectibleCollected += UpdateShirtText;
        gameManagerInstance.messScore += UpdateMessyMeter;

        timerController = UIScripts.TimerController.instance;

        //Deactivate HUD & ObjectiveText
        element_HUD.SetActive(false);
        element_ObjectiveText.SetActive(false);
        Invoke("SetStartScreenTrue", 1);
    }
    private void SetStartScreenTrue()
    {
        // quick fix for intro bug when input too early
        inStartScreen = true;
    }

    /*When used in start the value of max score was 0 since this part of the code runs before the part in collisionHandler
      So I'm using a bool to make update work as start, it's not a pretty sollution but it works ;3
      Would be grateful for some tips and tricks on how to do this in a better way*/
    bool doOnce = true; 
    private void Update()
    {
        if (doOnce)
        {
            maxMessiness = gameManagerInstance.MaxScore;
            doOnce = false;
        }
        UpdateStopWatch();

        //Handle Start Screen
        if (inStartScreen)
        {
            //Go To Main Menu
            if (Input.anyKeyDown)
            {
                element_StartScreen.SetActive(false);
                inStartScreen = false;
                CameraStateManager.SwapToCamera(Enums.CameraPerspective.pauseMenu);
            }
        }
        else
        {
            //In Game - Show
            if (shownObjective == false && canShowText == true)
            {
                textTimer += 1f * Time.deltaTime;
                if (textTimer > textDelay)
                {
                    if (element_ObjectiveText.activeSelf == false)
                    {
                        element_ObjectiveText.SetActive(true);
                    }
                    else
                    {
                        if (textTimer > textDelay + textVisibleFor)
                        {
                            element_ObjectiveText.SetActive(false);
                            textTimer = 0f;
                            shownObjective = true;
                        }
                    }
                }
            }
        }

        //Handle HUD when switching to and from Pause Menu
        if (CameraStateManager.GetPerspective() == Enums.CameraPerspective.pauseMenu)
        {
            if (element_HUD.activeSelf == true)
            {
                element_HUD.SetActive(false);
            }
        }
        else if (CameraStateManager.GetPerspective() == Enums.CameraPerspective.thirdPerson)
        {
            if (element_HUD.activeSelf == false)
            {
                element_HUD.SetActive(true);
            }
            if (canShowText == false)
            {
                canShowText = true;
            }
        }
    }

    //Show or Hide HUD - Used when going into pause screen, etc.
    public void ShowHUD(bool visible)
    {
        element_HUD.SetActive(visible);
    }

    //Update Stop Watch
    public void UpdateStopWatch()
    {

        stopwatchText.GetComponent<TMPro.TextMeshProUGUI>().text = timerController.GetTimer.ToString();
    }

    //Update Shirt Text
    public void UpdateShirtText()
    {
        currentShirts++;
        collectedText.GetComponent<TMPro.TextMeshProUGUI>().text = currentShirts.ToString();
        
        if (NoMoreClothes())
        {
            gameManagerInstance.Victory();
        }
    }

    private bool NoMoreClothes()
    {
        if(currentShirts >= maxShirts)
        {
            return true;
        }
        return false;
    }

    //Update Shootables After Pickup
    public void UpdateShootablePickup()
    {
        currentProjectiles++;
        for (var i = 0; i < shootIcon.Length; i++)
        {
            if (i < currentProjectiles)
            {
                shootIcon[i].GetComponent<Image>().sprite = projectileImages[1];
            }
            else
            {
                shootIcon[i].GetComponent<Image>().sprite = projectileImages[0];
            }
        }
    }

    //Update Shootables After Firing
    public void UpdateShootableFired()
    {
        currentProjectiles = 0;
        for (var i = 0; i < shootIcon.Length; i++)
        {
            if (i < currentProjectiles)
            {
                shootIcon[i].GetComponent<Image>().sprite = projectileImages[1];
            }
            else
            {
                shootIcon[i].GetComponent<Image>().sprite = projectileImages[0];
            }
        }
    }

    //Update Messiness Meter
    public void UpdateMessyMeter(float newValue)
    {
        //Get Percent
        float meterPercentage = (newValue / maxMessiness);
        messyMeter.localScale = new Vector3(meterPercentage, 1f, 1f);

        UpdateMessyText(meterPercentage);
    }

    //Update Messy Text
    private void UpdateMessyText(float percent)
    {
        int targetText = 0;

        if (percent < 0.33f)
        {
            targetText = 0;
        }
        else if (percent >= 0.33f && percent < 0.66f)
        {
            targetText = 1;
        }
        else
        {
            targetText = 2;
        }
        messyText.GetComponent<TMPro.TextMeshProUGUI>().text = messyTextValues[targetText];
    }
}
