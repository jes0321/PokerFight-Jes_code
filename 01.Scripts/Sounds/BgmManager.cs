using System;
using UnityEngine;

public class BgmManager : MonoSingleton<BgmManager>
{
    [SerializeField] private SoundSO _bgmSO;
    private SoundPlayer _bgmPlayer;
    private void Start()
    {
        _bgmPlayer = PoolManager.Instance.Pop("SoundPlayer") as SoundPlayer;
        
        _bgmPlayer.PlaySound(_bgmSO);
    }

    private void BGMPlay() => _bgmPlayer.PlaySound();
    private void BGMStop() => _bgmPlayer.StopSound();
}
