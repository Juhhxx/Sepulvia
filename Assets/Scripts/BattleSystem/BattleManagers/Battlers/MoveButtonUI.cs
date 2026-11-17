using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class MoveButtonUI : MonoBehaviour, ITooltipable, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private Image _iconImageBW;
    [SerializeField] private Image _frameImage;
    [SerializeField] private Image _frameSelectImage;
    [SerializeField] private GameObject _cooldownIndicator;
    [SerializeField] private Image _cooldownImage;
    [SerializeField] private TextMeshProUGUI _cooldownTMP;

    private MoveButton _moveButton;

    private void UpdateButton(Move move)
    {
        _iconImage.sprite = move.Icon;
        _iconImageBW.sprite = move.Icon;
    }

    private void UpdateCooldown(Move move)
    {
        _cooldownIndicator.SetActive(move.CheckIfCooldown());
        _iconImageBW.gameObject.SetActive(move.CheckIfCooldown());

        if (move.CheckIfCooldown())
        {
            int turns = move.Cooldown - move.TurnsPassed;

            _cooldownTMP.text = turns.ToString();
            _cooldownImage.fillAmount = turns / (float)move.Cooldown;
            _iconImageBW.fillAmount = turns / (float)move.Cooldown;
        }
    }

    private void UpdateStanceCost(Move move, Character player)
    {   
        // If the move has already been checked to NOT be on cooldown, check if the player has enough Stance to cast it
        if (!move.CheckIfCooldown()) {
            bool result = player.CurrentStance < move.StanceCost;
            _iconImageBW.gameObject.SetActive(result);

            // Refills the image, in case last combat ended with stance moves on cooldown
            if(result) _iconImageBW.fillAmount = 1;
        }
    }

    private void Awake()
    {
        _moveButton = GetComponent<MoveButton>();

        if (_moveButton != null)
        {
            _moveButton.OnMoveSetUp += UpdateButton;
            _moveButton.OnMoveCooldown += UpdateCooldown;
            _moveButton.OnMoveStanceCost += UpdateStanceCost;
        }
    }

    private void OnEnable()
    {
        _frameSelectImage.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _frameSelectImage.enabled = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        _frameSelectImage.enabled = false;
    }

    public TooltipInfo GetTooltipInfo()
    {
        // Get move
        Move move = _moveButton.Move;

        // Apply header info
        string header = $"{move.Name}";

        // if (move.CheckIfCooldown()) header += $" (in cooldown)";

        // Apply content info (RC, CD, SC and Description)
        string content = "";

        // Get the values for the recovery cost
        int recoveryCost = move.RecoveryCost;

        if (move.MoveLogic is MoveCombo mc)
        {
            recoveryCost = mc.GetTotalRecoveryCost();
        }
        
        content += $"Recovery Cost: {recoveryCost}";
        content += $"\nCooldown: {move.Cooldown}";

        if (move.Type == MoveTypes.Stance) content += $"\nStance Cost: {move.StanceCost}";

        var description = move.Description;

        // Get the values for the pull bonus
        string playerPullStr = "";
        int strBonus = _moveButton.PlayerController?.Character.PullStrenghtBonus ?? 0;

        if (_moveButton.PlayerController != null && strBonus != 0)
        {
            if (strBonus > 0) playerPullStr = "<color=green>(+" + strBonus + ")</color>";
            else playerPullStr =  "<color=red>(" + strBonus + ")</color>";
        }

        description = description.Replace("(pull_str)", $"{playerPullStr}");

        content += $"\n\n{description}";

        // Get additional tooltips info
        List<TooltipInfo> additional = new List<TooltipInfo>();

        if (_moveButton.PlayerController != null && strBonus != 0 && move.MoveLogic is MovePull)
        {
            foreach (StatusEffect se in _moveButton.PlayerController.StatusEffectManager.ActiveStatusEffects)
            {
                if (se.StatusEffectLogic is StatusEffectPullStrenghtBoost sb)
                {
                    string h = sb.Boost > 0 ? "<color=green>STRENGTH BUFF</color>" :
                                            "<color=red>STRENGTH NERF</color>";
                    
                    string sign = sb.Boost > 0 ? "+" : "";
                    
                    string c = $"{sign}{sb.Boost} Pull Strength from {se.Name}";

                    additional.Add(new TooltipInfo(h, c));
                }
            }
        }

        foreach (IDescriptable d in move.AdditionalDescriptions)
        {
            additional.Add(new TooltipInfo(d.GetName(),
                                        d.GetDescription()));
        }

        return new TooltipInfo(header, content, additional.ToArray());
    }
}
