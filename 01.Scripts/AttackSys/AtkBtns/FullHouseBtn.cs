using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FullHouseBtn : AttackBtn
{
    public override bool IsCardCount(int count)
    {
        return count >= 5;
    }

    public override void CheckAttackHand(List<Card> hand)
    {
        List<int> numbers = hand.Select(dt => dt.CardData.number).ToList();

        // 중복된 숫자와 그 개수 가져오기
        var duplicates = GetDuplicates(numbers);

        // 풀 하우스는 3개짜리와 2개짜리가 하나씩 있어야 함
        bool hasThreeOfAKind = false;
        bool hasPair = false;

        foreach (var pair in duplicates)
        {
            if (pair.Value == 3)
            {
                _damage = pair.Key;
                hasThreeOfAKind = true;
            }
            if (pair.Value == 2)
                hasPair = true;
        }

        // 풀 하우스가 성립하면 true
        if (hasThreeOfAKind && hasPair)
        {
            OnOffBtn(true);
        }
    }

    protected override void ApplyAttack()
    {
        CombatManager.Instance.isMulti = true;
    }
}
