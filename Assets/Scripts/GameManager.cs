using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject menuController;
    [SerializeField] float victoryInSeconds;

    public static GameManager instance; //used to call from other scripts as a singleton.

    public event Action collectibleCollected, shootableShot, victory, shootableCollected, deActivate, failedSuctionEvent,
                        suctionBlocket, startGame; //used as events, holds references to methods to call later.
    public Action<float> messScore, timer;
    public Action<int>  shootableShotInt;

    private float maxScore = 0;
    private CursorHandler cursorHandler;
    private UIScripts.TimerController timeController;

    public float MaxScore { set { maxScore = value; } get { return maxScore; } }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        timeController = UIScripts.TimerController.instance;
        cursorHandler = gameObject.GetComponent<CursorHandler>();
        AudioManager.Instance.StartGameplayMusic();
    }

    //Ability to start the timer from MainMenu_Controller - Added by Arian
    public void StartTimer()
    {
        timeController.BeginTimer();
    }

    private void OnEnable()
    {
        // subscribed here because audioManager exists before GameManager
        failedSuctionEvent += AudioManager.PlayWarningSound;
    }
    private void OnDisable()
    {
        failedSuctionEvent -= AudioManager.PlayWarningSound;
    }
    
    public void DelayedStart() //temp fix for preventing "eating" in new menu.
    {
        Invoke(nameof(StartGame), 0.3f);
    }
    private void StartGame()
    {
        startGame?.Invoke();
    }

    public void CollectiblesCollect() //use for trigger that will happen after collecting stuff. Scores interface etc.
    {
        collectibleCollected?.Invoke();
    }

    public void BlockSuction()
    {
        suctionBlocket?.Invoke();
    }

    public void ShootableCollected()
    {
        shootableCollected?.Invoke();
    }

    public void ShootableShot()
    {
        shootableShot?.Invoke();
    }


    public void ShootableShot(int newCurrent)
    {
        shootableShotInt?.Invoke(newCurrent);
    }
    public void FailedSuction()
    {
        failedSuctionEvent?.Invoke();
    }

    public void Victory()
    {
        victory?.Invoke();
        StartCoroutine(VictoryCanvasPopup());
        AudioManager.PlayWinningSound();
    }

    IEnumerator VictoryCanvasPopup()
    {
        yield return new WaitForSeconds(victoryInSeconds);
        menuController.GetComponent<MainMenu_Controller>().StartEndScreen();
    }

    public void DeActivate()
    {
        deActivate?.Invoke();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void GameRestart()
    {
        cursorHandler.LockCursor();
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
   
    public void QuitGame()
    {
        Application.Quit();
    }

    public void MessScore(float newValue)
    {
        messScore?.Invoke(newValue);
    }

}