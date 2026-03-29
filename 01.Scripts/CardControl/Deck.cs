using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Deck : MonoSingleton<Deck>
{
    [SerializeField] private TextMeshPro _countTxt;
    [SerializeField] private Transform deckPos;
    [SerializeField] private Transform centerPos;
    [SerializeField] private CardList _cardList;
    [SerializeField] private BoolEventChannelSO _gameStartChannel;
    private float _moveTime;
    private CardShuffle _shuffleCompo;
    public int maxCardCount=4;
    
    protected override void Awake()
    {
        base.Awake();
        _shuffleCompo = GetComponent<CardShuffle>();
        _gameStartChannel.OnValueEvent += ShuffleStart;
    }

    private void OnDestroy()
    {
        _gameStartChannel.OnValueEvent -= ShuffleStart;
    }

    private void ShuffleStart(bool isStart)
    {
        if (isStart)
        {
            CardManager.Instance.isLoading = true;
            _shuffleCompo.Shuffling();
        }   
    }

    public void MovePos()
    {
        transform.DOMove(deckPos.position, 0.5f).OnComplete(() =>
        {
            CardManager.Instance.isLoading = false;
            int count = maxCardCount - PlayerHand.Instance.CardCount;
            ExtractCard(count);
        });
    }

    private void Update()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
            ExtractCard(5);
    }

    public void ExtractCard(int count)
    {
        if(CardManager.Instance.isLoading) return;
        
        StartCoroutine(CardMoveCo(count));
    }

    private IEnumerator CardMoveCo(int count)
    {
        if (count > 1)
            GameManager.Instance.isFastMode = true;
        while (count>0)
        {
            CardManager.Instance.isLoading = true;
            CardData dt = GetRandomCard();
            DeckCountTxt();
            Card card = PoolManager.Instance.Pop("Card") as Card;
            card.FieldInit();
            card.Initialize(dt,false);
            card.transform.position = transform.position;
            
            _moveTime = GameManager.Instance.moveTime;
            PRS downPrs = new PRS(new Vector3(transform.position.x,transform.position.y-1,0), 
                Quaternion.identity, Vector3.one);
            card.MoveCompo.MoveTransform(downPrs,true,_moveTime*0.5f);
            yield return new WaitForSeconds(_moveTime*0.5f);
        
            PRS prs = new PRS(centerPos.position, Quaternion.identity, Vector3.one);
            card.MoveCompo.Prs = prs;
            card.MoveCompo.MoveTransform(prs,true,_moveTime);
            yield return new WaitForSeconds(_moveTime);
        
            PRS scale0 = new PRS(prs.pos, prs.rot, Vector3.up);
            card.MoveCompo.MoveTransform(scale0,true,_moveTime*0.5f);
            yield return new WaitForSeconds(_moveTime*0.5f);
            card.Renderer.SpriteEmptyChange(false);
        
            PRS scale1 = new PRS(prs.pos, prs.rot, Vector3.one);
            card.MoveCompo.MoveTransform(scale1,true,_moveTime*0.5f);
            yield return new WaitForSeconds(_moveTime*0.5f);
            PlayerHand.Instance.AddCard(card);
            count--;
        }
        GameManager.Instance.isFastMode = false;
        yield return new WaitForSeconds(_moveTime);
        CardManager.Instance.isLoading = false;
    }
    private CardData GetRandomCard() => _cardList.ExtractCard();

    private void DeckCountTxt()
    {
        int count = _cardList.CardCount();

        if (count == 0)
        {
            _cardList.CardCountReset();
        }
        _countTxt.text = $"{count} / 52";
    }
}
