using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingWindow : MonoBehaviour
{
    [SerializeField] private Slider ProgressSlider = null;

    private AsyncOperation async;

    private void Awake()
    {
        if (ProgressSlider == null)
        {
            Debug.LogError("LoadingWindow: Awake: ProgressSlider == null");
            return;
        }

        Messaging.System.ChangeLevel.AddListener((level, entrypoint) => 
        {
            LevelLoader.LevelLoaded = false;
            LevelLoader.entryPoint = entrypoint;
            Messaging.GUI.CloseWindows.Invoke();
            Messaging.GUI.OpenCanvas.Invoke("");
            Messaging.GUI.ClearDynamicGUI.Invoke();

            gameObject.SetActive(true);

            StartCoroutine(LoadScene(level));
        });

        gameObject.SetActive(false);
    }

    private IEnumerator LoadScene(string sceneName)
    {
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError("LoadingWindow: LoadingScreen: Scene \"" + sceneName + "\" can not be loaded.");
            yield return null;
        }

        async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;

        while (async.isDone == false)
        {
            ProgressSlider.value = async.progress * 1.111f; //because unity shows progress from 0.0 to 0.9

            if (async.progress == 0.9f) //when scene is fully loaded
            {
                ProgressSlider.value = 1f;
                async.allowSceneActivation = true;
                gameObject.SetActive(false);
            }

            yield return null;
        }
    }
}
