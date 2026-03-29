using System.Collections.Generic;
using UnityEngine;

public class FlushBtn : AttackBtn
{
    
    public override bool IsCardCount(int count)
    {
        return count >= 5;
    }

    public override void CheckAttackHand(List<Card> hand)
    {
        if (IsFlush(hand))
        {
            _isFlush = true;
            OnOffBtn(true);
        }
    }

    protected override void ApplyAttack()
    {
        _flushNum = 3;
    }
}
