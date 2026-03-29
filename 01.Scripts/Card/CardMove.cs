using DG.Tweening;
using UnityEngine;

public class CardMove : MonoBehaviour
{
    public PRS Prs;
    public void MoveTransform(PRS prs, bool useDotween, float dotweenTime = 0)
    {
        if (useDotween)
        {
            transform.DOMove(prs.pos, dotweenTime);
            transform.DORotateQuaternion(prs.rot, dotweenTime);
            transform.DOScale(prs.scale, dotweenTime).SetEase(Ease.Linear);
        }
        else
        {
            transform.localScale = prs.scale;
            transform.position = prs.pos;
            transform.rotation = prs.rot;
        }
    }
}
