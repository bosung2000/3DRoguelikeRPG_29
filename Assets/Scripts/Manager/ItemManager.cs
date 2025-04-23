using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    private static ItemManager instance;
    public static ItemManager Instance => instance;

    [SerializeField] private List<ItemData> allEquipItems = new List<ItemData>(); // 유물 제외한 일반 아이템
    [SerializeField] private List<ItemData> allrelicsItems = new List<ItemData>(); // 유물 아이템 전용 리스트

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

    private void Start()
    {
        
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
        allEquipItems.Clear();
        allrelicsItems.Clear();

        // 로드된 장비 아이템 추가 (원본 복사본 생성)
        foreach (var originalItem in loadedEquipItems)
        {
            // 아이템 인스턴스 생성 (원본을 직접 수정하지 않기 위해)
            ItemData itemInstance = CreateItemInstanceFromOriginal(originalItem);
            allEquipItems.Add(itemInstance);
        }

        // 로드된 유물 아이템 추가 - 별도 리스트에 보관 (원본 복사본 생성)
        foreach (var originalRelic in loadedRelicsItems)
        {
            // 유물 인스턴스 생성
            ItemData relicInstance = CreateItemInstanceFromOriginal(originalRelic);
            allrelicsItems.Add(relicInstance);
        }

        Debug.Log($"총 {allEquipItems.Count}개의 일반 아이템과 {allrelicsItems.Count}개의 유물 아이템이 로드되었습니다.");
    }

    /// <summary>
    /// allItems와 relicsItems를 각각의 dictionary에 초기화
    /// </summary>
    private void InitializeItemDictionaries()
    {
        if (allEquipItems == null)
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
        foreach (var item in allEquipItems)
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
        foreach (var relic in allrelicsItems)
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
        return new List<ItemData>(allrelicsItems);
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
        return allrelicsItems.FindAll(item => item.equipType == equipType);
    }

    // 유물 중에서 3개를 랜덤으로 뽑아 반환
    public List<ItemData> GetRelicsByRandom(int count = 3)
    {
        if (allrelicsItems == null || allrelicsItems.Count == 0)
            return new List<ItemData>();

        // 결과를 저장할 리스트
        List<ItemData> randomRelics = new List<ItemData>();

        // 유물이 3개 미만인 경우, 모든 유물 반환
        if (allrelicsItems.Count <= count)
            return new List<ItemData>(allrelicsItems);

        // 랜덤으로 유물 선택을 위해 원본 리스트를 복사
        List<ItemData> tempList = new List<ItemData>(allrelicsItems);

        // Fisher-Yates 셔플 알고리즘으로 랜덤하게 섞기
        for (int i = 0; i < count; i++)
        {
            // 남은 항목 중에서 랜덤으로 하나 선택
            int randomIndex = UnityEngine.Random.Range(0, tempList.Count);

            // 결과 리스트에 추가
            randomRelics.Add(tempList[randomIndex]);

            // 선택된 항목 제거 (중복 방지)
            tempList.RemoveAt(randomIndex);
        }

        return randomRelics;
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

    //아이템 판매시 list에서 추가
    public void AddItemList(ItemData itemData)
    {
        // 강화 수치 초기화
        ResetEnhancement(itemData);
        
        if (itemData.itemType == ItemType.Equipment)
        {
            if (!allEquipItems.Contains(itemData))
            {
                allEquipItems.Add(itemData);
            }
        }
        else if (itemData.itemType == ItemType.Relics)
        {
            if (!allrelicsItems.Contains(itemData))
            {
                allrelicsItems.Add(itemData);
            }
        }
        else
        {
            Debug.Log("잘못된 아이템 입니다.");
        }
    }
    
    // 아이템 강화 수치 초기화 메서드
    private void ResetEnhancement(ItemData itemData)
    {
        if (itemData == null) return;
        
        // ItemData 클래스의 ResetEnhancement 메서드 호출
        itemData.ResetEnhancement();
    }

    //아이템 구매시 list에서 제거 
    public void RemoveItemList(ItemData itemData)
    {
        if (itemData.itemType == ItemType.Equipment)
        {
            if (allEquipItems.Contains(itemData))
            {
                allEquipItems.Remove(itemData);
            }
        }
        else if (itemData.itemType == ItemType.Relics)
        {
            if (allrelicsItems.Contains(itemData))
            {
                allrelicsItems.Remove(itemData);
            }
        }
        else
        {
            Debug.Log("잘못된 아이템 입니다.");
        }
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

    /// <summary>
    /// 원본 아이템을 기반으로 새 인스턴스를 생성합니다.
    /// </summary>
    private ItemData CreateItemInstanceFromOriginal(ItemData original)
    {
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
            newItem.enhancementLevel = 0; // 강화 수치 초기화
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