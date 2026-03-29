using System.Collections.Generic;
using UnityEngine;

public class StrFlushBtn : AttackBtn
{
    public override bool IsCardCount(int count)
    {
        return count >= 5;
    }

    public override void CheckAttackHand(List<Card> hand)
    {
        if (IsFlush(hand))
        {
            if (IsStraight(hand))
            {
                OnOffBtn(true);
            }
        }
    }

    protected override void ApplyAttack()
    {
        CombatManager.Instance.isMulti = true;
        _damage = 100;
    }
}
