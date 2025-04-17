using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    private static ItemManager instance;
    public static ItemManager Instance => instance;

    [SerializeField] private List<ItemData> allItems = new List<ItemData>();
    private Dictionary<int, ItemData> itemDictionary = new Dictionary<int, ItemData>();
    private Dictionary<ItemType, List<ItemData>> itemsByType = new Dictionary<ItemType, List<ItemData>>();
    private Dictionary<int, List<ItemData>> itemsByTier = new Dictionary<int, List<ItemData>>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllItems(); 
            InitializeItemDictionaries();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// 아이템 읽어와서 allitems에 넣기 
    /// </summary>
    private void LoadAllItems()
    {
        // Resources/Items 폴더에서 모든 ItemData 에셋을 로드
        ItemData[] loadedItems = Resources.LoadAll<ItemData>("Items");

        // 기존 리스트 초기화
        allItems.Clear();
        
        // 로드된 아이템 추가
        foreach (var item in loadedItems)
        {
            allItems.Add(item);
        }

        Debug.Log($"총 {allItems.Count}개의 아이템이 로드되었습니다.");
    }
    /// <summary>
    /// allItems를 각각의 dictionary
    /// </summary>
    private void InitializeItemDictionaries()
    {
        if (allItems ==null)
        {
            Debug.Log("AllItems에 아이템이 없다");
            return;
        }

        // 딕셔너리 초기화
        itemDictionary.Clear();
        itemsByType.Clear();
        itemsByTier.Clear();

        // allItems에 있는 모든 아이템을 각 딕셔너리에 자동으로 분류
        foreach (var item in allItems)
        {
            // ID로 검색 가능하도록
            itemDictionary[item.id] = item;

            // 타입별 분류
            if (!itemsByType.ContainsKey(item.itemType))
                itemsByType[item.itemType] = new List<ItemData>();
            itemsByType[item.itemType].Add(item);

            // 티어별 분류 (장비 아이템만)
            if (item.itemType == ItemType.Equipment)
            {
                if (!itemsByTier.ContainsKey(item.Tier))
                    itemsByTier[item.Tier] = new List<ItemData>();
                itemsByTier[item.Tier].Add(item);
            }
        }
    }

    // ID로 아이템 찾기
    public ItemData GetItemById(int id)
    {
        return itemDictionary.TryGetValue(id, out var item) ? item : null;
    }

    // 타입으로 아이템 리스트 가져오기
    public List<ItemData> GetItemsByType(ItemType type)
    {
        return itemsByType.TryGetValue(type, out var items) ? items : new List<ItemData>();
    }

    // 티어로 아이템 리스트 가져오기
    public List<ItemData> GetItemsByTier(int tier)
    {
        return itemsByTier.TryGetValue(tier, out var items) ? items : new List<ItemData>();
    }

    // 장비 타입으로 아이템 리스트 가져오기
    public List<ItemData> GetEquipmentByType(EquipType equipType)
    {
        var equipmentList = GetItemsByType(ItemType.Equipment);
        return equipmentList.FindAll(item => item.equipType == equipType);
    }

    // 티어 범위로 아이템 리스트 가져오기
    public List<ItemData> GetItemsByTierRange(int minTier, int maxTier)
    {
        List<ItemData> result = new List<ItemData>();
        for (int i = minTier; i <= maxTier; i++)
        {
            result.AddRange(GetItemsByTier(i));
        }
        return result;
    }

    // 사용할일이 없을듯 ?SO 로 만들어서 관리하고 읽어올것이기때문에
    // 아이템 생성 (새로운 인스턴스)
    public ItemData CreateItemInstance(int id)
    {
        var original = GetItemById(id);
        if (original == null) return null;

        // ScriptableObject 복사
        var newItem = ScriptableObject.CreateInstance<ItemData>();
        newItem.id = original.id;
        newItem.itemType = original.itemType;
        newItem.equipType = original.equipType;
        newItem.useType = original.useType;
        newItem.itemName = original.itemName;
        newItem.description = original.description;
        newItem.Tier = original.Tier;
        newItem.gold = original.gold;
        newItem.Icon = original.Icon;
        newItem.itemObj = original.itemObj;

        // 옵션 복사
        if (original.itemType == ItemType.Equipment)
        {
            newItem.options = new List<ItemOption>(original.options);
            newItem.enhancementLevel = 0;
            newItem.maxEnhancementLevel = original.maxEnhancementLevel;
            newItem.enhancementCost = original.enhancementCost;
            newItem.enhancementCostMultiplier = original.enhancementCostMultiplier;
        }
        else if (original.itemType == ItemType.Consumable)
        {
            newItem.consumableEffects = new List<ConsumableEffect>(original.consumableEffects);
            newItem.maxStack = original.maxStack;
        }

        return newItem;
    }
}