using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AVMT.Gameplay
{
    public class BoardSlot : MonoBehaviour
    {
        private GamePiece piece;
        public GamePiece Piece
        {
            get => piece;
            set 
            {
                piece = value;
                piece.MoveTo(this);
            } 
        }

        public Vector2Int index;

        private bool[] availableMoves = new bool[4];

        public void Break()
        {
            ClearPiece();
        }

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

        public void SetAvailableMove(Direction direction)
        {
            availableMoves[(int)direction] = true;
        }

        public void ClearPiece()
        {
            Destroy(Piece.transform);
            ClearAvailableMoves();
        }

        public void ClearAvailableMoves()
        {
            availableMoves.Initialize();
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
