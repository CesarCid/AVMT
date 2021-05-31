using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AppManager;

[CreateAssetMenu(menuName = "System/Config/Piece Type", fileName = "PieceType")]
public class GamePieceType : ScriptableObject
{
    [SerializeField]
    private GameObject prefab;
    public GameObject Prefab => prefab;

    [SerializeField]
    private Sprite sprite;
    public Sprite Sprite => sprite;
}
