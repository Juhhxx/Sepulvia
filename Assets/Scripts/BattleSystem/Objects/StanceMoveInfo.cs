using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "StanceMove", menuName = "Battle System/New Stance Move")]
public class StanceMoveInfo : MoveBaseInfo
{
    [field: Space(10)]
    [field: Header("Bar Modifier Move Parameters")]
    [field: Space(5)]
    [field: ShowIf("Type", StanceMoveType.BarModifier)]
    [field: SerializeField] public BarModifier Modifier { get; private set; }

    public StanceMove Instantiate()
    {
        return new StanceMove(this);
    }
}

public class StanceMove : MoveBase
{
    public StanceMove(StanceMoveInfo info): base(info)
    {
        Modifier = info.Modifier;
    }

    [field: SerializeField, ReadOnly] public BarModifier Modifier { get; private set; }
    [field: SerializeField, ReadOnly] public int BarSection { get; private set; }
    public void SetBarSection(int index) => BarSection = index;
}
