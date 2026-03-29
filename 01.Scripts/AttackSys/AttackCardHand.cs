using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AttackCardHand : MonoSingleton<AttackCardHand>
{
    [SerializeField] private int _index=0;
    [SerializeField] private List<Material> colors = new List<Material>();
    [SerializeField] private AttackField _emblemField;
    
    public List<Card> _cards = new List<Card>();
    private List<AttackBtn> _btns = new List<AttackBtn>();
    [SerializeField] private BoolEventChannelSO _playerTurnEndChannel;
    [SerializeField] private BoolEventChannelSO _roundEndChannel;
    private List<AttackField> _fields = new List<AttackField>();
    protected override void Awake()
    {
        base.Awake();
        
        _btns = GetComponentsInChildren<AttackBtn>().ToList();
        _fields = GetComponentsInChildren<AttackField>().ToList();
        _playerTurnEndChannel.OnValueEvent += HandReset;
        _roundEndChannel.OnValueEvent += EndRound;
    }

    private void OnDestroy()
    {
        _playerTurnEndChannel.OnValueEvent -= HandReset;
        _roundEndChannel.OnValueEvent -= EndRound;
    }

    private void HandReset(bool obj)
    {
        if (!obj) return;
        
        _index = 4;
        _emblemField.OnOffMat(colors[_index],_index);
        _cards.ForEach(card=>PoolManager.Instance.Push(card));
        _cards.Clear();
        _btns.ForEach(btn=>btn.OnOffBtn(false));
        _fields.ForEach(field => field.isEmpty = true);
    }

    private void EndRound(bool obj)
    {
        if (!obj) return;

        HandReset(true);
        _btns.ForEach(btn => btn.OnOffVisible(true));
    }

    /// <summary>
    /// 카드 놓아 져있던거 취소 눌러서 빼면 실행
    /// </summary>
    /// <param name="card"></param>
    /// <param name="isEmblem"></param>
    public void RemoveCard(Card card, bool isEmblem = false)
    {
        CardData data = card.CardData;
        
        _cards.Remove(card);

        if (isEmblem)
            ApplyEmblemEffect(data.emblem,false);
        
        CheckBtns();
    }
    
    /// <summary>
    /// 카드 어택에 두면 실행
    /// </summary>
    /// <param name="card">추가된 카드</param>
    /// <param name="isEmblem">엠블럼인지 아닌지</param>
    public void AddCard(Card card,bool isEmblem = false)
    {
        CardData data = card.CardData;
        
        _cards.Add(card);

        if (isEmblem)
            ApplyEmblemEffect(data.emblem,true);
        
        CheckBtns();
    }

    private void CheckBtns()
    {
        _btns.Where(btn=>btn.IsCardCount(_cards.Count)).ToList().ForEach(btn =>
        {
            btn._cardCount = _cards.Count;
            btn.CheckAttackHand(_cards);
        });
        _btns.Where(btn=>!btn.IsCardCount(_cards.Count)).ToList().ForEach(btn =>
        {
            btn._cardCount = _cards.Count;
            btn.OnOffBtn(false);
        });
    }

    public Action emblemEffect= null;
    public bool isDia = false;
    public bool isSpade = false;
    private void ApplyEmblemEffect(CardEmblem emblem,bool isApply)
    {
        switch (emblem)
        {
            case CardEmblem.Clover:
                int num = Random.Range(0, 3);
                if(num == 0)
                    DiaApply();
                else if(num == 1)
                    HeartApply();
                else if(num == 2)
                    SpadeApply();
                _index = 0;
                break;
            case CardEmblem.Diamond :
                DiaApply();
                break;
            case CardEmblem.Heart:
                HeartApply();
                break;
            case CardEmblem.Spade :
                SpadeApply();
                _index = 3;
                break;
        }

        if (!isApply)
        {
            emblemEffect = null;
            isSpade = false;
            isDia = false;
            _index = 4;
        }
        _emblemField.OnOffMat(colors[_index],_index);
    }

    private void SpadeApply()
    {
        isSpade = true;
        emblemEffect = null;
    }

    private void HeartApply()
    {
        emblemEffect = ()=>CombatManager.Instance.HealPlayer();
        _index = 2;
    }

    private void DiaApply()
    {
        emblemEffect = null;
        isDia = true;
        _index = 1;
    }
}
