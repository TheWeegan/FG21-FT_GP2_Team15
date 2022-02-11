using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroLoadScene : MonoBehaviour
{
    // this gets called by animation events
    public void LoadNextScene()
    {
        int scenesInBuild = SceneManager.sceneCountInBuildSettings;
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextScene <= (scenesInBuild - 1))
        {
            SceneManager.LoadScene(nextScene);
        }
    }

    //[SerializeField] private int secondsBeforeLoadNext = 4;

    //private void Start()
    //{
    //
    //    int scenesInBuild = SceneManager.sceneCountInBuildSettings;
    //    int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
    //
    //    print("amount of scenes: " + scenesInBuild);
    //    print("next scene: " + nextScene);
    //
    //    if (nextScene <= (scenesInBuild - 1))
    //    {
    //        print("load scene");
    //        //StartCoroutine("LoadNextScene", nextScene);
    //        
    //    }
    //
    //    
    //}

    //IEnumerator LoadNextScene(int scene)
    //{
    //    yield return new WaitForSeconds(secondsBeforeLoadNext);
    //    SceneManager.LoadScene(scene);

    //}
}
