using UnityEngine;

public class ThunderAnim : MonoBehaviour
{
    public void AtkEnd()
    {
        Thunder.Instance.PlayHit();
    }
}