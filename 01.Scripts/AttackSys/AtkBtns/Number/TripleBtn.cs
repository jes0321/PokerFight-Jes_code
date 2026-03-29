using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TripleBtn : AttackBtn
{
    public override bool IsCardCount(int count)
    {
        return count >= 3;
    }

    public override void CheckAttackHand(List<Card> hand)
    {
        var dic = GetDuplicates(hand.Select(data => data.CardData.number).ToList());
        
        _damage = dic
            .Where(pair => pair.Value == 3) // 중복 개수가 2인 항목만 필터링
            .Select(pair => pair.Key) // 키만 추출
            .DefaultIfEmpty() // 값이 없을 경우 기본값 처리
            .Max(); // 가장 큰 키 찾기
        if (_damage>0)
        {
            if(_damage==1)
                _damage = 14;
            OnOffBtn(true);
        } 
    }

    protected override void ApplyAttack()
    {
        CombatManager.Instance.isTriple = true;
    }
}