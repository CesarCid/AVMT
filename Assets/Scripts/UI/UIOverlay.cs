using UnityEngine;
using UnityEngine.UI;

namespace AVMT.Gameplay
{
    public class UIOverlay : MonoBehaviour
    {
        [SerializeField]
        private Canvas overlayCanvas;

        private RoundManager roundManager;

        private void Awake()
        {
            Hide();
        }

        private void Start()
        {
            roundManager = RoundManager.Instance;

            roundManager.onRoundFinished += OnRoundFinished;            
        }

        private void OnRoundFinished(bool success)
        {
            if (success == false) 
            {
                Show();            
            }
        }

        public void Show() 
        {
            overlayCanvas.enabled = true;
        }
        public void Hide() 
        {
            overlayCanvas.enabled = false;
        }
    }
}
