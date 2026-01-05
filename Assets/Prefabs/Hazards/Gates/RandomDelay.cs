using UnityEngine;

public class RandomDelay : MonoBehaviour
{
    [SerializeField] private Animator _gateAnimator;
    [SerializeField] private string _stateName = "Idle"; // animation state name

    private void Start()
    {
        if (_gateAnimator == null)
            _gateAnimator = GetComponent<Animator>();

        // Pick a random point in the animation
        float randomNormalizedTime = Random.value;

        // Play the animation at that point
        _gateAnimator.Play(_stateName, 0, randomNormalizedTime);
    }
}