using System;
using UnityEngine;

public class Thunder : MonoSingleton<Thunder>
{
    [SerializeField] private SoundSO _sound;
    [SerializeField] private Animator thunder;
    [SerializeField] private Animator hit;
    private readonly int atkHash = Animator.StringToHash("Attack");
    
    public void Attack(Transform target)
    {
        transform.position = new Vector3(target.position.x,transform.position.y,transform.position.z);
        thunder.SetTrigger(atkHash);
    }

    public void PlayHit()
    {
        SoundPlayer player = PoolManager.Instance.Pop("SoundPlayer") as SoundPlayer;
        player.PlaySound(_sound);
        hit.SetTrigger(atkHash);
    }
}
