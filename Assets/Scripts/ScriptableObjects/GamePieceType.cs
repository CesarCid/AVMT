using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AVMT.AppManager;

namespace AVMT.Gameplay
{
    [CreateAssetMenu(menuName = "System/Config/Piece Type", fileName = "PieceType")]
    public class GamePieceType : ScriptableObject
    {
        [SerializeField]
        private GameObject prefab;
        public GameObject Prefab => prefab;

        [SerializeField]
        private Sprite sprite;
        public Sprite Sprite => sprite;

        public override string ToString()
        {
            return this.name;
        }
    }
}