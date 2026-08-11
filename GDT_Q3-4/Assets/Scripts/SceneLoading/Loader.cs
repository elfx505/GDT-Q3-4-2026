using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public static class Loader 
{   
    private class LoadingMonoBehaviour : MonoBehaviour { } // Dummy Class for Coroutine
    public enum Scene
    {
        Test,
        MainMenu,
        Loading,

    }

    private static Action onLoaderCallback;
    private static AsyncOperation loadingAsyncOperation;
    public static void Load(Scene scene)
    {   
        onLoaderCallback = () =>
        {   
            GameObject loadingGameObject = new GameObject("Loading Game Object");
            loadingGameObject.AddComponent<LoadingMonoBehaviour>().StartCoroutine(LoadSceneAsync(scene));
        };


        SceneManager.LoadScene(Scene.Loading.ToString());

    }

    private static IEnumerator LoadSceneAsync(Scene scene)
    {
        yield return null;

        loadingAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString());

        while (loadingAsyncOperation.isDone)
        {
            yield return null;
        }
    }

    public static float GetLoadingProgress()
    {
        if (loadingAsyncOperation != null)
        {
            return loadingAsyncOperation.progress;
        } else
        {
            return 1f;
        }
    }

    public static void LoaderCallback()
    {
        if (onLoaderCallback != null)
        {
            onLoaderCallback();
            onLoaderCallback = null;
        } 
    }
}
