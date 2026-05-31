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
        int maxIteration = 5;
        int iteration = 0;

        while (!ok || iteration == maxIteration)
        {
            move = _character.MoveSet[rnd.Next(_character.MoveSet.Count)];

            ok = !move.CheckIfCooldown() && move.CheckStanceCost(_character);

            iteration++;
        }

        if (move == null)
        {
            UnityEngine.Debug.Log("MAX ITERATION REACHED, CHOOSING FIRST AVAILABLE MOVE");
            move = _character.MoveSet[0];
        }

        return move;
    }
}