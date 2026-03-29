using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public bool isFastMode = false;

    public float moveTime => isFastMode ? 0.1f : 0.5f;


    public void TimeFast()
    {
        Time.timeScale = 2f;
    }
    public void TimeSlow()
    {
        Time.timeScale = 1f;
    }
}
