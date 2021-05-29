using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

[CreateAssetMenu(menuName = "System/Config/Game Mode", fileName = "GameMode")]
public class GameMode : ScriptableObject
{
    [SerializeField]
    private Scenes correspondingScene;
    public Scenes Scene => correspondingScene;
}
