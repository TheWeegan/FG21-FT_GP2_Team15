using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSoundTrigger : MonoBehaviour
{
    public void StopIntroSound()
    {
        AudioManager.Instance?.StartGameplayMusic();
    }
    public void StartScreenSoundFX()
    {
        AudioManager.Instance?.PlayScreenSoundEffect();
    }
}
