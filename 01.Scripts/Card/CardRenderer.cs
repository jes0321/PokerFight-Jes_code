using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardRenderer : MonoBehaviour
{
    private Animator _animator; // Animator 컴포넌트를 연결
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private AnimatorOverrideController _overrideController; // Animator Override Controller
    [SerializeField] private AnimationClip _emptyClip;
    public string targetStateName = "Idle"; // 바꿀 상태의 이름
    private int _originOrder;
    [SerializeField] private Sprite _backSprite;
    private Sprite _sprite;
    private AnimationClip _animationClip;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>(); 
    }

    public void Initialize(CardData data,bool isBack)
    {
        _sprite = data.sprite;
        _animationClip = data.aniClip;
        
        SpriteEmptyChange(isBack);
    }

    public void SpriteEmptyChange(bool isBack)
    {
       if(isBack)
           SetSpriteAndAnimClip(_backSprite,_emptyClip);
       else
           SetSpriteAndAnimClip(_sprite, _animationClip);
    }
    
    private void SetSpriteAndAnimClip(Sprite sprite,AnimationClip clip)
    {
        if(_spriteRenderer==null)
            Start();
        SetAnimClip(clip);
        _spriteRenderer.sprite = sprite;
    }

    #region AnimClip
    private AnimatorOverrideController CopyOverrideController(AnimatorOverrideController original)
    {
        // 새 인스턴스를 만들고, 원본의 RuntimeAnimatorController를 복사
        AnimatorOverrideController copy = new AnimatorOverrideController();
        copy.runtimeAnimatorController = original.runtimeAnimatorController;

        // 기존 오버라이드 리스트를 가져와서 복사
        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        original.GetOverrides(overrides);

        // 새로운 오버라이드를 적용
        List<KeyValuePair<AnimationClip, AnimationClip>> newOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(overrides);
        copy.ApplyOverrides(newOverrides);

        return copy;
    }
    private void SetAnimClip(AnimationClip clip)
    {
        // 기존 Animator Controller를 복사
        RuntimeAnimatorController originalController = _animator.runtimeAnimatorController;

        // _overrideController를 복사하여 새로운 AnimatorOverrideController로 설정
        _overrideController = CopyOverrideController(_overrideController);  // 이 라인에서 복사를 해야 중첩 문제 해결

        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        _overrideController.GetOverrides(overrides);

        // targetStateName에 해당하는 애니메이션 클립을 찾아서 변경
        for (int i = 0; i < overrides.Count; i++)
        {
            if (overrides[i].Key.name == targetStateName)
            {
                // clip이 null일 경우 _emptyClip을 사용
                if (clip == null)
                    overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, _emptyClip);
                else
                    overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, clip);
                break;
            }
        }

        // 변경된 오버라이드 적용
        _overrideController.ApplyOverrides(overrides);

        // 애니메이터에 새로운 runtimeAnimatorController 설정
        _animator.runtimeAnimatorController = _overrideController;
    }

    #endregion

    #region Order

    public void SetOriginOrder(int order)
    {
        _originOrder = order;
        SetOrder(_originOrder);
    }
    public void SetMostFrontOrder(bool isFront)
    {
        SetOrder(isFront?1000:_originOrder);
    }
    private void SetOrder(int order) => _spriteRenderer.sortingOrder = order;

    #endregion
    
}
