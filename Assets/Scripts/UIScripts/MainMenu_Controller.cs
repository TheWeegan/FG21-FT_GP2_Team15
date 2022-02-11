using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Cinemachine;

public class MainMenu_Controller : MonoBehaviour
{
    //Buttons & Arrow References
    [Header("Arrows")]
    [SerializeField] GameObject[] menuArrows;
    [Header("Buttons")]
    [SerializeField] string[] mainMenuButtons;
    [SerializeField] string[] settingsButtons;
    [SerializeField] string[] soundButtons;
    [SerializeField] string[] musicButtons;
    [SerializeField] string[] sensitivityButtons;
    [SerializeField] string[] resolutionButtons;
    [SerializeField] string[] pauseButtons;
    [SerializeField] string[] controlsButtons;

    //Text Objects
    [Header("Text Objects")]
    [SerializeField] GameObject menuText;
    [SerializeField] GameObject settingsText;
    [SerializeField] GameObject soundText;
    [SerializeField] GameObject musicText;
    [SerializeField] GameObject sensitivityText;
    [SerializeField] GameObject resolutionText;
    [SerializeField] GameObject pauseText;
    [SerializeField] GameObject controlsText;

    //Menu References
    [Header("Menus")]
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject[] settingsSubMenu;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject controlsMenu;
    [SerializeField] GameObject endMenu;

    //End Screen
    [Header("Menus")]
    [SerializeField] GameObject creditsMenu;
    [SerializeField] GameObject[] creditsPage;
    [SerializeField] string[] roleText;
    [SerializeField] string[] name1;
    [SerializeField] string[] name2;
    [SerializeField] string[] name3;
    [SerializeField] GameObject roleObject;
    [SerializeField] GameObject[] nameObject;
    private string creditsRole = "Artists";

    //Blink Panel
    [Header("Blink Panel")]
    [SerializeField] GameObject blinkPanel;
    private Image blinkImage;
    private float blinkTimer = 0f;
    private float blinkStay = .85f;
    private float blinkLeave = .85f;
    private bool blinkShow = false;

    //Stopwatch
    [Header("Stopwatch")]
    [SerializeField] GameObject stopwatchText;

    //Current Variables
    private int currentSelection = 0;
    private string currentMenu = "MainMenu";
    private string[] currentButtonList;
    private bool onPause = false;
    private bool gameStarted = false;
    private bool inEndGame = false; //For when the end screen appears


    private CursorHandler cursorHandler;
    private PlayerManager playerManager;
    private Settings cameraSettings;

    public bool OnPause { get => onPause; }

    //Prepare For First Initialization
    private void Awake()
    {
        InitialHide();
        currentButtonList = mainMenuButtons;
        menuArrows[0].SetActive(false);
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        cameraSettings = GameObject.Find("GameManager")?.GetComponent<Settings>();
        
        //ReplaceText(menuText, mainMenuButtons[0]);
    }

    //Update Stopwatch
    private void UpdateStopWatch()
    {
        stopwatchText.GetComponent<TMPro.TextMeshProUGUI>().text = "Cycle Duration " + UIScripts.TimerController.instance.GetTimer.ToString() + "s";
    }

    private void Start()
    {
        blinkImage = blinkPanel.GetComponent<Image>();
        blinkImage.enabled = true;
        cursorHandler = GameManager.instance.GetComponent<CursorHandler>();

        playerManager.DeActivate();
        cursorHandler.FreeCursor();
    }

    private void ExitPauseScreen()
    {
        playerManager.Activate();
        cursorHandler.LockCursor();
        CameraStateManager.SwapToCamera(Enums.CameraPerspective.thirdPerson);


        // inactivate menu stuff here
        onPause = false;
    }
    private void EnterPauseScreen()
    {
        playerManager.DeActivate();
        cursorHandler.FreeCursor();
        CameraStateManager.SwapToCamera(Enums.CameraPerspective.pauseMenu);

        DeactivateAllMenus();
        pauseMenu.SetActive(true);
        menuArrows[0].SetActive(false);
        menuArrows[1].SetActive(true);
        currentSelection = 0;
        currentMenu = "PauseMenu";
        onPause = true;
        currentButtonList = pauseButtons;

        UpdateStopWatch();
    }

    public void StartEndScreen()
    {
        playerManager.DeActivate();
        cursorHandler.FreeCursor();
        CameraStateManager.SwapToCamera(Enums.CameraPerspective.pauseMenu);

        DeactivateAllMenus();
        endMenu.SetActive(true);
        menuArrows[0].SetActive(false);
        menuArrows[1].SetActive(false);
        currentMenu = "EndMenu";
        inEndGame = true;
        blinkImage.enabled = false;

        UpdateStopWatch();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gameStarted && !CameraStateManager.IsTransitioning() && !inEndGame)
        {
            if (!onPause)
            {
                EnterPauseScreen();
            }
            else
            {
                ExitPauseScreen();
            }
        }

        if (inEndGame == false)
        {
            UpdateBlink(); // this should probably not be called when not in menu
        }
    }

    //Update Blinker
    private void UpdateBlink()
    {
        blinkTimer += 1f * Time.unscaledDeltaTime;
        if (blinkShow)
        {
            if (blinkTimer >= blinkStay)
            {
                blinkTimer = 0f;
                blinkShow = false;
                blinkImage.enabled = false;
            }
        }
        else
        {
            if (blinkTimer >= blinkLeave)
            {
                blinkTimer = 0f;
                blinkShow = true;
                blinkImage.enabled = true;
            }
        }
    }

    //Reset Blink
    private void ResetBlink()
    {
        blinkTimer = 0f;
        blinkShow = true;
        blinkImage.enabled = true;
    }

    //Replace Text
    private void ReplaceText(GameObject textObject, string newText)
    {
        textObject.GetComponent<TMPro.TextMeshProUGUI>().text = newText;
    }

    //Hide Everything On Awake
    private void InitialHide()
    {
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(false);
        controlsMenu.SetActive(false);
        endMenu.SetActive(false);
        creditsMenu.SetActive(false);

        for (var i = 0; i < settingsSubMenu.Length; i++)
        {
            settingsSubMenu[i].SetActive(false);
        }

        for (var i = 0; i < creditsPage.Length; i++)
        {
            creditsPage[i].SetActive(false);
        }
    }

    //Deactivate Full Menu Systems
    private void DeactivateAllMenus()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(false);
        controlsMenu.SetActive(false);
        endMenu.SetActive(false);
        creditsMenu.SetActive(false);

        for (var i = 0; i < settingsSubMenu.Length; i++)
        {
            settingsSubMenu[i].SetActive(false);
        }

        for (var i = 0; i < creditsPage.Length; i++)
        {
            creditsPage[i].SetActive(false);
        }
    }

    //Update Visible Arrows
    private void UpdateArrows()
    {
        int type;

        //Check Current Selection
        if (currentSelection > 0 && currentSelection < currentButtonList.Length-1)
        {
            type = 1;
        }
        else
        {
            if (currentSelection == 0)
            {
                type = 2;
            }
            else
            {
                type = 0;
            }
        }

        //Update Arrows
        switch (type)
        {
            case 0:
                menuArrows[0].SetActive(true);
                menuArrows[1].SetActive(false);
                break;
            case 1:
                menuArrows[0].SetActive(true);
                menuArrows[1].SetActive(true);
                break;
            case 2:
                menuArrows[0].SetActive(false);
                menuArrows[1].SetActive(true);
                break;
        }
    }

    //Press Enter Button
    public void ButtonEnter()
    {
        if (inEndGame == false)
        {
            //In Main Menu
            if (currentMenu == "MainMenu")
            {
                switch (currentSelection)
                {
                    case 0:
                        //Start Game
                        if (!gameStarted) GameManager.instance.StartTimer();
                        playerManager.Activate();
                        cursorHandler.LockCursor();
                        CameraStateManager.SwapToCamera(Enums.CameraPerspective.thirdPerson);
                        gameStarted = true;
                        Time.timeScale = 1;
                        break;
                    case 1:
                        //Go To Settings
                        currentButtonList = settingsButtons;
                        DeactivateAllMenus();
                        currentSelection = 0;
                        menuArrows[0].SetActive(false);
                        menuArrows[1].SetActive(true);
                        settingsMenu.SetActive(true);
                        currentMenu = "Settings";
                        ReplaceText(settingsText, settingsButtons[0]);
                        break;
                    case 2:
                        //Quit Game
                        Application.Quit();
                        break;
                }
            }
            else if (currentMenu == "Settings")
            {
                switch (currentSelection)
                {
                    case 0:
                        //Go To Sound
                        int currentAudioSetting = AudioManager.Instance.GetScaledSoundVolume();
                        currentButtonList = soundButtons;
                        DeactivateAllMenus();
                        currentSelection = currentAudioSetting;
                        menuArrows[0].SetActive(true);
                        menuArrows[1].SetActive(false);
                        settingsSubMenu[0].SetActive(true);
                        currentMenu = "Sound";
                        ReplaceText(soundText, soundButtons[currentSelection]);
                        break;
                    case 1:
                        //Go To Music
                        int currentMusicSetting = AudioManager.Instance.GetScaledMusicVolume();
                        currentButtonList = musicButtons;
                        DeactivateAllMenus();
                        currentSelection = currentMusicSetting;
                        menuArrows[0].SetActive(true);
                        menuArrows[1].SetActive(false);
                        settingsSubMenu[1].SetActive(true);
                        currentMenu = "Music";
                        ReplaceText(musicText, musicButtons[currentSelection]);
                        break;
                    case 2:
                        //Go To Sensitivity
                        int currentMouseSetting = cameraSettings.GetScaledMouseSens();
                        currentButtonList = sensitivityButtons;
                        DeactivateAllMenus();
                        currentSelection = currentMouseSetting;
                        menuArrows[0].SetActive(true);
                        menuArrows[1].SetActive(false);
                        settingsSubMenu[2].SetActive(true);
                        currentMenu = "Sensitivity";
                        ReplaceText(sensitivityText, sensitivityButtons[currentSelection]);
                        break;
                    case 3:
                        //Go To Resolution
                        int currentResolutionSetting = TranslateCurrentResolution();
                        currentButtonList = resolutionButtons;
                        DeactivateAllMenus();
                        currentSelection = currentResolutionSetting;
                        menuArrows[0].SetActive(true);
                        menuArrows[1].SetActive(true);
                        settingsSubMenu[3].SetActive(true);
                        currentMenu = "Resolution";
                        ReplaceText(resolutionText, resolutionButtons[currentSelection]);
                        break;
                }
            }
            else if (currentMenu == "PauseMenu")
            {
                switch (currentSelection)
                {
                    case 0:
                        //Resume Game
                        ExitPauseScreen();
                        // To do: Arian should check that this is ok | It's okay - Arian :)
                        break;
                    case 1:
                        //Restart Game
                        GameManager.instance.GameRestart();
                        // To do: Arian should check that this is ok | It's okay - Arian :)
                        break;
                    case 2:
                        //Go To Controls
                        currentButtonList = controlsButtons;
                        DeactivateAllMenus();
                        currentSelection = 0;
                        menuArrows[0].SetActive(false);
                        menuArrows[1].SetActive(true);
                        controlsMenu.SetActive(true);
                        currentMenu = "Controls";
                        ReplaceText(controlsText, controlsButtons[0]);
                        break;
                    case 3:
                        //Go To Settings
                        currentButtonList = settingsButtons;
                        DeactivateAllMenus();
                        currentSelection = 0;
                        menuArrows[0].SetActive(false);
                        menuArrows[1].SetActive(true);
                        settingsMenu.SetActive(true);
                        currentMenu = "Settings";
                        ReplaceText(settingsText, settingsButtons[0]);
                        break;
                    case 4:
                        //Go To Main Menu
                        currentButtonList = mainMenuButtons;
                        DeactivateAllMenus();
                        currentSelection = 1;
                        menuArrows[0].SetActive(true);
                        menuArrows[1].SetActive(true);
                        mainMenu.SetActive(true);
                        currentMenu = "MainMenu";
                        ReplaceText(settingsText, mainMenuButtons[1]);
                        onPause = false;
                        break;
                }
            }
            else
            {
                //Apply changes to settings
                switch (currentMenu)
                {
                    case "Sound":
                        //Current sound level (0-10) - Use currentSelection variable (0 being off and 10 being max)
                        AudioManager.Instance.SetSoundVolume(currentSelection);
                        GoBackToSettings(0);
                        break;
                    case "Music":
                        //Current music level (0-10) - Use currentSelection variable (0 being off and 10 being max)
                        AudioManager.Instance.SetMusicVolume(currentSelection);
                        GoBackToSettings(1);
                        break;
                    case "Sensitivity":
                        //Current sensitivity level (0-9) - Use currentSelection variable (0 being 10% and 9 being 100%)
                        
                        cameraSettings.UpdateCameraSpeed(currentSelection);
                        GoBackToSettings(2);
                        break;
                    case "Resolution":
                        switch (currentSelection)
                        {
                            case 0: // 1280 x 720
                                Screen.SetResolution(1280, 720, true);
                                break;
                            case 1: // 1920 x 1080
                                Screen.SetResolution(1920, 1080, true);
                                break;
                            case 2: // 2560 x 1440
                                Screen.SetResolution(2560, 1440, true);
                                break;
                            case 3: // 3840 x 2160
                                Screen.SetResolution(3840, 2160, true);
                                break;
                        }
                        GoBackToSettings(3);
                        break;
                }
            }
            ResetBlink();
        }
        else // In End Screen
        {
            switch (currentMenu)
            {
                case "EndMenu":
                    //Go To Thanks Credits Page
                    DeactivateAllMenus();
                    creditsMenu.SetActive(true);
                    creditsPage[0].SetActive(true);
                    currentMenu = "ThanksPage";
                    break;
                case "ThanksPage":
                    //Go To Credits Page
                    DeactivateAllMenus();
                    creditsMenu.SetActive(true);
                    creditsPage[1].SetActive(true);
                    currentMenu = "NamesPage";
                    nameObject[0].GetComponent<TMPro.TextMeshProUGUI>().text = name1[0];
                    nameObject[1].GetComponent<TMPro.TextMeshProUGUI>().text = name2[0];
                    nameObject[2].GetComponent<TMPro.TextMeshProUGUI>().text = name3[0];
                    roleObject.GetComponent<TMPro.TextMeshProUGUI>().text = roleText[0];
                    creditsRole = "Artists";
                    break;
                case "NamesPage":
                    //Go To Thanks to Futuregames Page
                    UpdateCreditsNames();
                    break;
                case "FuturePage":
                    //Go To Special Thanks To You Page
                    DeactivateAllMenus();
                    creditsMenu.SetActive(true);
                    creditsPage[3].SetActive(true);
                    currentMenu = "SpecialPage";
                    break;
                case "SpecialPage":
                    //Restart Game
                    GameManager.instance.GameRestart();
                    break;
            }
        }
    }

    private void UpdateCreditsNames()
    {
        switch (creditsRole)
        {
            case "Artists":
                nameObject[0].GetComponent<TMPro.TextMeshProUGUI>().text = name1[1];
                nameObject[1].GetComponent<TMPro.TextMeshProUGUI>().text = name2[1];
                nameObject[2].GetComponent<TMPro.TextMeshProUGUI>().text = name3[1];
                roleObject.GetComponent<TMPro.TextMeshProUGUI>().text = roleText[1];
                creditsRole = "Designers";
                break;
            case "Designers":
                nameObject[0].GetComponent<TMPro.TextMeshProUGUI>().text = name1[2];
                nameObject[1].GetComponent<TMPro.TextMeshProUGUI>().text = name2[2];
                nameObject[2].GetComponent<TMPro.TextMeshProUGUI>().text = name3[2];
                roleObject.GetComponent<TMPro.TextMeshProUGUI>().text = roleText[2];
                creditsRole = "Programmers";
                break;
            case "Programmers":
                nameObject[0].GetComponent<TMPro.TextMeshProUGUI>().text = name1[3];
                nameObject[1].GetComponent<TMPro.TextMeshProUGUI>().text = name2[3];
                nameObject[2].GetComponent<TMPro.TextMeshProUGUI>().text = name3[3];
                roleObject.GetComponent<TMPro.TextMeshProUGUI>().text = roleText[3];
                creditsRole = "ProjectManagers";
                break;
            case "ProjectManagers":
                nameObject[0].GetComponent<TMPro.TextMeshProUGUI>().text = name1[4];
                nameObject[1].GetComponent<TMPro.TextMeshProUGUI>().text = name2[4];
                nameObject[2].GetComponent<TMPro.TextMeshProUGUI>().text = name3[4];
                roleObject.GetComponent<TMPro.TextMeshProUGUI>().text = roleText[4];
                creditsRole = "QA";
                break;
            case "QA":
                //Go To Thanks to Futuregames Page
                DeactivateAllMenus();
                creditsMenu.SetActive(true);
                creditsPage[2].SetActive(true);
                currentMenu = "FuturePage";
                break;
        }
    }

    private int TranslateCurrentResolution()
    {
        // checks the current resolution and returns the setting option it relates to
        Resolution resolution = Screen.currentResolution;
        int settingOption = resolution.width <= 1280 ? 0 :
                            resolution.width <= 1920 ? 1 :
                            resolution.width <= 2560 ? 2 : 3;

        return settingOption;
    }
    private void GoBackToSettings(int newSelection)
    {
        switch (newSelection)
        {
            case 0:
                currentSelection = 0;
                menuArrows[0].SetActive(false);
                menuArrows[1].SetActive(true);
                ReplaceText(settingsText, settingsButtons[0]);
                break;
            case 1:
                currentSelection = 1;
                menuArrows[0].SetActive(true);
                menuArrows[1].SetActive(true);
                ReplaceText(settingsText, settingsButtons[1]);
                break;
            case 2:
                currentSelection = 2;
                menuArrows[0].SetActive(true);
                menuArrows[1].SetActive(true);
                ReplaceText(settingsText, settingsButtons[2]);
                break;
            case 3:
                currentSelection = 3;
                menuArrows[0].SetActive(true);
                menuArrows[1].SetActive(false);
                ReplaceText(settingsText, settingsButtons[3]);
                break;
        }

        currentButtonList = settingsButtons;
        DeactivateAllMenus();
        settingsMenu.SetActive(true);
        currentMenu = "Settings";
    }

    //Press Return Button
    public void ButtonReturn()
    {
        if (inEndGame == false)
        {
            //In Settings Menu
            if (currentMenu == "Settings")
            {
                if (onPause == false)
                {
                    currentButtonList = mainMenuButtons;
                    DeactivateAllMenus();
                    currentSelection = 1;
                    menuArrows[0].SetActive(true);
                    menuArrows[1].SetActive(true);
                    mainMenu.SetActive(true);
                    currentMenu = "MainMenu";
                    ReplaceText(settingsText, mainMenuButtons[1]);
                }
                else
                {
                    currentButtonList = pauseButtons;
                    DeactivateAllMenus();
                    currentSelection = 3;
                    menuArrows[0].SetActive(true);
                    menuArrows[1].SetActive(true);
                    pauseMenu.SetActive(true);
                    currentMenu = "PauseMenu";
                    ReplaceText(pauseText, pauseButtons[3]);
                }
            }
            //In Sub Settings Menu
            else if (currentMenu != "Settings" && currentMenu != "MainMenu")
            {
                if (currentMenu != "Controls")
                {
                    if (currentMenu != "PauseMenu")
                    {
                        switch (currentMenu)
                        {
                            case "Sound":
                                currentSelection = 0;
                                menuArrows[0].SetActive(false);
                                menuArrows[1].SetActive(true);
                                ReplaceText(settingsText, settingsButtons[0]);
                                break;
                            case "Music":
                                currentSelection = 1;
                                menuArrows[0].SetActive(true);
                                menuArrows[1].SetActive(true);
                                ReplaceText(settingsText, settingsButtons[1]);
                                break;
                            case "Sensitivity":
                                currentSelection = 2;
                                menuArrows[0].SetActive(true);
                                menuArrows[1].SetActive(true);
                                ReplaceText(settingsText, settingsButtons[2]);
                                break;
                            case "Resolution":
                                currentSelection = 3;
                                menuArrows[0].SetActive(true);
                                menuArrows[1].SetActive(false);
                                ReplaceText(settingsText, settingsButtons[3]);
                                break;
                        }

                        currentButtonList = settingsButtons;
                        DeactivateAllMenus();
                        settingsMenu.SetActive(true);
                        currentMenu = "Settings";
                    }
                }
                else
                {
                    currentButtonList = pauseButtons;
                    DeactivateAllMenus();
                    currentSelection = 2;
                    menuArrows[0].SetActive(true);
                    menuArrows[1].SetActive(true);
                    pauseMenu.SetActive(true);
                    currentMenu = "PauseMenu";
                    ReplaceText(pauseText, pauseButtons[2]);
                }
            }
            ResetBlink();
        }
        else // In End Screen
        {
            if (currentMenu == "SpecialPage")
            {
                Application.Quit();
            }
        }
    }

    //Press Up Button
    public void ButtonUp()
    {
        if (inEndGame == false) 
        {
            //Clamp
            if (currentSelection > 0)
            {
                currentSelection--;

                switch (currentMenu)
                {
                    case "PauseMenu":
                        ReplaceText(pauseText, pauseButtons[currentSelection]);
                        break;
                    case "Controls":
                        ReplaceText(controlsText, controlsButtons[currentSelection]);
                        break;
                    case "MainMenu":
                        ReplaceText(menuText, mainMenuButtons[currentSelection]);
                        break;
                    case "Settings":
                        ReplaceText(settingsText, settingsButtons[currentSelection]);
                        break;
                    case "Sound":
                        ReplaceText(soundText, soundButtons[currentSelection]);
                        break;
                    case "Music":
                        ReplaceText(musicText, musicButtons[currentSelection]);
                        break;
                    case "Sensitivity":
                        ReplaceText(sensitivityText, sensitivityButtons[currentSelection]);
                        break;
                    case "Resolution":
                        ReplaceText(resolutionText, resolutionButtons[currentSelection]);
                        break;
                }

                UpdateArrows();
                ResetBlink();
            }
        }
    }

    public void ButtonDown()
    {
        if (inEndGame == false)
        {
            //Clamp
            if (currentSelection < currentButtonList.Length - 1)
            {
                currentSelection++;

                switch (currentMenu)
                {
                    case "PauseMenu":
                        ReplaceText(pauseText, pauseButtons[currentSelection]);
                        break;
                    case "Controls":
                        ReplaceText(controlsText, controlsButtons[currentSelection]);
                        break;
                    case "MainMenu":
                        ReplaceText(menuText, mainMenuButtons[currentSelection]);
                        break;
                    case "Settings":
                        ReplaceText(settingsText, settingsButtons[currentSelection]);
                        break;
                    case "Sound":
                        ReplaceText(soundText, soundButtons[currentSelection]);
                        break;
                    case "Music":
                        ReplaceText(musicText, musicButtons[currentSelection]);
                        break;
                    case "Sensitivity":
                        ReplaceText(sensitivityText, sensitivityButtons[currentSelection]);
                        break;
                    case "Resolution":
                        ReplaceText(resolutionText, resolutionButtons[currentSelection]);
                        break;
                }

                UpdateArrows();
                ResetBlink();
            }
        }
    }
}
