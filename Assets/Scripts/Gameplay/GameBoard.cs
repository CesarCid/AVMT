using System;
using System.Collections.Generic;
using UnityEngine;

namespace AVMT.Gameplay
{
    public enum Direction { Right, Left, Up, Down };

    public class GameBoard : MonoBehaviour
    {
        private static int minMatch = 3;

        public static float SlotSize = 2.56f;
        public static float SlotHalfSize = 1.28f;

        private static Vector2Int[] directionVector = new Vector2Int[4] { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
        private static Direction[] oppositeDirections = new Direction[4] { Direction.Left, Direction.Right, Direction.Down, Direction.Up };

        public enum Axis { Horizontal, Vertical }
        public static Vector2Int BoardLenght = new Vector2Int(6, 9);

        [SerializeField]
        private GameObject boardSlotPrefab;

        [SerializeField]
        private Transform boardPivot;
        public Transform BoardPivot => boardPivot;

        private BoardSlot[,] slots = new BoardSlot[BoardLenght.x, BoardLenght.y];
        public BoardSlot[,] Slots { get => slots; }

        private void Awake()
        {
            SetupSlots();
        }
        private void SetupSlots()
        {
            for (int y = 0; y < BoardLenght.y; y++)
            {
                for (int x = 0; x < BoardLenght.x; x++)
                {
                    GameObject slot = Instantiate(boardSlotPrefab, boardPivot);
                    slot.transform.localPosition = new Vector3(SlotSize * x, SlotSize * y, 0);
                    slot.name = "BoardSlot " + x + "," + y;
                    slots[x, y] = slot.GetComponent<BoardSlot>();
                    slots[x, y].index = new Vector2Int(x, y);
                }
            }
        } 

        public bool GetMatchesInColumnAfterMovement(Vector2Int slot, Direction movementDirection, out List<List<int>> matchIndexes) 
        {
            Vector2Int movedSlot = slot + GetDirectionVector(movementDirection);
            int columnToLook = movedSlot.x;

            if(columnToLook < 0 || columnToLook > BoardLenght.x)
            {
                matchIndexes = new List<List<int>>();
                return false;
            }

            GamePieceType[] column = GetColumnTypes(columnToLook);

            GamePieceType movingPieceType = slots[slot.x, slot.y].Piece.Type;

            if (GetDirectionAxis(movementDirection) == Axis.Vertical)
            {
                GamePieceType targetPieceType = column[movedSlot.y];
                column[slot.y] = targetPieceType;
            }

            column[movedSlot.y] = movingPieceType;

            return EvaluateMatch(column, out matchIndexes);
        }

        public bool GetMatchesInLineAfterMovement(Vector2Int slot, Direction movementDirection, out List<List<int>> matchIndexes)
        {
            Vector2Int movedSlot = slot + GetDirectionVector(movementDirection);
            int lineToLook = movedSlot.y;

            if (lineToLook < 0 || lineToLook > BoardLenght.y)
            {
                matchIndexes = new List<List<int>>();
                return false;
            }            

            GamePieceType[] line = GetLineTypes(lineToLook);

            GamePieceType movingPieceType = slots[slot.x, slot.y].Piece.Type;
            
            if (GetDirectionAxis(movementDirection) == Axis.Horizontal)
            {
                GamePieceType targetPieceType = line[movedSlot.x];
                line[slot.x] = targetPieceType;
            }

            line[movedSlot.x] = movingPieceType;

            return EvaluateMatch(line, out matchIndexes);
        }

        public bool GetMatchesInColumn (int columnIndex, out List<List<int>> matchIndexes)
        {
            return EvaluateMatch(GetColumnTypes(columnIndex), out matchIndexes);
        }

        public bool GetMatchesInLine(int lineIndex, out List<List<int>> matchIndexes)
        {
            return EvaluateMatch(GetLineTypes(lineIndex), out matchIndexes);
        }

        private bool EvaluateMatch(GamePieceType[] sequence, out List<List<int>> matchIndexes)
        {
            int matchCount = 1;
            matchIndexes = new List<List<int>>();

            bool matchFound = false;
            int firstFoundIndex = 0;
            GamePieceType lastFoundType = null;

            //TODO: change to a "look-ahead for" to prevent code copying outside the for
            for (int index = 0; index < sequence.Length; index++) 
            {
                if (lastFoundType == sequence[index])
                {
                    matchCount++;
                    if (firstFoundIndex == 0)
                    {
                        firstFoundIndex = index - 1;
                    }
                }
                else 
                {
                    if (matchCount >= minMatch)
                    {
                        List<int> match = new List<int>();
                        for (int matchIndex = firstFoundIndex; matchIndex < firstFoundIndex + matchCount; matchIndex++)
                        {
                            match.Add(matchIndex);
                        }
                        matchIndexes.Add(match);
                        matchFound = true;
                    }

                    matchCount = 1;
                    firstFoundIndex = 0;
                }

                lastFoundType = sequence[index];
            }

            if (matchCount >= minMatch)
            {
                List<int> match = new List<int>();
                for (int matchIndex = firstFoundIndex; matchIndex < firstFoundIndex + matchCount; matchIndex++)
                {
                    match.Add(matchIndex);
                }
                matchIndexes.Add(match);
                matchFound = true;
            }

            return matchFound;
        }

        private GamePieceType[] GetColumnTypes (int columnIndex)
        {
            int verticalLenght = slots.GetLength((int)Axis.Vertical);

            GamePieceType[] column = new GamePieceType[verticalLenght];

            for (int i = 0; i < verticalLenght; i++) 
            {
                /*if(slots[columnIndex, i].Piece == null)
                {
                    break;
                }*/

                column[i] = slots[columnIndex, i].Piece.Type;
            }

            return column;
        }

        private GamePieceType[] GetLineTypes(int lineIndex)
        {
            int horizontalLenght = slots.GetLength((int)Axis.Horizontal);

            GamePieceType[] line = new GamePieceType[horizontalLenght];

            for (int i = 0; i < horizontalLenght; i++)
            {
                /*if (slots[i, lineIndex].Piece == null)
                {
                    break;
                }*/

                line[i] = slots[i, lineIndex].Piece.Type;
            }

            return line;
        }

        public static Vector2Int GetDirectionVector (Direction direction)
        {
            return directionVector[(int)direction];
        }

        public static Direction GetOppositeDiretion(Direction direction)
        {
            return oppositeDirections[(int)direction];
        }

        public static Axis GetDirectionAxis (Direction direction)
        {
            return (direction == Direction.Right || direction == Direction.Left) ? Axis.Horizontal : Axis.Vertical;
        }
    }
}
