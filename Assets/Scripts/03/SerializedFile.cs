using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedFile
{
    public int coinNum;
    public int diamondNum;
    public float PosX;
    public float PosY;

    public List<float> enemyPosX;
    public List<float> enemyPosY;
    public List<bool> isDead;

    public SerializedFile(int _coinNum,int _diamonNum, float _posx,float _posy, List<float> _enemyPosX, List<float> _enemyPosY, List<bool> _isDead) {
        coinNum = _coinNum;
        diamondNum = _diamonNum;
        PosX = _posx;
        PosY = _posy;
        enemyPosX = _enemyPosX;
        enemyPosY = _enemyPosY;
        isDead = _isDead;
    }
}
