using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;

public class CardShuffle : MonoBehaviour
{
    [SerializeField] private Sprite _backSprite;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [SerializeField] private SoundSO _shuffleSound;
    
    /// <summary>
    /// 카드매니저에서 불값을 조정해줘야함
    /// </summary>
    public void Shuffling()
    {
        //여기선 false로
        SpriteRenderer sprRenderer = Instantiate(_spriteRenderer,transform.position,Quaternion.identity);
        Transform renTrm = sprRenderer.transform;
        sprRenderer.sortingOrder = -10;
        sprRenderer.sprite = _backSprite;
        
        SoundPlayer player = PoolManager.Instance.Pop("SoundPlayer") as SoundPlayer;
        player.PlaySound(_shuffleSound);
        transform.DOScale(transform.localScale * 1.2f, 0.5f).OnComplete(() =>
        {
            renTrm.localScale = Vector3.one * 1.2f;
            Sequence sequence = DOTween.Sequence();
        
            sequence.AppendCallback(() => sprRenderer.sortingOrder = -10);

            float yPos = transform.position.y;
            sequence.Append(renTrm.DOLocalMoveY(yPos+2, 0.2f));
            sequence.AppendCallback(() => sprRenderer.sortingOrder = 10);
            sequence.Append(renTrm.DOLocalMoveY(yPos, 0.2f));
            sequence.SetLoops(7);
            sequence.OnComplete(() =>
            {
                Destroy(renTrm.gameObject);
                transform.DOScale(Vector3.one, 0.5f);
                Deck.Instance.MovePos();
                
            });
        });
        
    }
}
