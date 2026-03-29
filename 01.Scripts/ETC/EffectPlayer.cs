using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EffectPlayer : MonoBehaviour, IPoolable
{


    private ParticleSystem _particle;
    private float _duration;
    private WaitForSeconds _particleDuration;

    [SerializeField] private bool _isRoop=false;
    

    private void Awake()
    {
        _particle = GetComponent<ParticleSystem>();
        _duration = _particle.main.duration;
        _particleDuration = new WaitForSeconds(_duration); 
    }

    public void SetPositionAndPlay(Vector3 position)
    {
        transform.position = position;
        _particle.Play();
        if(!_isRoop)
            StartCoroutine(DelayAndGotoPoolCoroutine());
    }

    private IEnumerator DelayAndGotoPoolCoroutine()
    {
        yield return _particleDuration;
        PoolManager.Instance.Push(this);
    }

    public void StopEffect()
    {        
        PoolManager.Instance.Push(this);
    }
    public string PoolName => poolName;
    public string poolName = "EffectPlayer";
    public GameObject ObjectPrefab => gameObject;

    public void ResetItem()
    {
        _particle.Stop();
        _particle.Simulate(0); 
    }
}
