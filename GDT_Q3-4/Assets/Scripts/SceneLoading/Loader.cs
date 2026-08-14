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
            // Important: Tell this dummy object not to destroy itself instantly if things shift
            UnityEngine.Object.DontDestroyOnLoad(loadingGameObject);
            loadingGameObject.AddComponent<LoadingMonoBehaviour>().StartCoroutine(LoadSceneAsync(scene));
        };

        SceneManager.LoadScene(Scene.Loading.ToString());
    }

    private static IEnumerator LoadSceneAsync(Scene scene)
    {
        // Wait to give loading scene some time to load
        yield return new WaitForSecondsRealtime(0.3f);

        Application.backgroundLoadingPriority = ThreadPriority.Low;

        loadingAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString());

        loadingAsyncOperation.allowSceneActivation = false;

        while (!loadingAsyncOperation.isDone)
        {
            yield return null;
        }

        // Reset priority back to normal once loading is finished
        Application.backgroundLoadingPriority = ThreadPriority.Normal;
    }

    public static float GetLoadingProgress()
    {
        if (loadingAsyncOperation != null)
        {
            return Mathf.Clamp01(loadingAsyncOperation.progress / 0.9f);
        } 
        else
        {
            // Return 0 instead of 1 so the bar starts empty, not full
            return 0f;
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

    public static void ActivateLoadedScene()
    {
        if (loadingAsyncOperation != null)
        {
            loadingAsyncOperation.allowSceneActivation = true;
        }
    }

}