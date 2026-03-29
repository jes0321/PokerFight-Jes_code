using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
public enum TurnState
{
    Nothing,
    MyTurn,
    NotMyTurn
}
public class CardManager : MonoSingleton<CardManager>
{
    public bool isLoading = false;
    public bool isPlayer = true;
    
    private bool _isMyCardDrag = false;
    private Card _selectedCard;
    private bool _onCardArea = false;

    [SerializeField] private TurnState turnState;
    private Vector3 _MousePos
    {
        get
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = -10;
            return pos;
        }
    }

    [Header("Layer mask")]
    [SerializeField] private LayerMask _whatIsCardArea;
    [SerializeField] private LayerMask _whatIsField;
    private void Update()
    {
        if (_isMyCardDrag)
            CardDrag();

        ResetCard();
        DetectCardArea();
        SetPlayerState();
    }

    private void ResetCard()
    {
        if (isLoading)
        {
            if (_selectedCard != null)
            {
                _selectedCard = null;
            }
        }
    }

    private void SetPlayerState()
    {
        if(isLoading)
            turnState = TurnState.Nothing;
        else
            turnState = isPlayer?TurnState.MyTurn:TurnState.NotMyTurn;
    }

    private void DetectCardArea()
    {
        _onCardArea = Physics2D.Raycast(_MousePos,Vector3.forward,100f,_whatIsCardArea);
    }

    private void CardDrag()
    {
        if(_selectedCard==null) return;
        if (!_onCardArea)
        {
            _selectedCard.MoveCompo.MoveTransform(new PRS(_MousePos,Quaternion.identity,
                _selectedCard.MoveCompo.Prs.scale),false);
        }
        else if (_onCardArea && _selectedCard._isAttackCard)
        {
            AttackCardHand.Instance.RemoveCard(_selectedCard,_selectedCard.field.isEmblem);
            RemoveCard();
        }
    }

    private void RemoveCard()
    {
        PlayerHand.Instance.AddCard(_selectedCard);
        if(_selectedCard.field==null) return;
        _selectedCard.field.isEmpty = true;
        _selectedCard.field = null;
    }

    public void SelectCard(Card card)
    {
        if(turnState == TurnState.Nothing||_isMyCardDrag) return;
        _selectedCard = card;
        Enlarge(true,_selectedCard);
    }

    private void Enlarge(bool isEnlarge, Card card)
    {
        if(isLoading) return;
        if (isEnlarge)
        {
            card.Renderer.SetMostFrontOrder(true);
            Vector3 enlargePos = new Vector3(card.MoveCompo.Prs.pos.x, card.MoveCompo.Prs.pos.y+1.6f, -10f);
            card.MoveCompo.MoveTransform(new PRS(enlargePos,Quaternion.identity,Vector3.one*1.5f),false);
        }
        else
        {
            card.Renderer.SetMostFrontOrder(false);
            card.MoveCompo.MoveTransform(card.MoveCompo.Prs, false);
        }
    }

    public void ExitCard(Card card)
    {
        Enlarge(false,card);
    }

    public void CardMouseDown()
    {
        if(turnState != TurnState.MyTurn) return;
        _isMyCardDrag = true;
    }
    public void CardMouseUp()
    {
        _isMyCardDrag = false;
        if(turnState != TurnState.MyTurn) return;
        
        var hit = Physics2D.Raycast(_MousePos,Vector3.forward,100f,_whatIsField);
        
        if (hit.collider == null || !hit.collider.TryGetComponent(out AttackField field)) return;
        
        if(!field.isEmpty) return;

        if (_selectedCard.field != null)
        {
            AttackCardHand.Instance.RemoveCard(_selectedCard,_selectedCard.field.isEmblem);
            RemoveCard();
        }
        
        AttackCardHand.Instance.AddCard(_selectedCard, field.isEmblem);
        field.isEmpty = false;

        _selectedCard._isAttackCard = true;
        _selectedCard.field = field;
        
        _selectedCard.MoveCompo.Prs = new PRS(field.transform.position,
            quaternion.identity, _selectedCard.MoveCompo.Prs.scale);
        PlayerHand.Instance.RemoveCard(_selectedCard);
    }
}
