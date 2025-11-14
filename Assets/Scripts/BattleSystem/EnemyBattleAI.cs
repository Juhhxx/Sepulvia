using System;

[Serializable]
public class EnemyBattleAI
{
    public EnemyBattleAI()
    {

    }
    
    public int ChooseRandom(int numberOfMoves)
    {
        Random rnd = new Random();
        return rnd.Next(numberOfMoves);
    }
}