using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameMode;

public class AppManager : MonoBehaviour
{
    private static AppManager am;
    public static AppManager Instance => am;

    public enum Scenes { System, Menu, Gameplay }
    
    private GameMode activeMode;
    public GameMode ActiveMode => activeMode;

    [SerializeField]
    private GameMode startingMode;

    private void Awake()
    {
        if (am == null)
            am = this;
    }

    void Start()
    {
        LoadMode(startingMode);
    }

    public void LoadMode(GameMode mode) 
    {
        if (mode == activeMode)
            return;
        
        if (activeMode != null)
            UnloadMode(activeMode);

        LoadScene(mode.Scene);
        activeMode = mode;
    }

    private void UnloadMode(GameMode mode) 
    {
        UnloadScene(mode.Scene);
    }

    private void LoadScene (Scenes scene)
    {
        int sceneIndex = (int)scene;

        if (SceneManager.GetSceneByBuildIndex(sceneIndex).isLoaded)
            return;

        SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
    }

    private void UnloadScene(Scenes scene)
    {
        int sceneIndex = (int)scene;

        if (SceneManager.GetSceneByBuildIndex(sceneIndex).isLoaded == false)
            return;

        SceneManager.UnloadSceneAsync(sceneIndex);
    }
}
