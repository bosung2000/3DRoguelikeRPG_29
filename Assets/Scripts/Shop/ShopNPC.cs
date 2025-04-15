using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopNPC : MonoBehaviour
{
    [SerializeField] private Shop shop;
    [SerializeField] private float interactionDistance = 2f;

    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("Player"))
        //{
        //    // 상호작용 가능 UI 표시
        //    UIManager.Instance.ShowInteractionUI("상점 열기 (E)");
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
        shopUI.Initialize(shop); // 해당 NPC의 Shop 컴포넌트를 전달
    }
}
