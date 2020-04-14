using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoChangeScene : MonoBehaviour
{
    [SerializeField] private string SceneToLoad = "MainMenu";
    [SerializeField] private float WaitTime = 1f;

    void Start() =>
        StartCoroutine(LoadSceneAfterTime());

    IEnumerator LoadSceneAfterTime()
    {
        yield return new WaitForSeconds(WaitTime);

        SceneManager.LoadScene(SceneToLoad);
    }
}
