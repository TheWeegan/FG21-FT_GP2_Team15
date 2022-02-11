using System.Collections;
using UnityEngine;
using Cinemachine;
using Enums;

public class CameraStateManager : MonoBehaviour
{
    [SerializeField] private static CinemachineFreeLook thirdPersonCam;
    [SerializeField] private static CinemachineVirtualCamera pauseMenuCam;
    [SerializeField] private static CinemachineVirtualCamera cinematicCam;
    [SerializeField] private Transform player;
    private static CinemachineBrain brain;

    private static CameraStateManager instance; // only used for internal Coroutines

    public static bool IsTransitioning() => brain.IsBlending;

    #region Unity events - initialization
    private void Awake()
    {
        #region GetComponents if null
        if (player == null) player = GameObject.Find("Player").transform;
        if (thirdPersonCam == null) thirdPersonCam = GetComponentInChildren<CinemachineFreeLook>();
        if (pauseMenuCam == null) pauseMenuCam = GameObject.Find("Pause menu camera").GetComponent<CinemachineVirtualCamera>();
        #endregion

        brain = thirdPersonCam?.transform.root.GetComponentInChildren<CinemachineBrain>();

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
        Invoke(nameof(PausedTimeScaleCheck), 0.1f);
    }
    #endregion

    public static CameraPerspective GetPerspective()
    {
          return brain.ActiveVirtualCamera == (ICinemachineCamera)thirdPersonCam ? CameraPerspective.thirdPerson :
                 brain.ActiveVirtualCamera == (ICinemachineCamera)pauseMenuCam   ? CameraPerspective.pauseMenu   : CameraPerspective.cinematic;
    }

    public static void SwapToCamera(CameraPerspective newPerspective)
    {
        ICinemachineCamera newCamera = newPerspective == CameraPerspective.pauseMenu ?   (ICinemachineCamera)pauseMenuCam :
                                       newPerspective == CameraPerspective.thirdPerson ? (ICinemachineCamera)thirdPersonCam :
                                       newPerspective == CameraPerspective.cinematic ?    (ICinemachineCamera)cinematicCam : 
                                                                                         (ICinemachineCamera)thirdPersonCam;
        (brain.ActiveVirtualCamera.Priority, newCamera.Priority) = (newCamera.Priority, brain.ActiveVirtualCamera.Priority);

        if (newPerspective == CameraPerspective.pauseMenu)
        {
            instance.StartCoroutine(PauseMenuRoutine());
        }
    }
    private void PausedTimeScaleCheck()
    {
        if (brain.ActiveVirtualCamera != null)
            if (brain.ActiveVirtualCamera.Equals((ICinemachineCamera)pauseMenuCam) && Time.timeScale != 0)
                Time.timeScale = 0;
    }
    private static IEnumerator PauseMenuRoutine()
    {
        yield return new WaitForSeconds(0.2f); // because instant change causes freezing
        yield return new WaitWhile(() => brain.IsBlending);
        Time.timeScale = 0;
        yield return new WaitWhile(() => brain.ActiveVirtualCamera == (ICinemachineCamera)pauseMenuCam);
        Time.timeScale = 1;
    }

}
