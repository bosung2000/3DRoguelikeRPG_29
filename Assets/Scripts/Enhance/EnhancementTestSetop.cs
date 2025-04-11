using UnityEngine;
using UnityEngine.UI;

public class EnhancementTestSetup : MonoBehaviour
{
    public EquipmentEnhanceUI enhanceUI;  
    public ItemData testItem;            
    public Button setupButton;           
    void Start()
    {
        setupButton.onClick.AddListener(SetupTestItem);
    }

    void SetupTestItem()
    {
        enhanceUI.SetEquipment(testItem);
        Debug.Log("테스트 아이템 세팅 완료!");
    }
}
