using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BoolEventChannelSO", menuName = "SO/Event/Bool")]
public class BoolEventChannelSO : ScriptableObject
{
    public Action<bool> OnValueEvent;

    public void RaiseEvent(bool value)
    {
        OnValueEvent?.Invoke(value);
    }
}
