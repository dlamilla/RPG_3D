using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollision : MonoBehaviour
{
    public GameObject item;
    public Items drop;
    public WeaponType wType;
    public bool open;
    public string notificationText;
    public Transform upPoint;
    GameObject player;
    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        if (drop != null)
        {
            wType = drop.typeItem;
            notificationText = drop.msg;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !open)
        {
            player = other.gameObject;
            player.GetComponent<PlayerMotion>().chest = this;
            UIManager.instance.showInteractuar();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && !open)
        {
            player.GetComponent<PlayerMotion>().chest = null;
            player = null;
            UIManager.instance.hideInteractuar();
        }
    }

    public void Open()
    {
        if (open)
        {
            return;
        }

        open = true;
        player.GetComponent<PlayerMotion>().interacting = true;
        player.GetComponent<PlayerMotion>().Stopping();
        UIManager.instance.hideInteractuar();
        anim.enabled = true;
        StartCoroutine("Finish");
    }

    IEnumerator Finish()
    {
        yield return new WaitForSeconds(2f);
        player.GetComponent<PlayerMotion>().selectTarget(upPoint);
        UIManager.instance.showNotification(notificationText);
        yield return new WaitForSeconds(0.2f);
        item.SetActive(true);
        yield return new WaitForSeconds(2f);
        UIManager.instance.hideNotification();
        item.SetActive(false);
        player.GetComponent<PlayerMotion>().chest = null;
        player.GetComponent<PlayerMotion>().interacting = false;
        player.GetComponent<PlayerMotion>().StopEnd();
        switch (wType)
        {
            case WeaponType.sword:
                player.GetComponent<Inventory>().swordActive(drop);
                player.GetComponent<Inventory>().weapons.Add(drop);
                break;
            default:
                break;

        }

        player.GetComponent<PlayerMotion>().noTarget();
        player = null;

    }
}
