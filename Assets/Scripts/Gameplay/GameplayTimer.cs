using System;
using UnityEngine;

namespace AVMT.Gameplay
{
    [Serializable]
    public class GameplayTimer : MonoBehaviour
    {
        private RoundManager roundManager;
        
        private float remainingTime = RoundManager.RoundTimeLimit;
        public float RemainingTime => remainingTime;

        private bool timerOn = false;

        public Action onTimerStart;
        public Action onTimerEnd;

        private void Start()
        {
            roundManager = RoundManager.Instance;

            roundManager.onRoundStarted += (i) => StartTimer();
            roundManager.onRoundFinished += (b) => StopTimer();
        }

        void Update()
        {
            if (timerOn)
            {
                if (remainingTime <= 0)
                {
                    StopTimer();
                    onTimerEnd?.Invoke();
                }
                
                remainingTime -= Time.deltaTime;
            }
        }

        public void SetupTimer(int seconds)
        {
            remainingTime = seconds;
        }

        public void StartTimer()
        {
            SetupTimer(RoundManager.RoundTimeLimit);
            timerOn = true;
            onTimerStart?.Invoke();
        }

        public void StopTimer()
        {
            remainingTime = 0;
            timerOn = false;            
        }
    }
}
