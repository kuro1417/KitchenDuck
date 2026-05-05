using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        MainMenuScenes,
        GameScenes,
        LoadingScenes,
        LobbyScences,
        CharacterSelectScenes
    }

    private static Scene targetScene;

    public static void Load(Scene targetScene)
    {
        Loader.targetScene = targetScene;

        SceneManager.LoadScene(Scene.LoadingScenes.ToString());  
    }

    public static void LoadNetwork(Scene targetScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }

    public static void LoaderCallBack()
    {
         SceneManager.LoadScene(targetScene.ToString());
    }

    public static string GetCurrentScene()
    {
        return targetScene.ToString();
    }
}
