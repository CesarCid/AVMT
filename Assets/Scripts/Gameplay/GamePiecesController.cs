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
                    /*
                    if (gameBoard.GetMatchesInLine(y, out List<List<int>> lineMatches))
                    {
                        foreach(List<int> list in lineMatches)
                        {
                            exceptions.Add(gameBoard.Slots[list[0], y].Piece.Type);
                        }
                    }

                    if (gameBoard.GetMatchesInColumn(x, out List<List<int>> columnMatches))
                    {
                        foreach (List<int> list in columnMatches)
                        {
                            exceptions.Add(gameBoard.Slots[x, list[0]].Piece.Type);
                        }
                    }
                    */
                    CreatePiece(GetRandomPieceType(exceptions), gameBoard.Slots[x, y]);
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

            return availablePieces.ElementAt(Random.Range(0, availablePieces.Count()));
        }


        private GameObject CreatePiece(GamePieceType type, BoardSlot slotParent)
        {
            GameObject piece = GameObject.Instantiate(type.Prefab, slotParent.transform);
            slotParent.Piece = piece.GetComponent<GamePiece>();
            return piece;
        }

        private void OnPieceTouchDown(GameObject go)
        {
            Debug.Log("[GamePiecesController] OnPieceTouchDown | piece: " + go.name);
        }

        private void OnPieceTouchUp(GameObject go)
        {
            Debug.Log("[GamePiecesController] OnPieceTouchUp | piece: " + go.name);
            BoardSlot slot = go.GetComponent<BoardSlot>();

            if (slot != null)
            {
                List<List<int>> matches;
                if (gameBoard.GetMatchesInColumn(slot.index.x, out matches)) 
                {
                    Debug.Log(matches.Count + " matches");
                }

                if (gameBoard.GetMatchesInColumnAfterMovement(slot.index, Direction.Right, out matches))
                {
                    Debug.Log(matches.Count + " column matches after moving to the right");
                }

                if (gameBoard.GetMatchesInLineAfterMovement(slot.index, Direction.Right, out matches))
                {
                    Debug.Log(matches.Count + " line matches after moving to the right");
                }

                if (gameBoard.GetMatchesInColumnAfterMovement(slot.index, Direction.Up, out matches))
                {
                    Debug.Log(matches.Count + " column matches after moving up");
                }

                if (gameBoard.GetMatchesInLineAfterMovement(slot.index, Direction.Up, out matches))
                {
                    Debug.Log(matches.Count + " line matches after moving up");
                }

                //foreach (var result in gameBoard.EvaluateMatchesInAllDirections(slot.index))
                //    Debug.Log(result);
                //Debug.Log(gameBoard.EvaluatePossibleMatchesInAllNeighbours(slot.index, slot.Piece.Type));


            }
        }
    }
}

