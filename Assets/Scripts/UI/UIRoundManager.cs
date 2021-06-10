using UnityEngine;
using UnityEngine.UI;

namespace AVMT.Gameplay
{
    public class UIRoundManager : MonoBehaviour
    {
        [SerializeField]
        private Text text;

        private RoundManager roundManager;
        private void Start()
        {
            roundManager = RoundManager.Instance;

            roundManager.onRoundStarted += UpdateText;
        }

        private void UpdateText(int round) 
        {
            text.text = "Round " + (round + 1);
        }
    }
}