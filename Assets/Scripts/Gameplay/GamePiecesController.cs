using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GamePiecesController : MonoBehaviour
{    
    [SerializeField]
    private GamePieceType[] gamePieces;

    [SerializeField]
    private GameBoard gameBoard;

    private TouchController touchController;
       

    void Awake()
    {
        touchController = TouchController.Instance;

        touchController.TouchedDownOnInteractable += OnPieceTouchDown;
        touchController.TouchedUpOnInteractable += OnPieceTouchUp;
    }

    void Start() 
    {
        PopulateBoard();
    }

    public void PopulateBoard()
    {
        for (int y = 0; y < GameBoard.BoardLenght.y; y++)
        {
            for (int x = 0; x < GameBoard.BoardLenght.x; x++)
            {
                CreatePiece(GetRandomPieceType(), gameBoard.Slots[x,y]);
            }
        }
    }

    private GamePieceType GetRandomPieceType() 
    {
        return GetRandomPieceType(new GamePieceType[0]);
    }
    private GamePieceType GetRandomPieceType(GamePieceType[] exceptions)
    {
        IEnumerable<GamePieceType> availablePieces = gamePieces.Except(exceptions);

        return availablePieces.ElementAt(Random.Range(0, availablePieces.Count()));
    }


    private GameObject CreatePiece (GamePieceType type, BoardSlot slotParent)
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
            Debug.Log("[GamePiecesController] OnPieceTouchDown | Evaluate right: " + gameBoard.EvaluateMatch(slot));
        }
    }
}

