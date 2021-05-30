using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GamePiece : MonoBehaviour
{
    public static float PieceSize = 2.56f;
    public static float PieceHalfSize = 1.28f;
    
    public enum PieceState { Unselected, Selected, Moving, Breaking }


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
