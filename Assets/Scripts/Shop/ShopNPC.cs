using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopNPC : MonoBehaviour
{
    [SerializeField] private Shop shop;
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private Button shopbtn;
    private FloatingJoystick floatingJoystick;


    private void Start()
    {
        playerManager =GameManager.Instance.PlayerManager;
        floatingJoystick = FindObjectOfType<FloatingJoystick>();

        shopbtn = FindInactiveButtonInScene("ShopOpen"); // 오브젝트 이름을 정확히 입력

        if (shopbtn == null)
        {
            Debug.LogError("Shop Button을 찾을 수 없습니다!");
        }
    }

    // 비활성화 오브젝트까지 Button 찾는 함수
    private Button FindInactiveButtonInScene(string btnName)
    {
        Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform t in allTransforms)
        {
            if (t.name == btnName && t.GetComponent<Button>() != null)
            {
                return t.GetComponent<Button>();
            }
        }
        return null;
    }


    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {

            shopbtn.gameObject.SetActive(true);
            shopbtn.GetComponent<MapBtnUI>().shopnpc = this;
        }

        if (collision.transform.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {

            OpenShop();
            Time.timeScale = 0;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            shopbtn.gameObject.SetActive(false);
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

    public void OpenShop()
    {
        var shopUI = UIManager.Instance.ShowPopupUI<UIShop>();
        shopUI.Initialize(shop,playerManager); // 해당 NPC의 Shop 컴포넌트를 전달

        //조이스틱 UI끄기
        floatingJoystick.gameObject.SetActive(false);

    }

}
