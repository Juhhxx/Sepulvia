using UnityEngine;

[RequireComponent(typeof(ButtonEventsUtility))]
public class ButtonSFXManager : MonoBehaviour
{
    private ButtonEventsUtility _buttonsUtility;

    [SerializeField] private PlaySound _buttonDownSound;
    [SerializeField] private PlaySound _buttonUpSound;
    [SerializeField] private PlaySound _buttonEnterSound;
    [SerializeField] private PlaySound _buttonExitSound;
    [SerializeField] private PlaySound _buttonHoverSound;
    [SerializeField] private PlaySound _buttonHoldSound;

    private void Awake()
    {
        _buttonsUtility = GetComponent<ButtonEventsUtility>();
    }

    private void Start()
    {
        _buttonsUtility.OnButtonDown += () => ToggleSound(true, _buttonDownSound);
        _buttonsUtility.OnButtonUp += () => ToggleSound(true, _buttonUpSound);

        _buttonsUtility.OnButtonEnter += () => ToggleSound(true, _buttonEnterSound);
        _buttonsUtility.OnButtonExit += () => ToggleSound(true, _buttonExitSound);

        _buttonsUtility.OnButtonHover += (hover) => ToggleSound(hover, _buttonHoverSound);
        _buttonsUtility.OnButtonHold += (hold) => ToggleSound(hold, _buttonHoldSound);
    }

    private void ToggleSound(bool on, PlaySound sound)
    {
        if (on) sound?.SoundPlay();
        else sound?.Stop();
    }
}
