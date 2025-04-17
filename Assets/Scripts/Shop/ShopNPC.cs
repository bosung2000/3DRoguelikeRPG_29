using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopNPC : MonoBehaviour
{
    [SerializeField] private Shop shop;
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private PlayerManager playerManager;

    private void Start()
    {
        playerManager =GameManager.Instance.PlayerManager;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("Player"))
        //{
        //    OpenShop();
        //}
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            OpenShop();
        }
    }
    
    private void OpenShop()
    {
        var shopUI = UIManager.Instance.ShowPopupUI<UIShop>();
        shopUI.Initialize(shop,playerManager); // 해당 NPC의 Shop 컴포넌트를 전달

        
    }

}
