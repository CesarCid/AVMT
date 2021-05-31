using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardSlot : MonoBehaviour
{
    [SerializeField]
    private GamePiece piece;
    public GamePiece Piece 
    { 
        get => piece; 
        set 
        {
            if (piece != null)
                return;

            piece = value;
        } 
    }

    public Vector2Int index = default;

    private void Awake()
    {

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
