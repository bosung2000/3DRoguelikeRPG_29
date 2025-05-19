using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopNPC : MonoBehaviour
{
    [SerializeField] private Shop shop;
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private PlayerManager playerManager;
    private FloatingJoystick floatingJoystick;


    private void Start()
    {
        playerManager =GameManager.Instance.PlayerManager;
        floatingJoystick = FindObjectOfType<FloatingJoystick>();
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.transform.CompareTag("Player"))
    //    {

    //        OpenShop();
    //        Time.timeScale = 0;
    //    }
    //}

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {

            OpenShop();
            Time.timeScale = 0;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //if (other.transform.CompareTag("Player")&& Input.GetKeyDown(KeyCode.E))
        //{

        //    OpenShop();
        //    Time.timeScale = 0;
        //}
    }

    private void OpenShop()
    {
        var shopUI = UIManager.Instance.ShowPopupUI<UIShop>();
        shopUI.Initialize(shop,playerManager); // 해당 NPC의 Shop 컴포넌트를 전달

        //조이스틱 UI끄기
        floatingJoystick.gameObject.SetActive(false);

    }

}
