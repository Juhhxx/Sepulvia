using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    [SerializeField] private ThresholdBarManager _playerStanceBar;

    [SerializeField] private Transform _playerPivot;
    [SerializeField] private List<PivotFormation> _enemyPivotFormations;

    [Serializable]
    public class PivotFormation
    {
        public int FormationSize;
        public List<Transform> PivotPoints;
    }

    private List<GameObject> _characterModels;

    [SerializeField] private GameObject _actionButtons;
    [SerializeField] private GameObject _moveButtons;
    [SerializeField] private GameObject _stanceMoveButtons;
    [SerializeField] private GameObject _targetButtons;

    public enum BattleUIState
    {
        Action,
        Move,
        StanceMove,
        Target,
        SelectBar,
        None
    }

    [SerializeField] private GameObject _selectBarCanvas;

    [SerializeField] private GameObject _moveInfoPanel;
    [SerializeField] private TextMeshProUGUI _panelTitle;
    [SerializeField] private TextMeshProUGUI _panelDescription;

    [SerializeField] private GameObject _battleWinTransitionScreen;
    [SerializeField] private GameObject _battleLossTransitionScreen;
    [SerializeField] private GameObject _winBattleScreen;
    [SerializeField] private GameObject _decisionBattleScreen;
    [SerializeField] private GameObject _rewardsBattleScreen;
    [SerializeField] private GameObject _rewardsShowcase;
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private GameObject _essencePrefab;
    [SerializeField] private RectTransform _decisionScreenHeart;
    private Vector3 _decisionHearthDefaultPos;
    private Vector3 _decisionHearthDefaultScale;

    private BattleManager _battleManager;

    private void Awake()
    {
        _battleManager = FindAnyObjectByType<BattleManager>();
    }

    private void Start()
    {
        _decisionHearthDefaultPos = _decisionScreenHeart.anchoredPosition;
        _decisionHearthDefaultScale = _decisionScreenHeart.localScale;

        _battleManager.Player.OnStanceChange += (_,_,_) => UpdateStanceBar(_battleManager.Player);
    }

    public void setBattleLossTransitionScreen(GameObject screen)
    {
        _battleLossTransitionScreen = screen;
    }

    public void ClearCreatedObjects()
    {
        if (_characterModels != null)
        {
            foreach (GameObject go in _characterModels) Destroy(go);
            _characterModels.Clear();
        }
    }

    public void InstantiateBattlePrefabs(PlayerParty playerParty, EnemyParty enemyParty)
    {
        _characterModels = new List<GameObject>();

        Character player = playerParty.Player;

        GameObject playerGO = Instantiate(player.BattlePrefab, _playerPivot.position, Quaternion.identity);

        BattlerController playerController = playerGO.GetComponent<BattlerController>();

        _characterModels.Add(playerGO);
        _battleManager.RegisterBattlerController(player, playerController);
        Debug.Log($"Instantiated player prefab for {player.Name}", this);

        List<Vector3> positions = GetEnemySpawnPoints(enemyParty.PartySize);

        for (int i = 0; i < enemyParty.PartySize; i++)
        {
            Character c = enemyParty.PartyMembers[i];

            GameObject enemyGO = Instantiate(c.BattlePrefab, positions[i], Quaternion.identity);

            _characterModels.Add(enemyGO);
            _battleManager.RegisterBattlerController(c, enemyGO.GetComponent<BattlerController>());
            Debug.Log($"Instantiated enemy prefab for {c.Name}", this);
        }
    }

    public List<Vector3> GetEnemySpawnPoints(int count)
    {
        foreach (PivotFormation pf in _enemyPivotFormations)
        {
            if (pf.FormationSize == count)
            {
                return pf.PivotPoints.Select(t => t.position).ToList();
            }
        }

        return null;
    }


    // UI States
    public void SetUIState(BattleUIState state)
    {
        ToggleActionButtons(state == BattleUIState.Action);
        ToggleMoveButtons(state == BattleUIState.Move);
        ToggleStanceMoveButtons(state == BattleUIState.StanceMove);
        ToggleTargetButtons(state == BattleUIState.Target);
        ToggleSelectBar(state == BattleUIState.SelectBar);
        ToggleMoveInfo(false); //Always toggle move info panel off when changing states

        // if (state == BattleUIState.None) ToggleActionButtonsDeactivated();
    }
    
    private void ToggleActionButtons(bool onOff)
    {
        _actionButtons.SetActive(onOff);
        // _battleManager.MoveButton.gameObject.SetActive(true);
        // _battleManager.StanceMoveButton.gameObject.SetActive(true);
        // _battleManager.RunButton.gameObject.SetActive(true);
    }
    private void ToggleActionButtonsDeactivated()
    {
        _actionButtons.SetActive(true);
        _battleManager.MoveButton.gameObject.SetActive(false);
        _battleManager.StanceMoveButton.gameObject.SetActive(false);
        _battleManager.RunButton.gameObject.SetActive(false);
    }
    private void ToggleMoveButtons(bool onOff) => _moveButtons.SetActive(onOff);
    private void ToggleStanceMoveButtons(bool onOff) => _stanceMoveButtons.SetActive(onOff);
    private void ToggleTargetButtons(bool onOff) => _targetButtons.SetActive(onOff);
    public void ToggleSelectBar(bool onOff) => _selectBarCanvas.SetActive(onOff);
    public void ToggleMoveInfo(bool onOff, Move move = null)
    {
        if (onOff)
        {
            _panelTitle.text = $"{move.Name}";

            _panelDescription.text = $"Recovery Cost: {move.RecoveryCost}";
            //_panelDescription.text += move.RecoveryCost == 1 ? " turn" : " turns";

            /*
            if (move.CheckIfCooldown())
            {
                int turnsLeft = move.Cooldown - move.TurnsPassed;
                _panelDescription.text += $"\nCooldown: {turnsLeft}/{move.Cooldown}";
            }
            else _panelDescription.text += $"\nCooldown: {move.Cooldown}";
            _panelDescription.text += move.Cooldown == 1 ? " turn" : " turns";
            */

            _panelDescription.text += $"\nCooldown: {move.Cooldown}";

            if (move.Type == MoveTypes.Stance) _panelDescription.text += $"\nStance Cost: {move.StanceCost} stance";

            _panelDescription.text += $"\n\n{move.Description}";

            if (move.CheckIfCooldown()) _panelTitle.text += $" (cooldown)";

            Debug.Log(_panelTitle.text);
            Debug.Log(_panelDescription.text);
        }
        
        _moveInfoPanel.SetActive(onOff);
    }

    public IEnumerator ShowWinScreen()
    {
        // Dissolve transition in
        yield return DissolveEndTransition(_battleWinTransitionScreen, -1f, 1f, 1f)
            .WaitForCompletion();     

        // Activate the menu
        _decisionScreenHeart.anchoredPosition = _decisionHearthDefaultPos;
        _decisionScreenHeart.localScale =  _decisionHearthDefaultScale;

        DoDecisionHeartDefaultAnim();
        _winBattleScreen.SetActive(true);
        _decisionBattleScreen.SetActive(true);
        _rewardsBattleScreen.SetActive(false);

        //METER AQUI A ANIMAÇÃO DA BEAD A REVELAR-SSE
        DoBeadSpawnIn();

        // Dissolve transition out
        yield return DissolveEndTransition(_battleWinTransitionScreen, 1f, -1f, .5f)
            .WaitForCompletion();

        yield return null;
    }

    public IEnumerator ShowLossScreen()
    {   
        // Dissolve transition in
        yield return DissolveEndTransition(_battleLossTransitionScreen, -1f, 1f, .5f)
            .WaitForCompletion();

        // Activate the menu
        MenuManager.Instance.ToggleGameOverMenu(true);

        // Dissolve transition out
        yield return DissolveEndTransition(_battleLossTransitionScreen, 1f, -1f, 1f)
            .WaitForCompletion();
    }

    private Tween DissolveEndTransition(GameObject battleEndTransitionScreen, float startDissolveValue, float endDissolveValue, float dissolveDuration)
    {   
        // Get the object's image component
        Image img = battleEndTransitionScreen.GetComponent<Image>();

        // Clone the image's material and apply it, so it doesn't interact with any other object's material
        Material dissolveMaterial = new Material (img.material);
        img.material = dissolveMaterial;
        
        // Set the starting dissolve value
        dissolveMaterial.SetFloat("_Dissolve_Amount", startDissolveValue);

        // Determine whether screen is fading in our out
        bool fadingIn = startDissolveValue < endDissolveValue;

        // If fading in, activate the screen
        if (fadingIn) battleEndTransitionScreen.SetActive(true);

        // Dissolve the image
        return DOTween.To(
            () => dissolveMaterial.GetFloat("_Dissolve_Amount"),
            value => dissolveMaterial.SetFloat("_Dissolve_Amount", value),
            endDissolveValue,
            dissolveDuration
        )
        .OnComplete(() =>
        {   
            // If fading out, disable the screen
            if(!fadingIn) battleEndTransitionScreen.SetActive(false);
        });
    }

    public void ShowRewardsScreen()
    {
        _decisionBattleScreen.SetActive(false);
        _rewardsBattleScreen.SetActive(true);
        DoRewardsShowAnim();
    }
    
    private List<GameObject> _rewardShowcaseObjs = new List<GameObject>();
    public void ShowRewards(List<ItemInfo> items, int essence)
    {
        if (_rewardShowcaseObjs.Count > 0)
        {
            foreach (GameObject go in _rewardShowcaseObjs) Destroy(go);
            _rewardShowcaseObjs.Clear();
        }

        foreach(ItemInfo item in items)
        {
            GameObject go = Instantiate(_itemPrefab, _rewardsShowcase.transform);
            go.SetActive(true);

            TextMeshProUGUI tmp = go.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = item.Name;

            Image img = go.GetComponentInChildren<Image>();
            img.sprite = item.Sprite;

            _rewardShowcaseObjs.Add(go);
        }

        if (essence > 0)
        {
            GameObject go = Instantiate(_essencePrefab, _rewardsShowcase.transform);
            go.SetActive(true);

            TextMeshProUGUI[] tmps = go.GetComponentsInChildren<TextMeshProUGUI>();
            tmps[1].text = $"x{essence}";

            _rewardShowcaseObjs.Add(go);
        }
    }

    public void HideFinalScreens()
    {
        _winBattleScreen.SetActive(false);
    }

    public void SetUpStanceBars(Character player)
    {        
        _playerStanceBar.SetUpBar(player.Name, "Stance", player.MaxStance);
    }
    public void UpdateStanceBar(Character player)
    {
        _playerStanceBar.UpdateFillAmout(player.CurrentStance);
    }

    // Animations

    private float _defaultMoveAmount = 25f;
    public void DoDecisionHeartDefaultAnim()
    {
        Vector3 pos = _decisionScreenHeart.anchoredPosition;

        _decisionScreenHeart.DOKill();
        _decisionScreenHeart.DOAnchorPosY(pos.y + _defaultMoveAmount, 1.5f).SetEase(Ease.InOutFlash).SetLoops(-1, LoopType.Yoyo);
    }

    private Tween _moveTween;
    private float _moveAmount = 50f;

    public void DoDecisionHearthMove(bool right)
    {
        Vector3 finalPos = _decisionHearthDefaultPos;

        if (right) finalPos.x += _moveAmount;
        else finalPos.x -= _moveAmount;

        if (_moveTween != null) _moveTween.Kill();

        _moveTween = _decisionScreenHeart.DOAnchorPosX(finalPos.x, 0.2f).SetEase(Ease.OutFlash);
    }
    public void DoDecisionHeartMoveDefault()
    {
        if (_moveTween != null) _moveTween.Kill();

        _moveTween = _decisionScreenHeart.DOAnchorPosX(_decisionHearthDefaultPos.x, 0.5f).SetEase(Ease.OutFlash);
    }

    public void DoDecisionHeartSpareAnim(Action action) //MUDAR ESTA ANIMAÇÃO PARA O QUE QUERO QUE ELE FAÇA QUANDO CLICO SPARE
    {
        _decisionScreenHeart.DOKill();

        _beadTransform.DOKill();
        DoBeadSpawnOut();
        DoButtonSpawnOut();

        //OLD STUFF
        // _decisionScreenHeart.DOAnchorPos(_decisionHearthDefaultPos, 0.2f).SetEase(Ease.OutFlash).OnComplete(() =>
        
        //     _decisionScreenHeart.DOAnchorPosY(_decisionHearthDefaultPos.y + 1000f, 2f).SetEase(Ease.InExpo).OnComplete(() => action())
        // );
    }

    private float _assimilationScale = 100f;
    public void DoDecisionHeartAssimilateAnim(Action action) //MUDAR ESTA ANIMAÇÃO PARA O QUE QUERO QUE ELE FAÇA QUANDO CLICO ASSIMILATE
    {
        _decisionScreenHeart.DOKill();
        _decisionScreenHeart.DOShakeAnchorPos(10f, 50f);

        _beadTransform.DOKill();
        DoBeadSpawnOut();
        DoButtonSpawnOut();

        //OLD STUFF
        _decisionScreenHeart.DOAnchorPos(_decisionHearthDefaultPos, 0.2f).SetEase(Ease.OutFlash).OnComplete(() =>

            _decisionScreenHeart.DOScale(Vector3.one * _assimilationScale, 2f).SetEase(Ease.InExpo).OnComplete(() => action())

        );
    }

    public void DoRewardsShowAnim()
    {
        _rewardsBattleScreen.transform.localScale = Vector3.zero;
        _rewardsBattleScreen.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }


    [SerializeField] private Transform _beadTransform;
    [SerializeField] private float _rotationSpeed = 1f;
    [SerializeField] private float _spawnSpeed = 1f;
    [SerializeField] private GameObject _decisionButtons;
    [SerializeField] private GameObject _beadRenderObject;
    private bool _wasAlreadyHoveringRight = false;
    private bool _wasAlreadyHoveringLeft = false;
    private bool _isBeadRevealed = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void DoBeadSpawnIn()
    {
        _beadRenderObject.SetActive(true);
        foreach(Transform child in _decisionButtons.transform)
        {
            child.gameObject.SetActive(false);
        }

        _beadTransform.localScale = Vector3.zero;

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(_beadTransform.DOScale(Vector3.one, _spawnSpeed).SetEase(Ease.Linear));
        mySequence.OnComplete(() =>
        {
            //DoBeadAwakening();
            DoButtonSpawnIn();
        });

        mySequence.Play();
        //_beadTransform.localScale = Vector3.zero;
        //_beadTransform.DOScale(Vector3.one, _spawnSpeed).SetEase(Ease.OutBack);
    }
    public void DoBeadSpawnOut()
    {
        _beadTransform.localScale = Vector3.one;
        _beadTransform.DOScale(Vector3.zero, _spawnSpeed).SetEase(Ease.InBack);
    }
    public void DoButtonSpawnIn()
    {
        foreach(Transform child in _decisionButtons.transform)
        {
            child.gameObject.SetActive(true);
            Vector3 scale = child.localScale;
            child.localScale = Vector3.zero;
            child.DOScale(scale, _spawnSpeed).SetEase(Ease.OutBack);
        }
    }

    public void DoButtonSpawnOut()
    {
        foreach(Transform child in _decisionButtons.transform)
        {
            child.DOKill();
            child.gameObject.SetActive(true);
            Vector3 scale = child.localScale;   
            child.DOScale(Vector3.zero, _spawnSpeed).SetEase(Ease.InBack);
        }
    }
    public void DoButtonHoverExpandLeft()
    {
        if(!_wasAlreadyHoveringLeft)
        {
            
        }
    }
    public void DoBeadAwakening()
    {
        _beadTransform.rotation = Quaternion.Euler(0, 0, 0);
        Quaternion targetRotation = Quaternion.Euler(180,0,0);
        //_beadTransform.DORotateQuaternion(targetRotation, 1f).SetEase(Ease.InOutBack);
        _beadTransform.DORotate(new Vector3(180, 0, 0), _rotationSpeed).SetEase(Ease.InOutBack);

        foreach(Transform child in _decisionButtons.transform)
        {
            child.gameObject.SetActive(true);
        }
    }
    public void DoNormalRotationRight()
    {
        if (!_isBeadRevealed)
        {
            DoBeadAwakening();
            _isBeadRevealed = true;
            return;
        }

        if (_wasAlreadyHoveringRight)
        {
            return;
        }
        _beadTransform.DOKill();
        
        _beadTransform.DORotate(new Vector3(540, 0, 0), _rotationSpeed, RotateMode.FastBeyond360).SetEase(Ease.InOutBack);
        _wasAlreadyHoveringRight = true;
        _wasAlreadyHoveringLeft = false;
    }
    public void DoNormalRotationLeft()
    {
        if (!_isBeadRevealed)
        {
            DoBeadAwakening();
            _isBeadRevealed = true;
            return;
        }
        if (_wasAlreadyHoveringLeft)
        {
            return;
        }
        _beadTransform.DOKill();
        
        _beadTransform.DORotate(new Vector3(-540, 0, 0), _rotationSpeed, RotateMode.FastBeyond360).SetEase(Ease.InOutBack);
        _wasAlreadyHoveringLeft = true;
        _wasAlreadyHoveringRight = false;
    }

    private IEnumerator DoBeadSpawnInCoroutine()
    {
        DoBeadSpawnIn();
        yield return null;
    }

}
