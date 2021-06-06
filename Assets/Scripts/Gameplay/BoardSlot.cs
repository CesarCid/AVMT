using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AVMT.Gameplay
{
    public class BoardSlot : MonoBehaviour
    {
        public GamePiece piece;

        public Vector2Int index;

        private bool[] availableMoves = new bool[4];

        //private List<Direction> availableMoves;
        //public List<Direction> AvailableMoves => availableMoves;

        public void Select()
        {
            piece.Select();
        }

        public void Deselect()
        {
            piece.Deselect();
        }

        public bool GetAvailableMove(Direction direction) 
        {
            return availableMoves[(int)direction];
        }

        public void AddAvailableMove(Direction direction)
        {
            availableMoves[(int)direction] = true;
        }

        #region Mouse Input Debug

#if UNITY_EDITOR
        private void OnMouseDown()
        {
            TouchController.Instance.DebugOnMouseDown();
        }

        private void OnMouseUp()
        {
            TouchController.Instance.DebugOnMouseUp();
        }
#endif

        #endregion
    }
}
