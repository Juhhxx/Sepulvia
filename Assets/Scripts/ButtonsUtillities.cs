using NaughtyAttributes;
using UnityEngine;

public class ButtonsUtillities : MonoBehaviour
{
    public void LoadScene(string scene)
    {
        MenuManager.Instance.LoadScene(scene);
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
