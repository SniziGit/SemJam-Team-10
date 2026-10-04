using Unity.VisualScripting;
using UnityEngine;
[RequireComponent (typeof(CanvasGroup))]

public class UIElement : MonoBehaviour
{
public void OpenUI() // do not call on update or awake
    {
        UIManager.Instance.OpenUI(gameObject);
    }
    public void CloseUI() // do not call on update or awake
    {
        UIManager.Instance.CloseUI(gameObject);
    }
}
