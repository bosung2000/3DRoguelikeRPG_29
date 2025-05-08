using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapShow : MonoBehaviour
{
    [SerializeField] private UIMap uIMap;


    void Start()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //if (uIMap ==null)
            //{
            //    uIMap = FindObjectOfType<UIMap>();
            //}
            UIManager.Instance.ShowPopupUI<UIMap>();
            //uIMap.gameObject.SetActive(true);
        }
    }


}
