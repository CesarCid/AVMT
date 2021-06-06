using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AVMT.Gameplay
{
    public class GamePiece : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private GamePieceType type;
        public GamePieceType Type => type;

        public enum PieceState { Unselected, Selected, Moving }
        private PieceState state = PieceState.Unselected;

        private void Awake()
        {
            spriteRenderer.sprite = type.Sprite;
        }

        public void Select()
        {
            if (state == PieceState.Selected)
            {
                Deselect();
                return;
            }

            state = PieceState.Selected;
            spriteRenderer.color = Color.gray;
        }

        public void Deselect()
        {
            state = PieceState.Unselected;
            spriteRenderer.color = Color.white;
        }

        public void MoveTo(BoardSlot slot) 
        {
            transform.parent = slot.transform;
            transform.localPosition = Vector3.zero;
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