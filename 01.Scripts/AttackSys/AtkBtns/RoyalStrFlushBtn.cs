using System.Collections.Generic;

public class RoyalStrFlushBtn : AttackBtn
{
    public override bool IsCardCount(int count)
    {
        return count >= 5;
    }

    public override void CheckAttackHand(List<Card> hand)
    {
        if (IsRoyalStraight(hand))
        {
            if (IsFlush(hand))
            {
                OnOffBtn(true);
            }
        }
    }

    protected override void ApplyAttack()
    {
        CombatManager.Instance.isMulti = true;
        _damage = 100;
        CombatManager.Instance.SkipStage();
    }
}