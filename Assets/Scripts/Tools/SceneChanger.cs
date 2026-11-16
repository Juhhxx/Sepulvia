using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private ScreenWipeTrigger _fadeTrigger;

    public void ChangeScene(string scene, Action onLoad, bool doFade = true)
    {
        StartCoroutine(ChangeSceneCR(scene, onLoad, doFade));
    }

    private IEnumerator ChangeSceneCR(string scene, Action onLoad, bool doFade)
    {
        if (doFade) _fadeTrigger.TriggerWipe(true);

        yield return new WaitForEndOfFrame();
        if (doFade) yield return new WaitUntil(() => !_fadeTrigger.CurrentWipe.IsPlaying);

        SceneManager.LoadScene(scene);

        onLoad?.Invoke();

        if (doFade) _fadeTrigger.TriggerWipe(false);
    }

    public async Task ChangeSceneAsync(string scene, Action onLoad, bool doFade = true)
    {
        if (doFade) _fadeTrigger.TriggerWipe(true);

        await SceneManager.LoadSceneAsync(scene);

        onLoad?.Invoke();

        if (doFade) _fadeTrigger.TriggerWipe(false);
    }
}
