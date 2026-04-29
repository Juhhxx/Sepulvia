using System;

[Serializable]
public class EnemyBattleAI
{
    private Character _character;

    public EnemyBattleAI(Character character)
    {
        _character = character;
    }
    
    public Move ChooseRandom()
    {
        Random rnd = new Random();
        Move move = null;

        bool ok = false;

        while (!ok)
        {
            move = _character.MoveSet[rnd.Next(_character.MoveSet.Count)];

            ok = !move.CheckIfCooldown() && move.CheckStanceCost(_character);
        }

        return move;
    }
}