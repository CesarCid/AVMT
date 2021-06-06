using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AVMT.AppManager;

namespace AVMT
{
    [CreateAssetMenu(menuName = "System/Config/Game Mode", fileName = "GameMode")]
    public class GameMode : ScriptableObject
    {
        [SerializeField]
        private Scenes correspondingScene;
        public Scenes Scene => correspondingScene;
    }
}