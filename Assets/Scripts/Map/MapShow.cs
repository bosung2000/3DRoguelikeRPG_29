using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapShow : MonoBehaviour
{
    [SerializeField] private UIMap uIMap;


    void Start()
    {
        uIMap =FindObjectOfType<UIMap>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            UIManager.Instance.ShowPopupUI<UIMap>();
        }
    }


}
