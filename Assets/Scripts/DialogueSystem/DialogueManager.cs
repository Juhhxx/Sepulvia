using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviourSingleton<DialogueManager>
{
    [SerializeField] private GameObject _dialogueBox;
    [SerializeField] private GameObject _nextArrow;
    [SerializeField] private TextMeshProUGUI _dialogueTextBox;
    [SerializeField] private float _textSpeed;
    [SerializeField][InputAxis] private string _jumpKey;

    private Queue<DialogueEvent> _dialogueQueue;
    private YieldInstruction _wfs;
    private YieldInstruction _wff;
    private WaitForKeyDown _wfk;
    private bool _skipDialogue = false;
    private bool _dialoguePlaying;
    private bool _fillingDialogue = false;

    public void Awake()
    {
        base.SingletonCheck(this, false);
    }

    private void Update()
    {
        CheckDialogEnd();
        CheckForSkip();
    }

    public void SetUpDialogueManager()
    {
        _dialogueQueue = new Queue<DialogueEvent>();
        _wfs = new WaitForSeconds(_textSpeed);
        _wff = new WaitForEndOfFrame();
        _wfk = new WaitForKeyDown(_jumpKey);
        _dialogueBox.SetActive(false);
    }

    public void AddDialogue(string dialogue, Action action = null)
    {
        DialogueEvent d = new DialogueEvent(dialogue, action);
        _dialogueQueue.Enqueue(d);
    }
    public void ClearDialogues()
    {
        StopAllCoroutines();
        _dialogueQueue.Clear();
        _dialoguePlaying = false;
    }
    public void StartDialogues()
    {
        StopAllCoroutines();
        StartCoroutine(PlayDialogues());
    }
    public void StartDialogues(string dialogue)
    {
        StopAllCoroutines();
        AddDialogue(dialogue);
        StartCoroutine(PlayDialogues());
    }
    public bool CheckDialogEnd()
    {
        return !_dialoguePlaying;
    }

    public void HideDialogue()
    {
        StopAllCoroutines();
        _dialogueBox.SetActive(false);
        _dialoguePlaying = false;
    }

    private void CheckForSkip()
    {
        if (Input.GetButtonDown(_jumpKey) && _fillingDialogue)
        {
            _skipDialogue = true;
        }
    }

    private IEnumerator PlayDialogues()
    {
        _dialogueBox?.SetActive(true);
        _dialoguePlaying = true;

        int size = _dialogueQueue.Count;
        int i = 0;

        while (i < size)
        {
            _nextArrow?.SetActive(false);

            // Dequeue next dialogue
            (string dialogue, Action action) = _dialogueQueue.Dequeue().GetValues();

            // Invoke dialogue acion, if it exists
            action?.Invoke();

            // Reset Text Box
            _dialogueTextBox.text = "";

            _fillingDialogue = true;

            foreach (char c in dialogue)
            {
                if (_skipDialogue)
                {
                    _skipDialogue = false;
                    _dialogueTextBox.text = dialogue;
                    break;
                }
                
                yield return _wfs; // Wait For Seconds 

                // Add letter
                _dialogueTextBox.text += c;
            }

            _fillingDialogue = false;
            _skipDialogue = false;
            
            // Show next dialogue arrow
            _nextArrow?.SetActive(true);
            
            yield return _wff; // Wait For End of Frame
            yield return _wfk; // Wait For Key Pressed
            yield return _wff; // Wait For End of Frame

            i++;
        }

        _dialoguePlaying = false;
        _dialogueBox?.SetActive(false);
    }
}
