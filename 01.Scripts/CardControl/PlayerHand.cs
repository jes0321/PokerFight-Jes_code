using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerHand : MonoSingleton<PlayerHand>
{
    
    [SerializeField] private TextMeshPro _cardCount;
    [SerializeField] private BoolEventChannelSO _gameStartEventChannel;
    [SerializeField] private Transform rightTrm, leftTrm;
    [SerializeField] private HandCardList _cardList;
    private List<Card> _cards = new List<Card>();
    public int CardCount => _cards.Count;
    private void Start()
    {
        _cardList.datas.Clear();
        _gameStartEventChannel.OnValueEvent += CardAlignmentEvent;
    }

    public void SetCardsPRS()
    {
        _cards.ForEach(card=>
        {
            card.MoveCompo.Prs = new PRS(
                new Vector3(card.MoveCompo.Prs.pos.x + 4, card.MoveCompo.Prs.pos.y, card.MoveCompo.Prs.pos.z)
                , card.MoveCompo.Prs.rot, card.MoveCompo.Prs.scale);
        });
    }
    private void OnDestroy()
    {
        _gameStartEventChannel.OnValueEvent -= CardAlignmentEvent;
    }

    
    [SerializeField] private SoundSO _dropSound;

    public void AddCard(Card card)
    {
        _cardList.datas.Add(card.CardData);
        _cards.Add(card);
        CardAlignment(true,!card._isAttackCard);
        card._isAttackCard = false;
        CardAlignment(true);
        FixTxt();
    }

    public void FixTxt()
    {
        _cardCount.text = $"{CardCount} / 10";
    }
    
    public void RemoveCard(Card card)
    {
        _cardList.datas.Remove(card.CardData);
        _cards.Remove(card);
        CardAlignment(true);
    }

    public void CardAlignmentEvent(bool isStart)
    {
        CardAlignment(isStart);
    }
    private void CardAlignment(bool isStart,bool isDotween=true)
    {
        if(!isStart) return;
        List<PRS> orginPRSs = new List<PRS>();
        orginPRSs = RoundAlignment(leftTrm, rightTrm,_cardList.datas.Count,0.5f,Vector3.one);

        ListAlignment();
        
        for (var index = 0; index < _cardList.datas.Count; index++)
        {
            var data = _cardList.datas[index];
            
            Card card;
            if (index>=_cards.Count)
            {
                card = PoolManager.Instance.Pop("Card") as Card;
                _cards.Add(card);
            }
            else
            {
                card = _cards[index];
            }
            card.Initialize(data,true);
            card.Renderer.SetOriginOrder(index);
            card.MoveCompo.Prs = orginPRSs[index];
            card.MoveCompo.MoveTransform(orginPRSs[index],isDotween,GameManager.Instance.moveTime);
            
            SoundPlayer player = PoolManager.Instance.Pop("SoundPlayer") as SoundPlayer;
            player.PlaySound(_dropSound);
        }
    }

    private void ListAlignment()
    {
        _cardList.datas = _cardList.datas.OrderBy(dt => dt.number).ToList();
    }

    private List<PRS> RoundAlignment(Transform leftTrm1, Transform rightTrm1, int count, float height, Vector3 scale)
    {
        float[] objLerps = new float[count];
        List<PRS> results = new List<PRS>();

        switch (count)
        {
            case 1 : objLerps = new float[] { 0.5f }; break;
            case 2 : objLerps = new float[] { 0.27f,0.73f }; break;
            case 3 : objLerps = new float[] { 0.1f,0.5f,0.9f }; break;
            default:
                float interval = 1f/(count-1);
                for(int i=0; i<count; i++)
                    objLerps[i] = interval*i;
                break;
        }

        for (int i = 0; i < count; i++)
        {
            var targetPos = Vector3.Lerp(leftTrm1.position, rightTrm1.position, objLerps[i]);
            var targetRot = Quaternion.identity;
            if (count > 3)
            {
                float curve = Mathf.Sqrt(Mathf.Pow(height,2)-Mathf.Pow(objLerps[i]-0.5f,2));
                curve = height>=0?curve:-curve;
                targetPos.y += curve;
                targetRot = Quaternion.Slerp(leftTrm.rotation, rightTrm.rotation, objLerps[i]);
            }
            results.Add(new PRS(targetPos, targetRot,scale));
        }
        return results;
    }
}
