using System;
using UnityEngine;


public class Card : MonoBehaviour, IPoolable
{
    #region Pool Sys
    public string PoolName => _poolName;
    [SerializeField] private string _poolName;
    public GameObject ObjectPrefab => gameObject;
    public void ResetItem()
    {
    }
    #endregion
    
    public CardData CardData { get; private set; }
    private bool _isPlayerCard;
    public bool _isAttackCard = false;
    public AttackField field;
    #region compo
    public CardRenderer Renderer {get; private set;}
    
    public CardMove MoveCompo { get; private set; }

    #endregion
    private void Awake()
    {
        Renderer = GetComponentInChildren<CardRenderer>();
        MoveCompo = GetComponent<CardMove>();
    }

    public void FieldInit()
    {
        field = null;
    }
    public void Initialize(CardData cardData,bool isPlayerCard)
    {
        CardData = cardData;
        _isPlayerCard = isPlayerCard;
        if(Renderer == null||MoveCompo == null)
            Awake();
        
        Renderer.Initialize(CardData,!isPlayerCard);
    }

    private void OnMouseOver()
    {
        if(_isPlayerCard)
            CardManager.Instance.SelectCard(this);
    }

    private void OnMouseExit()
    {
        if(_isPlayerCard)
            CardManager.Instance.ExitCard(this);
    }

    private void OnMouseDown()
    {
        if(_isPlayerCard)
            CardManager.Instance.CardMouseDown();
    }
    private void OnMouseUp()
    {
        if(_isPlayerCard)
            CardManager.Instance.CardMouseUp();
    }
}
