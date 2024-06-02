using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public GameObject notification;
    public GameObject interactuar;
    public static UIManager instance;
    private void Awake()
    {
        instance = this;
    }

    public void showInteractuar()
    {
        interactuar.SetActive(true);
    }

    public void hideInteractuar()
    {
        interactuar.SetActive(false);
    }

    public void showNotification(string msg)
    {
        if (!notification.activeSelf)
        {
            notification.SetActive(true);
        }
        notification.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "";
        notification.GetComponent<RectTransform>().DOSizeDelta(new Vector2(720, 200), 0.2f).OnComplete(() => notification.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = msg);
    }

    public void hideNotification()
    {
        notification.GetComponent<RectTransform>().DOSizeDelta(new Vector2(720, 0), 0.2f).OnComplete(() => notification.SetActive(false));
    }
}
