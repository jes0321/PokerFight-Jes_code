using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinText : MonoSingleton<WinText>
{
    [SerializeField] private RectTransform rectTransform;

    public void WinAction()
    {
        DOVirtual.DelayedCall(3f,()=>rectTransform.DOScale(new Vector3(3.23f, 3.3f, 0),1).OnComplete(()=>
            SceneManager.LoadScene("TitleScene")));
    }
}
