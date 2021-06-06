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
        private bool inputsAllowed = false;
        public bool InputsAllowed => inputsAllowed;

        public Action<BoardSlot> OnSlotSelected;
        public Action<GamePiece, BoardSlot> OnPieceMoved;

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

            OnPieceMoved += SetDirty;
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
                        if (gameBoard.Slots[x - 1, y].Piece.Type == gameBoard.Slots[x - 2, y].Piece.Type)
                        {
                            exceptions.Add(gameBoard.Slots[x - 1, y].Piece.Type);
                        }
                    }

                    if (y > 1)
                    {
                        if (gameBoard.Slots[x, y - 1].Piece.Type == gameBoard.Slots[x, y - 2].Piece.Type)
                        {
                            exceptions.Add(gameBoard.Slots[x, y - 1].Piece.Type);
                        }
                    }

                    CreatePiece(GetRandomPieceType(exceptions), gameBoard.Slots[x, y]);
                }
            }
        }

        public bool UpdateAvailableMoves()
        {
            bool anyAvailableMove = false;
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
                            gameBoard.Slots[x, y].SetAvailableMove(direction);
                            Vector2Int movedSlot = currentSlot + GameBoard.GetDirectionVector(direction);
                            gameBoard.Slots[movedSlot.x, movedSlot.y].SetAvailableMove(GameBoard.GetOppositeDiretion(direction));

                            anyAvailableMove = true;
                        }
                    }
                }
            }
            return anyAvailableMove;
        }

        public bool ResolveMatches()
        {
            bool matchesResolved = false;

            for (int y = 0; y < gameBoard.dirtyLines.Count; y++)
            {
                for (int x = 0; x < gameBoard.dirtyColumns.Count; x++)
                {
                    if (gameBoard.GetMatchesInColumn(x, out List<List<int>> columnMatches) ||
                        gameBoard.GetMatchesInLine(y, out List<List<int>> lineMatches))
                    { 
                        //Get matches and break pieces
                        //matchesResolved = true;                    
                    }
                }
            }
            return matchesResolved;
        }

        private void SetDirty(GamePiece piece, BoardSlot slot)
        {
            gameBoard.dirtyColumns.Add(slot.index.x);
            gameBoard.dirtyLines.Add(slot.index.y);
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
            if (slotParent.Piece != null)
            {
                slotParent.ClearPiece();
            }

            GameObject piece = Instantiate(type.Prefab, slotParent.transform);
            slotParent.Piece = piece.GetComponent<GamePiece>();
            return piece;
        }


        #region Inputs
        BoardSlot preSelectedSlot = null;
        BoardSlot selectedSlot = null;

        private void OnPieceTouchDown(GameObject go)
        {
            if (inputsAllowed == false) 
            {
                return;
            }

            Debug.Log("[GamePiecesController] OnPieceTouchDown | piece: " + go.name);
            BoardSlot slot = go.GetComponent<BoardSlot>();

            if (slot != null)
            {
                preSelectedSlot = slot;
            }
        }

        private void OnPieceTouchUp(GameObject go)
        {
            if (inputsAllowed == false)
            {
                return;
            }

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

        public void SetInputsAllowed(bool allowed)
        {
            inputsAllowed = allowed;
        }
        #endregion

        private void SelectSlot(BoardSlot slot)
        {
            if (selectedSlot != null)
            {
                BoardSlot previouslySelectedSlot = selectedSlot;

                DeselectSlot(previouslySelectedSlot);

                TrySwitchPieces(previouslySelectedSlot, slot);
                
                return;                
            }

            selectedSlot = slot;
            slot.Select();
        }

        private void DeselectSlot(BoardSlot slot)
        {
            slot.Deselect();
            selectedSlot = null;
        }

        private bool TrySwitchPieces(BoardSlot origin, BoardSlot target) 
        {
            Vector2Int movementDelta = target.index - origin.index;
            
            if (movementDelta.magnitude > 1)
            {
                return false;
            }

            if (origin.GetAvailableMove(GameBoard.GetDirectionFromVector(movementDelta)) == false)
            {
                return false;
            }
            
            GamePiece originPiece = origin.Piece;
            GamePiece targetPiece = target.Piece;

            origin.Piece = targetPiece;
            OnPieceMoved?.Invoke(originPiece, origin);
            target.Piece = originPiece;
            OnPieceMoved?.Invoke(targetPiece, target);

            return true;
        }
    }
}

