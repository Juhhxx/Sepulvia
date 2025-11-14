using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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