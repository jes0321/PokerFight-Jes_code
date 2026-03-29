using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AttackBtn : MonoBehaviour
{
    protected bool _isFlush =false;
    [SerializeField] private GameObject _toolTip;
    [SerializeField] private Renderer _renderer;
    private GameObject _visual;
    [SerializeField] protected BoolEventChannelSO _playerTurnEndChannel;
    private Material _material;
    [SerializeField] private bool _single = true;
    private bool _isAble = false;
    private bool _isOn = true;
    protected int _damage;
    public int _cardCount;
    protected int _flushNum=1;
    
    
    protected int _dropCardCount;
    private void Awake()
    {
        _material = GetComponentInChildren<Renderer>().material;
        _visual = transform.Find("Visual").gameObject;
        _renderer.sortingLayerName = "UI";
        _renderer.sortingOrder = 1;
    }

    public abstract bool IsCardCount(int count);
    public abstract void CheckAttackHand(List<Card> hand);
    protected abstract void ApplyAttack();

    public void OnOffBtn(bool isOn)
    {
        _isAble = isOn;
        _material.SetFloat("_IsAble", isOn ? 1 : 0);
    }
    
    private void OnMouseDown()
    {
        if(!_isAble||!_isOn) return;
        //버튼을 눌러서 공격을 한 상태.
        //추후 다른 처리들 더 해줘야함

        PlayerHand.Instance.FixTxt();
        if(_isFlush)
            ApplyAttack();
        CombatManager.Instance.Heal = _damage*2*_flushNum;
        AttackCardHand.Instance.emblemEffect?.Invoke();
        if (AttackCardHand.Instance.isSpade)
        {
            for (int i = 0; i < _flushNum; i++)
            {
                _damage = Mathf.RoundToInt(_damage*1.25f);
            }
            AttackCardHand.Instance.isSpade = false;
        }
        ApplyAttack();
        CombatManager.Instance.Damage = _damage;
        OnMouseExit();
        OnOffVisible(false);
        _playerTurnEndChannel.RaiseEvent(true);
        CardManager.Instance.isPlayer = false;
        int count;
        count = Mathf.Clamp(_cardCount, 0, 2);
        if (AttackCardHand.Instance.isDia)
        {
            count= count + _dropCardCount+(1*_flushNum);
            Deck.Instance.ExtractCard(Mathf.Clamp(count,0,10-PlayerHand.Instance.CardCount));
            AttackCardHand.Instance.isDia = false;
        }
        else
        {
            count += _dropCardCount;
            Deck.Instance.ExtractCard(Mathf.Clamp(count,0,10-PlayerHand.Instance.CardCount));
        }
        _dropCardCount = 0;
        _flushNum = 1;
    }

    public void OnOffVisible(bool isOn)
    {
        if (_single)
        {
            _visual.SetActive(isOn);
            _isOn = isOn;
        }
    }
    protected Dictionary<int, int> GetDuplicates(List<int> numbers)
    {
        Dictionary<int, int> counts = new Dictionary<int, int>();
        foreach (int num in numbers)
        {
            if (counts.ContainsKey(num))
                counts[num]++;
            else
                counts[num] = 1;
        }
       

        // 중복된 숫자만 필터링
        Dictionary<int, int> duplicates = new Dictionary<int, int>();
        foreach (var pair in counts)
        {
            if (pair.Value > 1)
                duplicates[pair.Key] = pair.Value;
        }

        return duplicates;
    }

    protected bool IsRoyalStraight(List<Card> card)
    {
        // 리스트 정렬
        List<int> numbers = card.Select(dt => dt.CardData.number).Distinct().OrderBy(x => x).ToList();

        // 에이스(1)가 포함된 경우 10, J, Q, K, A 형태 확인
        if (numbers.Contains(1) && numbers.Contains(10) && numbers.Contains(11) &&
            numbers.Contains(12) && numbers.Contains(13))
        {
            _damage = 14;
            return true;
        }
        return false;
    }

    protected bool IsStraight(List<Card> card)
    {
        // 리스트 정렬
        Dictionary<int, int> duplicates = GetDuplicates(card.Select(dt=>dt.CardData.number).ToList());
        if (duplicates.Count > 0) return false;
        List<int> numbers = card.Select(dt=>dt.CardData.number).Distinct().OrderBy(x => x).ToList();

        // 에이스(1)가 포함된 경우 10, J, Q, K, A 형태 확인
        if (numbers.Contains(1) && numbers.Contains(10) && numbers.Contains(11) &&
            numbers.Contains(12) && numbers.Contains(13))
        {
            _damage = 14;
            return true;
        }
        // 정렬된 숫자에서 연속 여부 확인
        for (int i = 0; i < numbers.Count - 1; i++)
        {
            if (numbers[i + 1] != numbers[i] + 1)
                return false;
        }
        _damage = numbers[numbers.Count - 1];
        return true;
    }

    protected bool IsFlush(List<Card> cards)
    {
        CardEmblem firstEmblem = cards.First().CardData.emblem;

        if (cards.All(card => card.CardData.emblem == firstEmblem))
        {
            int num = cards.Select(card => card.CardData.number).Min();
            if (num == 1)
                _damage = 14;
            else
                _damage = cards.Select(card => card.CardData.number).Max();
            return true;
        }
        return false;
    }

    private void OnMouseEnter()
    {
        if(!_isOn) return;
        _toolTip.SetActive(true);
    }

    private void OnMouseExit()
    {
        _toolTip.SetActive(false);
    }
}