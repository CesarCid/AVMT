using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AVMT.Gameplay
{
    public class GamePiecesController : MonoBehaviour
    {
        private const float FillEmptyDelay = 0.05f;

        [SerializeField]
        private GamePieceType[] gamePieces;

        [SerializeField]
        private GameBoard gameBoard;

        private TouchController touchController;
        private bool inputsAllowed = false;
        public bool InputsAllowed => inputsAllowed;

        public Action<BoardSlot> onSlotSelected;

        public Action<GamePiece, BoardSlot> onPieceMoved;

        public Action<BoardSlot, BoardSlot> onPiecesSwitched;

        public Action<GamePiece> onPieceBroken;

        public Action<List<int>> onMatchesFound;

        public Action onEmptySpacesFilled;

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
            ClearBoard();

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

        private void ClearBoard()
        {
            for (int y = 0; y < GameBoard.BoardLenght.y; y++)
            {
                for (int x = 0; x < GameBoard.BoardLenght.x; x++)
                {
                    gameBoard.Slots[x, y].DestroyPiece();
                }
            }
        }

        public bool UpdateAvailableMoves()
        {
            gameBoard.ClearDirty();
            return gameBoard.UpdateAvailableMoves();
        }

        public bool ResolveMatches()
        {
            bool matchesResolved = false;
            HashSet<Vector2Int> matchIndexes = new HashSet<Vector2Int>();
            List<int> foundMatchesLenght = new List<int>();

            foreach (int dirtyLine in gameBoard.dirtyLines)
            {
                if (gameBoard.GetMatchesInLine(dirtyLine, out List<List<int>> lineMatches))
                {
                    foreach (List<int> lineMatch in lineMatches)
                    {
                        foreach (int column in lineMatch)
                        {
                            matchIndexes.Add(new Vector2Int(column, dirtyLine));
                        }
                        foundMatchesLenght.Add(lineMatch.Count);
                    }
                    matchesResolved = true;
                }                
            }
            foreach (int dirtyColumn in gameBoard.dirtyColumns)
            {
                if (gameBoard.GetMatchesInColumn(dirtyColumn, out List<List<int>> columnMatches))
                {
                    foreach (List<int> columnMatch in columnMatches)
                    {
                        foreach (int line in columnMatch)
                        {
                            matchIndexes.Add(new Vector2Int(dirtyColumn, line));
                        }
                        foundMatchesLenght.Add(columnMatch.Count);
                    }
                    matchesResolved = true;
                } 
            }

            foreach(Vector2Int matchIndex in matchIndexes)
            {
                onPieceBroken?.Invoke(gameBoard.Slots[matchIndex.x, matchIndex.y].Piece);
                gameBoard.Break(gameBoard.Slots[matchIndex.x, matchIndex.y]);                
            }

            if (matchesResolved)
            {
                onMatchesFound?.Invoke(foundMatchesLenght);
            }

            return matchesResolved;
        }

        public void FillEmptySpaces() 
        {
            StartCoroutine(FillEmptySpacesLoop());
        }

        private IEnumerator FillEmptySpacesLoop()
        {
            for (int y = gameBoard.dirtyLines.Min(); y < GameBoard.BoardLenght.y; y++)
            {                
                foreach (int x in gameBoard.dirtyColumns)
                {
                    if (gameBoard.Slots[x, y].Piece == null)
                    {
                        BoardSlot occupiedSlot = GetFirstOccupiedSlotUpwards(y, x);

                        if (occupiedSlot == null)
                        { 
                            CreatePiece(GetRandomPieceType(), gameBoard.Slots[x, y]);
                        }
                        else
                        {
                            TryMovePiece(occupiedSlot, gameBoard.Slots[x, y]);
                        }
                    }
                }
                yield return new WaitForSeconds(FillEmptyDelay);
                
            }
            onEmptySpacesFilled?.Invoke();
        }

        private BoardSlot GetFirstOccupiedSlotUpwards(int lineFrom, int column)
        {
            for (int line = lineFrom; line < GameBoard.BoardLenght.y; line++)
            {
                if (gameBoard.Slots[column, line].Piece == null)
                {
                    continue;
                }
                return gameBoard.Slots[column, line];
            }
            return null;
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

            gameBoard.SetDirty(slotParent);

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
            onSlotSelected?.Invoke(slot);
            slot.Select();
        }

        private void DeselectSlot(BoardSlot slot)
        {
            slot.Deselect();
            selectedSlot = null;
        }

        private bool TryMovePiece(BoardSlot from, BoardSlot to)
        {
            if (from.Piece == null)
            {
                return false;
            }
            GamePiece fromPiece = from.Piece;
            from.ClearPiece();

            return MovePiece(fromPiece, to);
        }

        private bool MovePiece(GamePiece from, BoardSlot to) 
        {
            to.Piece = from;            
            onPieceMoved?.Invoke(from, to);

            gameBoard.SetDirty(to);

            return true;
        }

        private bool TrySwitchPieces(BoardSlot slot1, BoardSlot slot2) 
        {
            Vector2Int movementDelta = slot2.index - slot1.index;
            
            if (movementDelta.magnitude > 1)
            {
                return false;
            }

            if (slot1.GetAvailableMove(GameBoard.GetDirectionFromVector(movementDelta)) == false)
            {
                return false;
            }
            
            GamePiece piece1 = slot1.Piece;
            GamePiece piece2 = slot2.Piece;

            if (!MovePiece(piece1, slot2) || !MovePiece(piece2, slot1))
            {
                return false;
            }

            onPiecesSwitched?.Invoke(slot1, slot2);

            return true;
        }
    }
}

