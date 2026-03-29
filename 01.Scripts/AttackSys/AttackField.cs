using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class AttackField : MonoBehaviour
{
    public bool isEmpty = true;
    public bool isEmblem = false;
    
    [SerializeField] private List<string> _tips;
    [SerializeField] private GameObject _toolTip;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private TextMeshPro _text;
    private List<Renderer> _materials = new List<Renderer>();
    private void Awake()
    {
        _materials = GetComponentsInChildren<Renderer>().ToList();
        
        if(!isEmblem) return;
        _renderer.sortingLayerName = "UI";
        _renderer.sortingOrder = 1;
        _text.text = _tips[4];
    }

    public void OnOffMat(Material mat,int index)
    {
        _text.text = _tips[index];
        _materials.ForEach(material=>material.material = mat);
    }

    private void OnMouseEnter()
    {
        if(!isEmblem) return;
        _toolTip.SetActive(true);
    }
    private void OnMouseExit()
    {
        if(!isEmblem) return;
        _toolTip.SetActive(false);
    }
}