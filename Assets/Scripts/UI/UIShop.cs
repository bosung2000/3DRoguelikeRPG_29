using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : PopupUI
{
    //판매 나의인벤토리 
    //구매 shop가지고 있는 아이템 
    //판매해야할 아이템 list 가지고있고 
    //buy > inventory읽어와서 show 
    //sell > uishop(판매list)이중에서 색출해서(뭐 등급이 맞게 높은 등급일수록 좋은아이템이 나와야하잔아)
    // 그럼 아아템의 등급이 있어야하겠는데 ?            
    [SerializeField] TextMeshProUGUI goldTxt;
    [SerializeField] GameObject completePopup;
    [SerializeField] Button completeBtn;
    [SerializeField] TextMeshProUGUI completeTxt;
    [SerializeField] Button exitBtn;

}
