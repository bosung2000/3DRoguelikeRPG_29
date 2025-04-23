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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {

            OpenShop();
            Time.timeScale = 0;
        }
    }

    private void OpenShop()
    {
        var shopUI = UIManager.Instance.ShowPopupUI<UIShop>();
        shopUI.Initialize(shop,playerManager); // 해당 NPC의 Shop 컴포넌트를 전달

        
    }

}
