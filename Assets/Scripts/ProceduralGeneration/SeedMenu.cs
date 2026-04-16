using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SeedMenu : MonoBehaviour
{
    [SerializeField] TMP_InputField _seedInputField;
    [SerializeField] private TextMeshProUGUI _warningText;

    private void Start()
    {
        _warningText.text = "";

        Color color = _warningText.color;
        color.a = 0f;
        _warningText.color = color;
    }

    public void VerifySeedInput()
    {
        string seedInput = _seedInputField.text;

        string result = SeedManager.Instance.SetSeed(seedInput);

        if (result != "")
        {
            StartCoroutine(ShowWarningCR(result));
        }
        else
        {
            MenuManager.Instance.LoadScene("MainGame");
        }
    }

    private IEnumerator ShowWarningCR(string message)
    {
        _warningText.text = message;
        Color c = _warningText.color;
        c.a = 1f;
        _warningText.color = c;


        while (_warningText.color.a >= 0f)
        {
            Color color = _warningText.color;
            color.a -= Time.deltaTime;
            _warningText.color = color;

            yield return null;
        }
    }
}
