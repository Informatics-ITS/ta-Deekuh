using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    public string word;
    public int enemyCount;
    public int enemyHP;
}


