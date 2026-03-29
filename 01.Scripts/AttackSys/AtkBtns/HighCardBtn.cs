using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HighCardBtn : AttackBtn
{
    public override bool IsCardCount(int count)
    {
        return count >= 1;
    }

    public override void CheckAttackHand(List<Card> hand)
    {
        OnOffBtn(true);
        if (hand.Min(card => card.CardData.number) == 1)
        {
            _damage = 14;
            return;
        }
        _damage = hand.Max(card=>card.CardData.number);
    }

    protected override void ApplyAttack()
    {
        //변수에 담긴 데미지로 공격하기.
    }
}
