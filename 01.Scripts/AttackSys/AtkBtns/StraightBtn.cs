using System.Collections.Generic;
using UnityEngine;

public class StraightBtn : AttackBtn
{
    public override bool IsCardCount(int count)
    {
        return count >=5;
    }

    public override void CheckAttackHand(List<Card> hand)
    {
        if (IsStraight(hand))
        {
            OnOffBtn(true);
        }
    }

    protected override void ApplyAttack()
    {
        CombatManager.Instance.scrPlayerCombat.isSkip = true;
    }
}
