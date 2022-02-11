using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UIScripts
{
    public class TimerController : MonoBehaviour
    {  
        public static TimerController instance;
        public float GetTimer { get { return (int)timerValue; } }
       
        private bool timerGoing;

        private float timerValue = 0;

        private void Awake()
        {
            instance = this;
        }

        public void BeginTimer()
        {
            timerGoing = true;
            StartCoroutine(UpdateTimer());
        }

        public void EndTimer()
        {
            timerGoing = false;
        }

        private IEnumerator UpdateTimer()
        {
            while (timerGoing)
            {
                timerValue += Time.deltaTime;
                yield return null;
            }
        
        }
    }
}