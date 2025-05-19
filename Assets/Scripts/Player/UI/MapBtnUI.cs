using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapBtnUI : MonoBehaviour
{
    [SerializeField] private Button Mapbtn;
    public ShopNPC shopnpc;
    // Start is called before the first frame update
    void Start()
    {
        Mapbtn = GetComponent<Button>();
        this.gameObject.SetActive(false);

    }

    public void MapShow()
    {
        UIManager.Instance.ShowPopupUI<UIMap>();
    }

    public void ShopShow()
    {
        if (shopnpc != null)
        {
            shopnpc.OpenShop();
        }
    }



}
