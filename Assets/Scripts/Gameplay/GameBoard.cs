using System;
using System.Collections.Generic;
using UnityEngine;

namespace AVMT.Gameplay
{
    public enum Direction { Right, Left, Up, Down };

    public class GameBoard : MonoBehaviour
    {
        private const int GameMatchCount = 3;

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
        public BoardSlot[,] Slots => slots;

        //public class BoardSlots
        //{
        //    private BoardSlot[,] slots = new BoardSlot[BoardLenght.x, BoardLenght.y];
        //    private BoardSlot[] setupSlots = new BoardSlot[BoardLenght.x];

        //    public BoardSlot this[int index1, int index2]
        //    {
        //        get 
        //        { 
        //            if (index2 == BoardLenght.y)
        //            {
        //                return setupSlots[index1];
        //            }

        //            return slots[index1, index2]; 
        //        }
        //        set { slots[index1, index2] = value; }
        //    }
        //}

        [SerializeField]
        public HashSet<int> dirtyLines = new HashSet<int>();
        [SerializeField]
        public HashSet<int> dirtyColumns = new HashSet<int>();

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

        public bool UpdateAvailableMoves()
        {
            ClearAllAvailableMoves();

            bool anyAvailableMove = false;
            for (int y = 0; y < BoardLenght.y; y++)
            {
                for (int x = 0; x < BoardLenght.x; x++)
                {
                    Vector2Int currentSlot = new Vector2Int(x, y);

                    foreach (Direction direction in Enum.GetValues(typeof(Direction)))
                    {
                        if (GetMatchesInLineAfterMovement(currentSlot, direction, out List<List<int>> lineMatches) ||
                            GetMatchesInColumnAfterMovement(currentSlot, direction, out List<List<int>> columnMatches))
                        {
                            Slots[currentSlot.x, currentSlot.y].SetAvailableMove(direction);
                            Vector2Int movedSlot = currentSlot + GetDirectionVector(direction);
                            Slots[movedSlot.x, movedSlot.y].SetAvailableMove(GetOppositeDiretion(direction));

                            anyAvailableMove = true;
                        }
                    }
                }
            }
            return anyAvailableMove;
        }

        private void ClearAllAvailableMoves()
        {
            for (int y = 0; y < BoardLenght.y; y++)
            {
                for (int x = 0; x < BoardLenght.x; x++)
                {
                    Slots[x, y].ClearAvailableMoves();
                }
            }
        }

        public bool GetMatchesInColumnAfterMovement(Vector2Int slot, Direction movementDirection, out List<List<int>> matchIndexes) 
        {
            if (IsMovementValid(slot, movementDirection) == false)
            {
                matchIndexes = new List<List<int>>();
                return false;
            }

            Vector2Int movedSlot = slot + GetDirectionVector(movementDirection);

            GamePieceType[] column = GetColumnTypes(movedSlot.x);

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
            if (IsMovementValid(slot, movementDirection) == false)
            {
                matchIndexes = new List<List<int>>();
                return false;
            }            

            Vector2Int movedSlot = slot + GetDirectionVector(movementDirection);

            GamePieceType[] line = GetLineTypes(movedSlot.y);

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

        private bool EvaluateMatch(GamePieceType[] sequence, out List<List<int>> matchIndexes, int minMatchCount = GameMatchCount)
        {
            matchIndexes = new List<List<int>>();

            bool matchFound = false;

            for (int index = 0; index < sequence.Length; index++)
            {
                if (sequence[index] == null)
                    break;

                int lookAheadIndex;
                GamePieceType foundType = sequence[index];
                List<int> match = new List<int>();
                match.Add(index);

                for (lookAheadIndex = index + 1; lookAheadIndex < sequence.Length;)
                {
                    if (foundType != sequence[lookAheadIndex])
                    {
                        break;
                    }

                    match.Add(lookAheadIndex);
                    lookAheadIndex++;
                }

                if (match.Count >= minMatchCount)
                {
                    matchIndexes.Add(match);
                    matchFound = true;
                    index = lookAheadIndex;
                }
            }

            return matchFound;
        }

        #region aux
        private GamePieceType[] GetColumnTypes (int columnIndex)
        {
            GamePieceType[] column = new GamePieceType[BoardLenght.y];

            for (int i = 0; i < BoardLenght.y; i++) 
            {
                if(slots[columnIndex, i].Piece == null)
                {
                    break;
                }

                column[i] = slots[columnIndex, i].Piece.Type;
            }

            return column;
        }

        private GamePieceType[] GetLineTypes(int lineIndex)
        {
            GamePieceType[] line = new GamePieceType[BoardLenght.x];

            for (int i = 0; i < BoardLenght.x; i++)
            {
                if (slots[i, lineIndex].Piece == null)
                {
                    break;
                }

                line[i] = slots[i, lineIndex].Piece.Type;
            }

            return line;
        }

        public void Break(BoardSlot slot)
        {
            slot.Break();
            SetDirty(slot);
        }

        public void SetDirty(BoardSlot slot)
        {
            dirtyColumns.Add(slot.index.x);
            dirtyLines.Add(slot.index.y);
        }

        public void ClearDirty()
        {
            dirtyColumns.Clear();
            dirtyLines.Clear();
        }

        public static Vector2Int GetDirectionVector (Direction direction)
        {
            return directionVector[(int)direction];
        }

        public static Direction GetDirectionFromVector(Vector2Int vector)
        {
            if (Math.Abs(vector.x) > 0)
            {
                if (vector.x > 0)
                {
                    return Direction.Right;
                }
                else
                {
                    return Direction.Left;
                }
            }
            else
            {
                if (vector.y > 0)
                {
                    return Direction.Up;
                }
                else
                {
                    return Direction.Down;
                }
            }
        }

        public static Direction GetOppositeDiretion(Direction direction)
        {
            return oppositeDirections[(int)direction];
        }

        public static Axis GetDirectionAxis (Direction direction)
        {
            return (direction == Direction.Right || direction == Direction.Left) ? Axis.Horizontal : Axis.Vertical;
        }

        private static bool IsMovementValid (Vector2Int slot, Direction movementDirection)
        {
            Vector2Int movedSlot = slot + GetDirectionVector(movementDirection);

            if (movedSlot.x < 0 || movedSlot.x >= BoardLenght.x || movedSlot.y < 0 || movedSlot.y >= BoardLenght.y)
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}
