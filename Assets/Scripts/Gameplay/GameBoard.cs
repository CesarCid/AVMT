using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    private static int minMatch = 3;

    public static float SlotSize = 2.56f;
    public static float SlotHalfSize = 1.28f;

    public enum Direction { Right, Left, Up, Down };
    public enum Coord { Horizontal, Vertical }

    public static Vector2Int BoardLenght = new Vector2Int(6, 9);

    [SerializeField]
    private GameObject boardSlotPrefab;

    [SerializeField]
    private Transform boardPivot;
    public Transform BoardPivot => boardPivot;

    private BoardSlot[,] slots = new BoardSlot[BoardLenght.x , BoardLenght.y];
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
                slots[x,y] = slot.GetComponent<BoardSlot>();
                slots[x,y].index = new Vector2Int(x, y);
            }
        }
    }

    public bool EvaluateMatch(BoardSlot slot)
    {
        int matchCount = EvaluateMatchInDirection(slot, Direction.Down);

        return matchCount >= minMatch;
    }

    private int EvaluateMatchInDirection(BoardSlot slot, Direction direction)
    {
        Coord coordinate = (direction == Direction.Right || direction == Direction.Left) ? Coord.Horizontal : Coord.Vertical;

        //slot.index.x + 1;
        int startingPoint = (coordinate == Coord.Horizontal) ? slot.index.x : slot.index.y;
        startingPoint += (direction == Direction.Right || direction == Direction.Up) ? 1 : -1;
        int endPoint = (coordinate == Coord.Horizontal) ? BoardLenght.x : BoardLenght.y;
        int slotX = slot.index.x;
        int slotY = slot.index.y;

        int matchCount = 1;

        //TODO: Left and Down are not working for i is always incrementing;
        for (int i = startingPoint; i < endPoint; i++)
        {
            slotX = (coordinate == Coord.Horizontal) ? i : slotX;
            slotY = (coordinate == Coord.Horizontal) ? slotY : i;

            if (slots[slotX, slotY].Piece.Type == slot.Piece.Type)
            {
                matchCount++;                
            }
            else
            {
                break;
            }
        }

        return matchCount;
    }

    int Increment(int i) 
    {
        return i++;
    }
}
