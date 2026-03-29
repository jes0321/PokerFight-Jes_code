using System;
using UnityEngine;

[CreateAssetMenu(fileName = "IntEventChannelSO", menuName = "SO/Event/Int")]
public class IntEventChannelSO : ScriptableObject
{
    public Action<int> OnValueEvent;

    public void RaiseEvent(int value)
    {
        OnValueEvent?.Invoke(value);
    }
}
