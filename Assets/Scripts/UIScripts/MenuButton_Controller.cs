using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton_Controller : MonoBehaviour
{
    //Necessary References
    [SerializeField] int buttonType;
    GameObject mainMenuController;
    private MainMenu_Controller menuScript;
    private Transform originPosition;
    private Vector3 positionAtPauseStart;

    //Variables For Different Transforms
    private Vector3 defaultPos;
    private Vector3 hoverPos;
    private Vector3 clickPos;

    //Button Movement Variables
    private int buttonState = 0;
    private bool clicked = false;
    private float clickedTimer = 0f;
    private float clickSpeed = 30f;

    private bool isUpdatingButtons = false;

    //Initialize Buttons
    void Start()
    {
        mainMenuController = GameObject.FindGameObjectWithTag("MainMenuController");
        menuScript = mainMenuController.GetComponent<MainMenu_Controller>();

        //Prepare State Transforms
        originPosition = this.transform.parent.transform;
        
    }

    private void Update()
    {
        if (Time.timeScale == 0)
        {
            if (!isUpdatingButtons)
            {
                positionAtPauseStart = transform.position; // instead of originPosition
            }
            UpdateButtonPosition(buttonState);
        }
        else if (isUpdatingButtons)
        {
            isUpdatingButtons = false;
        }

        //Revert To Hover State After Timer
        if (clicked)
        {
            clickedTimer += 1f * Time.unscaledDeltaTime;
            if (clickedTimer > .2f)
            {
                clicked = false;
                clickedTimer = 0f;
            }
        }
        // this could probably be removed - see UpdateButtonPosition()
        //hoverPos = originPosition.position + (transform.up * (0.0031f / 4f)); 
        //clickPos = originPosition.position + (transform.up * 0.0028f);
    }

    //Update Transforms Using Lerp Based On State
    private void UpdateButtonPosition(int state)
    {
        isUpdatingButtons = true;
        hoverPos = positionAtPauseStart + (transform.up * (0.0031f / 4f));
        clickPos = positionAtPauseStart + (transform.up * 0.0028f);
        switch (state)
        {
            case 0:
                if (transform.position != positionAtPauseStart)
                {
                    transform.position = Vector3.Lerp(transform.position, positionAtPauseStart, clickSpeed * Time.unscaledDeltaTime);
                }
                break;
            case 1:
                if (transform.position != hoverPos)
                {
                    transform.position = Vector3.Lerp(transform.position, hoverPos, clickSpeed * Time.unscaledDeltaTime);
                }
                break;
            case 2:
                if (transform.position != clickPos)
                {
                    transform.position = Vector3.Lerp(transform.position, clickPos, clickSpeed * Time.unscaledDeltaTime);
                }
                break;
        }
        
    }

    //Update Button To Hover State
    void OnMouseOver()
    {
        if (buttonState != 1 && clicked == false)
        {
            buttonState = 1;
        }
    }

    //Update Button To Default State
    void OnMouseExit()
    {
        if (buttonState != 0)
        {
            buttonState = 0;
        }
    }
    //Update Button To Click State & Activate Function In MainMenu_Controller
    void OnMouseDown()
    {
        buttonState = 2;
        clicked = true;

        //Identify Button
        switch (buttonType)
        {
            case 0:
                menuScript.ButtonUp();
                break;
            case 1:
                menuScript.ButtonDown();
                break;
            case 2:
                menuScript.ButtonEnter();
                break;
            case 3:
                menuScript.ButtonReturn();
                break;
        }
    }
}
