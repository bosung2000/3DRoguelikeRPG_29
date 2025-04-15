using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;

public enum SkillType
{
    Melee,
    Ranged,
    Buff,
}
public enum BuffType
{
    None,
    Heal,
    ATK,
    RES,
}

[CreateAssetMenu(fileName = "Skill", menuName = "New Skill")]
public class Skill : ScriptableObject
{
    [Header("Info")]
    public int index;
    public string _name;
    public string description;
    public float value;
    public int requiredMana;
    public float cooldown;
    public int attackRange;
    public int projectileSpeed;
    public SkillType skillType;
    public Sprite icon; // 적 아이콘
    public GameObject prefab; // instantiate 할 투사체 프리팹
    public bool isOwned; //현재 이 스킬을 갖고 있는지

    [Header("Buff variable")]
    public BuffType buffType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
