using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


#region Enums
public enum CardEmblem
{
    None,
    Heart,
    Spade,
    Clover,
    Diamond
}
public enum CardType
{
    None,
    Number,
    English,
    Joker
}

#endregion

#region Card data

[Serializable]
public class CardData
{
    public Sprite sprite;
    public CardEmblem emblem;
    public CardType type;
    public int number;
    public AnimationClip aniClip;
    public int count;
}

[Serializable]
public class CardDatas
{
    #region Card datas
    private List<CardData[]> _cardList = new List<CardData[]>();
    public CardData[] diaCards;
    public CardData[] spadeCards;
    public CardData[] heartCards;
    public CardData[] cloverCards;
    public CardData[] jokerCards;
    #endregion
    
    public CardData ExtractCard()
    {
        int count = 1;
        while (count<20) // 조건을 만족할 때까지 반복
        {
            var cards = GetRandomDtList();
            var ableCard = cards.Where(card => card.count > 0).ToArray();

            if (ableCard.Length > 0)
            {
                // 랜덤으로 카드 하나 선택
                CardData card = ableCard[Random.Range(0,ableCard.Length)];
                card.count--;
                return card;
            }
            count++;
        }
        CardCountReset();
        return ExtractCard();
    }
    private CardData[] GetRandomDtList()
    {
        if (_cardList.Count == 0)
            CardListSetting();
        int ranNum = Random.Range(0, 4);
        CardData[] cards = _cardList[ranNum];
        return cards;
    }
    private void CardListSetting()
    {
        _cardList.Add(diaCards);
        _cardList.Add(spadeCards);
        _cardList.Add(heartCards);
        _cardList.Add(cloverCards);
    }
    public void CardCountReset()
    {
        if(_cardList.Count==0)
            CardListSetting();
        foreach (var dts in _cardList)
        {
            foreach (var dt in dts)
                dt.count = 1;
        }
    }

    public int CardCount()
    {
        int count = 0;

        foreach (var datas in _cardList)
        {
            foreach (var data in datas)
            {
                if(data.count==1)
                    count++;
            }
        }

        return count;
    }
}

#endregion

[CreateAssetMenu(fileName = "CardList", menuName = "SO/Card/List")]
public class CardList : ScriptableObject
{
    public CardDatas cards;

    private void OnEnable()
    {
        CardCountReset();
    }

    public CardData ExtractCard() => cards.ExtractCard();
    public void CardCountReset() => cards.CardCountReset();
    
    public int CardCount()=> cards.CardCount();
}
