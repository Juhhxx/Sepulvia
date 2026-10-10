using UnityEngine;
using UnityEngine.UI;

public class OnEnableActivateButton : MonoBehaviour
{
    private Button button;

    private void OnEnable()
    {
        if (button == null) button = GetComponent<Button>();
        button.interactable = true;
    }
}
