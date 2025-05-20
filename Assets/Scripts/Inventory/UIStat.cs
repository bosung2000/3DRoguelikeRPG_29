using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIStat : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI StatsSlotPrefab;
    [SerializeField] private Transform SlotParent;
    private List<TextMeshProUGUI> StatSlotList;

    PlayerStat playerStat;
    private void Awake()
    {

    }
    private void Start()
    {

    }

    private void OnEnable()
    {
        playerStat = GameManager.Instance.PlayerManager.Player._playerStat;
        playerStat.OnStatsChanged += OnupdateStat;
        initupdatestatui();
    }

    private void initupdatestatui()
    {
        OnupdateStat(playerStat);
    }

    private void OnupdateStat(PlayerStat _playerStat)
    {
        RemoveSlots();
        StatSlotList = new List<TextMeshProUGUI>();

        // PlayerStatType enum의 모든 값을 가져와서 순회
        foreach (PlayerStatType type in System.Enum.GetValues(typeof(PlayerStatType)))
        {
            // 각 스탯 타입에 대한 값 가져오기
            float value = _playerStat.GetStatValue(type);

            // 값이 0인 스탯은 표시하지 않을 수도 있음 (선택 사항)
            // if (Mathf.Approximately(value, 0f)) continue;

            // 슬롯 생성
            TextMeshProUGUI slot = Instantiate(StatsSlotPrefab, SlotParent);

            // 스탯 타입에 따른 이름 가져오기
            string statName = GetStatDisplayName(type);

            // 스탯 값 형식 지정
            string formattedValue = FormatStatValue(type, value);

            // 텍스트 설정
            slot.text = $"{statName}: {formattedValue}";

            // 색상 설정
            slot.color = GetStatColor(type);

            StatSlotList.Add(slot);
        }
    }

    // 스탯 타입에 따른 표시 이름 반환
    private string GetStatDisplayName(PlayerStatType type)
    {
        switch (type)
        {
            case PlayerStatType.Attack:
                return "공격력";
            case PlayerStatType.MaxHP:
                return "최대 체력";
            case PlayerStatType.HP:
                return "현재 체력";
            case PlayerStatType.MaxMP:
                return "최대 마나";
            case PlayerStatType.MP:
                return "현재 마나";
            case PlayerStatType.MoveSpeed:
                return "이동 속도";
            case PlayerStatType.DMGReduction:
                return "받는 데미지 감소";
            case PlayerStatType.CriticalChance:
                return "치명타 확률";
            case PlayerStatType.CriticalDamage:
                return "치명타 피해";
            case PlayerStatType.absorp:
                return "흡혈량";
            case PlayerStatType.DMGIncrease:
                return "데미지 증가";
            case PlayerStatType.HPRecovery:
                return "HP 회복";
            case PlayerStatType.MPRecovery:
                return "MP 회복";
            case PlayerStatType.GoldAcquisition:
                return "골드 획득량";
            case PlayerStatType.SkillCooltime:
                return "스킬 쿨타임 감소";
            case PlayerStatType.AttackSpeed:
                return "공격 속도";
            default:
                return type.ToString();
        }
    }

    // 스탯 값 형식화 (타입에 따라 다르게 표시)
    private string FormatStatValue(PlayerStatType type, float value)
    {
        // 퍼센트로 표시해야 하는 타입들
        if (type == PlayerStatType.CriticalChance ||
            type == PlayerStatType.CriticalDamage ||
            type == PlayerStatType.DMGReduction ||
            type == PlayerStatType.absorp ||
            type == PlayerStatType.DMGIncrease ||
            type == PlayerStatType.GoldAcquisition ||
            type == PlayerStatType.SkillCooltime ||
            type == PlayerStatType.AttackSpeed)
        {
            return $"{value:F1}%"; // 소수점 한 자리까지 표시하고 % 추가
        }

        // 체력, 마나, 공격력 등 정수로 표시할 타입들
        if (type == PlayerStatType.MaxHP ||
            type == PlayerStatType.HP ||
            type == PlayerStatType.MaxMP ||
            type == PlayerStatType.MP ||
            type == PlayerStatType.Attack ||
            type == PlayerStatType.HPRecovery ||
            type == PlayerStatType.MPRecovery)
        {
            return $"{Mathf.RoundToInt(value)}"; // 정수로 반올림하여 표시
        }

        // 기타 값들은 소수점 한 자리까지 표시
        return $"{value:F1}";
    }

    // 스탯 타입에 따른 색상 반환 (선택 사항)
    private Color GetStatColor(PlayerStatType type)
    {
        // 기본값은 흰색으로 설정
        Color statColor = Color.white;

        // 타입에 따라 다른 색상 지정 (원하는 경우 수정)
        switch (type)
        {
            case PlayerStatType.Attack:
                statColor = new Color(0.95f, 0.2f, 0.2f); // 빨간색
                break;
            case PlayerStatType.MaxHP:
            case PlayerStatType.HP:
                statColor = new Color(0.2f, 0.8f, 0.2f); // 녹색
                break;
            case PlayerStatType.MaxMP:
            case PlayerStatType.MP:
                statColor = new Color(0.2f, 0.4f, 0.9f); // 파란색
                break;
            case PlayerStatType.CriticalChance:
            case PlayerStatType.CriticalDamage:
                statColor = new Color(0.9f, 0.7f, 0.1f); // 노란색
                break;
                // 더 많은 타입에 대한 색상 설정 가능
        }

        return statColor;
    }

    private void RemoveSlots()
    {
        if (StatSlotList == null)
        {
            return;
        }

        // 모든 슬롯 오브젝트 파괴
        foreach (var slot in StatSlotList)
        {
            if (slot != null)
            {
                Destroy(slot.gameObject);
            }
        }

    }


}
