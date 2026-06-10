using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ButtonsUtillities : MonoBehaviour
{
    [SerializeField] private Button _loadSaveButton;

    public void Start()
    {
        if (_loadSaveButton != null) _loadSaveButton.interactable = SaveManager.Instance.SaveExists;
    }
    public void LoadScene(string scene)
    {
        MenuManager.Instance.LoadScene(scene);
    }

    public void LoadSave()
    {
        MenuManager.Instance.LoadSave();
    }

    public void OpenOptions()
    {
        MenuManager.Instance.ToggleOptionsMenu(true);
    }

    public void Quit()
    {
        MenuManager.Instance.ToggleConfirmQuitMenu(true);
    }

    public void ResetSelection()
    {
        MenuManager.Instance.ResetSelection();
    }

    // public void OpenCredits()
    // {
    //     CreditsManager.Instance.OpenCredits();
    // }
}
