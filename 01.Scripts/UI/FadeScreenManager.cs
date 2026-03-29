using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeScreenManager : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;
    [SerializeField] private BoolEventChannelSO _fadeScreenChannel;
    [SerializeField] private BoolEventChannelSO _saveEventChannel;
    [SerializeField] private BoolEventChannelSO _loadEventChannel;
    [SerializeField] private BoolEventChannelSO _gameStartEventChannel;
    [SerializeField] private BoolEventChannelSO _deadStartChannel;

    [SerializeField] private float _fadeDuration = 0.5f;
    private readonly int _valueHash = Shader.PropertyToID("_Value");

    private void Awake()
    {
        _fadeImage.material = new Material(_fadeImage.material);
        //스프라이트 렌더러와는 다르게 Image의 매티리얼은 전부 셰어드를 사용한다.
        //따라서 셰이더 작업을 하는 UI는 시작할 때 이를 인스턴스화 해주는 작업이 필요하다.

        _fadeScreenChannel.OnValueEvent += HandleFadeEvent;
    }

    private void OnDestroy()
    {
        _fadeScreenChannel.OnValueEvent -= HandleFadeEvent;
    }

    private void HandleFadeEvent(bool isFadeIn)
    {
        float fadeValue = isFadeIn ? 1.2f : 0f;
        float startValue = isFadeIn ? 0f : 1.2f;
        
        _fadeImage.material.SetFloat(_valueHash, startValue);

        if (isFadeIn)
        {
            _loadEventChannel.RaiseEvent(false);
        }
        
        var tweenCore =  _fadeImage.material.DOFloat(fadeValue, _valueHash, _fadeDuration).OnComplete(()=>
        {
            _gameStartEventChannel.RaiseEvent(true);
        });

        if (isFadeIn == false)
        {
            tweenCore.OnComplete(()=>
            {
                _deadStartChannel.RaiseEvent(true);
                _saveEventChannel.RaiseEvent(false);
                _gameStartEventChannel.RaiseEvent(true);
            });
        }
    }

    private void OnApplicationQuit()
    {
        _saveEventChannel.RaiseEvent(true);
    }
}
