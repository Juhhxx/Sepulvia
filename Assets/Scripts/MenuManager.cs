using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviourSingleton<MenuManager>
{
    [SerializeField, Scene] private List<string> _noPauseScenes;
    [SerializeField] private KeyCode _pauseKey;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _optionsMenu;
    [SerializeField] private GameObject _confirmQuitMenu;
    [SerializeField] private GameObject _confirmMainMenu;

    private Animator _anim;
    private SceneChanger _sceneChanger;

    private bool _canPause = true;
    public bool CanPause { get => _canPause; set => _canPause = value; }

    private bool _optionsOpen = false;
    public bool OptionsOpen => _optionsOpen;

    // LocalizationUI _localization;

    private void Awake()
    {
        base.SingletonCheck(this, true);
    }

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _sceneChanger = FindAnyObjectByType<SceneChanger>();
        // _localization = GetComponent<LocalizationUI>();

        ResetMenus();

        SceneManager.sceneLoaded += (scene, mode) => ResetMenus();
    }

    public void ResetMenus()
    {
        PauseManager.Instance.UnPause();

        if (_anim != null)
        {
            _anim.SetTrigger("Reset");
        }

        _pauseMenu?.SetActive(false);
        _optionsMenu?.SetActive(false);
        _confirmQuitMenu?.SetActive(false);
        _confirmMainMenu?.SetActive(false);
    }

    public void Quit() => Application.Quit();

    public void LoadScene(string scene) => LoadScene(scene, null, true);
    public void LoadScene(string scene, Action onLoad = null, bool doFade = true)
    {
        _sceneChanger.ChangeScene(scene, onLoad, doFade);
    }

    public void ResetSelection() => EventSystem.current.SetSelectedGameObject(null);

    private void CheckPause()
    {
        if (!_canPause) return;

        if (_noPauseScenes.Contains(SceneManager.GetActiveScene().name)) return;

        if (Input.GetKeyDown(_pauseKey))
        {
            if (_pauseMenu.activeInHierarchy)
            {
                // Check if other menus are open
                if (_optionsMenu.activeInHierarchy) ToggleOptionsMenu(false);
                else if (_confirmMainMenu.activeInHierarchy) ToggleConfirmMainMenu(false);
                else TogglePauseMenu(false);
            }
            else TogglePauseMenu(true);
        }
    }

    public void TogglePauseMenu(bool onOff)
    {
        // AudioManager.Instance.TogglePauseAllGroups(onOff);
        ResetSelection();

        if (_anim == null)
        {
            _pauseMenu.SetActive(onOff);

            if (onOff) PauseManager.Instance.Pause();
            else PauseManager.Instance.UnPause();

            return;
        }

        _anim.ResetTrigger("Reset");

        _anim.enabled = true;

        if (onOff)  _anim.SetTrigger("OpenPause");
        else  _anim.SetTrigger("ClosePause");
    }
    public void ToggleOptionsMenu(bool onOff)
    {
        _optionsOpen = onOff;

        if (_anim == null)
        {
            _optionsMenu.SetActive(onOff);

            return;
        }

        _anim.ResetTrigger("Reset");
        _anim.enabled = true;

        // _localization.ToggleDropdown(_noPauseScenes.Contains(SceneManager.GetActiveScene().name));

        if (onOff) _anim.SetTrigger("OpenOptions");
        else _anim.SetTrigger("CloseOptions");
    }
    public void ToggleConfirmQuitMenu(bool onOff) => _confirmQuitMenu.SetActive(onOff);
    public void ToggleConfirmMainMenu(bool onOff) => _confirmMainMenu.SetActive(onOff);

    private void Update()
    {
        CheckPause();
    }
}
