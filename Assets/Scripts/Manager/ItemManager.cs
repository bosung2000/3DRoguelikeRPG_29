using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    private static ItemManager instance;
    public static ItemManager Instance => instance;

    [SerializeField] private List<ItemData> allItems = new List<ItemData>(); // 유물 제외한 일반 아이템
    [SerializeField] private List<ItemData> relicsItems = new List<ItemData>(); // 유물 아이템 전용 리스트
    
    private Dictionary<int, ItemData> itemDictionary = new Dictionary<int, ItemData>();
    private Dictionary<ItemType, List<ItemData>> itemsByType = new Dictionary<ItemType, List<ItemData>>();
    private Dictionary<int, List<ItemData>> itemsByTier = new Dictionary<int, List<ItemData>>();
    
    // 유물 아이템 관련 딕셔너리
    private Dictionary<int, ItemData> relicsDictionary = new Dictionary<int, ItemData>();

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
    /// 아이템 읽어와서 allitems과 relicsItems에 넣기 
    /// </summary>
    private void LoadAllItems()
    {
        // Resources/Items 폴더에서 모든 ItemData 에셋을 로드
        ItemData[] loadedEquipItems = Resources.LoadAll<ItemData>("Items");
        ItemData[] loadedRelicsItems = Resources.LoadAll<ItemData>("Relics");

        // 기존 리스트 초기화
        allItems.Clear();
        relicsItems.Clear();
        
        // 로드된 장비 아이템 추가
        foreach (var equipItem in loadedEquipItems)
        {
            allItems.Add(equipItem);
        }
        
        // 로드된 유물 아이템 추가 - 별도 리스트에 보관
        foreach (var relicsItem in loadedRelicsItems)
        {
            relicsItems.Add(relicsItem);
        }

        Debug.Log($"총 {allItems.Count}개의 일반 아이템과 {relicsItems.Count}개의 유물 아이템이 로드되었습니다.");
    }
    
    /// <summary>
    /// allItems와 relicsItems를 각각의 dictionary에 초기화
    /// </summary>
    private void InitializeItemDictionaries()
    {
        if (allItems == null)
        {
            Debug.Log("AllItems에 아이템이 없습니다.");
            return;
        }

        // 딕셔너리 초기화
        itemDictionary.Clear();
        itemsByType.Clear();
        itemsByTier.Clear();
        relicsDictionary.Clear();

        // allItems에 있는 일반 아이템을 각 딕셔너리에 분류
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
        
        // relicsItems의 유물 아이템을 별도 딕셔너리에 분류
        foreach (var relic in relicsItems)
        {
            relicsDictionary[relic.id] = relic;
            
            // itemsByType에도 유물 타입으로 추가
            if (!itemsByType.ContainsKey(ItemType.Relics))
                itemsByType[ItemType.Relics] = new List<ItemData>();
            itemsByType[ItemType.Relics].Add(relic);
        }
    }

    // ID로 아이템 찾기 (일반 아이템 + 유물)
    public ItemData GetItemById(int id)
    {
        // 먼저 일반 아이템에서 찾기
        if (itemDictionary.TryGetValue(id, out var item))
            return item;
            
        // 없으면 유물에서 찾기
        if (relicsDictionary.TryGetValue(id, out var relic))
            return relic;
            
        return null;
    }
    
    // 유물 아이템만 가져오기
    public List<ItemData> GetAllRelics()
    {
        return new List<ItemData>(relicsItems);
    }
    
    // ID로 유물 아이템 찾기
    public ItemData GetRelicById(int id)
    {
        return relicsDictionary.TryGetValue(id, out var relic) ? relic : null;
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
    
    // 유물 타입으로 아이템 리스트 가져오기
    public List<ItemData> GetRelicsByEquipType(EquipType equipType)
    {
        // 유물은 항상 EquipType.Relics이지만 혹시 다른 유형이 추가될 경우를 대비
        return relicsItems.FindAll(item => item.equipType == equipType);
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

    // 아이템 생성 (새로운 인스턴스)
    public ItemData CreateItemInstance(int id)
    {
        // 일반 아이템과 유물 모두에서 검색
        ItemData original = GetItemById(id);
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
        else if (original.itemType == ItemType.Relics)
        {
            newItem.options = new List<ItemOption>(original.options);
            // 유물은 강화 불가능
            newItem.enhancementLevel = 0;
            newItem.maxEnhancementLevel = 0;
        }
        else if (original.itemType == ItemType.Consumable)
        {
            newItem.consumableEffects = new List<ConsumableEffect>(original.consumableEffects);
            newItem.maxStack = original.maxStack;
        }

        return newItem;
    }
}