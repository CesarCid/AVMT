using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AVMT.Gameplay
{
    public class GamePiecesController : MonoBehaviour
    {
        [SerializeField]
        private GamePieceType[] gamePieces;

        [SerializeField]
        private GameBoard gameBoard;

        private TouchController touchController;

        private void OnValidate()
        {
            if (gameBoard != null)
            {
                return;
            }

            gameBoard = FindObjectOfType<GameBoard>();
        }

        void Awake()
        {
            touchController = TouchController.Instance;

            touchController.TouchedDownOnInteractable += OnPieceTouchDown;
            touchController.TouchedUpOnInteractable += OnPieceTouchUp;
        }

        public void PopulateBoard()
        {
            for (int y = 0; y < GameBoard.BoardLenght.y; y++)
            {
                for (int x = 0; x < GameBoard.BoardLenght.x; x++)
                {
                    List<GamePieceType> exceptions = new List<GamePieceType>();

                    if (x > 1)
                    {
                        if (gameBoard.Slots[x - 1, y].piece.Type == gameBoard.Slots[x - 2, y].piece.Type)
                        {
                            exceptions.Add(gameBoard.Slots[x - 1, y].piece.Type);
                        }
                    }

                    if (y > 1)
                    {
                        if (gameBoard.Slots[x, y - 1].piece.Type == gameBoard.Slots[x, y - 2].piece.Type)
                        {
                            exceptions.Add(gameBoard.Slots[x, y - 1].piece.Type);
                        }
                    }

                    CreatePiece(GetRandomPieceType(exceptions), gameBoard.Slots[x, y]);
                }
            }
        }

        public void UpdateAvailableMoves()
        {
            for (int y = 0; y < GameBoard.BoardLenght.y; y++)
            {
                for (int x = 0; x < GameBoard.BoardLenght.x; x++)
                {
                    Vector2Int currentSlot = new Vector2Int(x, y);

                    foreach (Direction direction in Enum.GetValues(typeof(Direction)))
                    {
                        if (gameBoard.GetMatchesInLineAfterMovement(currentSlot, direction, out List<List<int>> lineMatches) ||
                            gameBoard.GetMatchesInColumnAfterMovement(currentSlot, direction, out List<List<int>> columnMatches))
                        {
                            gameBoard.Slots[x, y].AddAvailableMove(direction);
                            Vector2Int movedSlot = currentSlot + GameBoard.GetDirectionVector(direction);
                            gameBoard.Slots[movedSlot.x, movedSlot.y].AddAvailableMove(GameBoard.GetOppositeDiretion(direction));
                        }
                    }
                }
            }
        }


        private GamePieceType GetRandomPieceType()
        {
            return GetRandomPieceType(new List<GamePieceType>());
        }
        private GamePieceType GetRandomPieceType(List<GamePieceType> exceptions)
        {
            IEnumerable<GamePieceType> availablePieces = gamePieces.Except(exceptions);

            return availablePieces.ElementAt(UnityEngine.Random.Range(0, availablePieces.Count()));
        }


        private GameObject CreatePiece(GamePieceType type, BoardSlot slotParent)
        {
            GameObject piece = Instantiate(type.Prefab, slotParent.transform);
            slotParent.piece = piece.GetComponent<GamePiece>();
            return piece;
        }

        BoardSlot preSelectedSlot = null;
        BoardSlot selectedSlot = null;

        private void OnPieceTouchDown(GameObject go)
        {
            Debug.Log("[GamePiecesController] OnPieceTouchDown | piece: " + go.name);
            BoardSlot slot = go.GetComponent<BoardSlot>();

            if (slot != null)
            {
                preSelectedSlot = slot;
            }
        }

        private void OnPieceTouchUp(GameObject go)
        {
            Debug.Log("[GamePiecesController] OnPieceTouchUp | piece: " + go.name);
            BoardSlot slot = go.GetComponent<BoardSlot>();

            if (slot != null)
            {
                if (preSelectedSlot == slot)
                {
                    SelectSlot(slot);
                }
            }

            preSelectedSlot = null;
        }

        private void SelectSlot(BoardSlot slot)
        {
            if (selectedSlot != null)
            {
                TryMovePiece(selectedSlot, slot);

                DeselectSlot(selectedSlot);
            }

            selectedSlot = slot;
            slot.Select();
        }

        private void DeselectSlot(BoardSlot slot)
        {
            slot.Deselect();
            selectedSlot = null;
        }

        private bool TryMovePiece(BoardSlot origin, BoardSlot target) 
        {
            Vector2Int movementDelta = origin.index - target.index;
            
            if (movementDelta.magnitude > 1)
            {
                return false;
            }

            if (origin.GetAvailableMove(GameBoard.GetDirectionFromVector(movementDelta)) == false)
            {
                return false;
            }

            /*
            GamePiece originPiece = origin.piece;
            GamePiece targetPiece = target.piece;

            origin.piece = targetPiece;
            target.piece = originPiece;
            */
            return true;
        }
    }
}

