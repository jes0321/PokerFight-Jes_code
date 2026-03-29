using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
 
[CreateAssetMenu(fileName = "EnemyListSO", menuName = "SO/Enemy/List")]
public class EnemyListSO : ScriptableObject
{
    [Serializable]
    public struct SOList
    {
        public List<EnemySO> enemies;
    }
    
    public List<SOList> soLists;

    public List<EnemySO> Bosss;
    
    public EnemySO GetEnemyList(int stageNum)
    {
        int num = stageNum / 5;
        int count = soLists[num].enemies.Count;
        return soLists[num].enemies[Random.Range(0, count)];
    }

    public EnemySO GetBossSO(int stageNum)
    {
        int num = stageNum / 5 -1;
        return Bosss[num];
    }
}
