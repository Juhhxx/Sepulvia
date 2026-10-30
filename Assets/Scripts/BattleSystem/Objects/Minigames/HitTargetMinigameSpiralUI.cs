using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class HitTargetMinigameSpiralUI : MonoBehaviour
{
    private HitTargetMinigame _minigameController;

    private void Awake()
    {
        _minigameController = GetComponent<HitTargetMinigame>();

        // _minigameController.OnMinigameStart += (a) => SetUpHitIndicator(_minigameController.SuccessRange);
    }
}
