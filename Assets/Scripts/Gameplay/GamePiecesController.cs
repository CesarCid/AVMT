using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiecesController : MonoBehaviour
{
    TouchController touchController;

    private static Vector2Int BoardCells = new Vector2Int(6, 8);

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

    void PopulateBoard() 
    { 

    }


    private void OnPieceTouchDown(GameObject go) 
    {
        Debug.Log("[GamePiecesController] OnPieceTouchDown | piece: " + go.name);
    }

    private void OnPieceTouchUp(GameObject go) 
    {
        Debug.Log("[GamePiecesController] OnPieceTouchUp | piece: " + go.name);
    }
}

