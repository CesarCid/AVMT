using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AVMT.Gameplay
{
    public class UITimerManager : MonoBehaviour
    {
        [SerializeField]
        private Text text;

        [SerializeField]
        private GameplayTimer timer;

        private bool timerOn;

        private void OnValidate()
        {
            if (timer == null)
            {
                timer = FindObjectOfType<GameplayTimer>();
            }
        }

        private void Start()
        {
            timer.onTimerStart += OnTimerStart;
            timer.onTimerEnd += OnTimerEnd;
        }

        private void OnTimerStart() 
        {
            timerOn = true;
            text.color = Color.black;
        }
        private void OnTimerEnd() 
        {
            timerOn = false;
        }

        private void Update()
        {
            if (timerOn)
            {
                UpdateText();
            }
        }

        private void UpdateText()
        {
            string minutes = Mathf.FloorToInt(timer.RemainingTime / 60).ToString();
            string seconds = Mathf.Max(Mathf.FloorToInt(timer.RemainingTime % 60), 0).ToString("D2");

            if (timer.RemainingTime < 60)
            {
                text.text = seconds;
            }
            else
            {
                text.text = minutes + ":" + seconds;
            }

            text.color = (timer.RemainingTime <= 10f) ? Color.red : Color.black;
        }
    }
}