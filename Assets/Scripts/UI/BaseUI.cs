using UnityEngine;

public class BaseUI : MonoBehaviour
{
    protected virtual void Awake()
    {

    }
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
    protected virtual void Clear()
    {

    }
} 