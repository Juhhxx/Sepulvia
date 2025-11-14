using System;

[Serializable]
public class EnemyBattleAI
{
    private CharacterInfo _character;

    public EnemyBattleAI(CharacterInfo character)
    {
        _character = character;
    }
    
    public MoveInfo ChooseRandom()
    {
        Random rnd = new Random();
        MoveInfo move = null;

        bool ok = false;

        while (!ok)
        {
            move = _character.MoveSet[rnd.Next(_character.MoveSet.Count)];

            ok = !move.CheckIfCooldown() && move.CheckStanceCost(_character);
        }

        return move;
    }
}