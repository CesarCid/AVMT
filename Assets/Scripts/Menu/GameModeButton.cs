using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVMT
{
    public class GameModeButton : MonoBehaviour
    {
        [SerializeField]
        private GameMode gameMode = default;

        public void LoadGameMode()
        {
            AppManager.Instance.LoadMode(gameMode);
        }

        public void ReloadGameMode()
        {
            AppManager.Instance.ReloadMode();
        }
    }
}
