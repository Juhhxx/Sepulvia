using UnityEngine;
using System;

public interface IMoveMinigame
{
    public void StartMinigame(int rounds);
    public void MakeEasier();

    // Send the result of the minigame (float between 0 and 1)
    public event Action<float> OnMinigameEnd;
}
