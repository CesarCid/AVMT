using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeButton : MonoBehaviour
{
    [SerializeField]
    private GameMode gameMode = default;

    public void LoadGameMode()
    {
        GameManager.Instance.LoadMode(gameMode);
    }
}
