using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HandCardList", menuName = "SO/Card/Hand")]
public class HandCardList : ScriptableObject
{
    public List<CardData> datas = new List<CardData>();
    public void ResetList()=>datas.Clear();

    private void OnEnable()
    {
        ResetList();
    }
}
