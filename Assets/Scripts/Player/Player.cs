using UnityEngine;


public class Player : MonoBehaviour
{
    [SerializeField] PlayerStatData statData;
    public PlayerStat _playerStat;
    [SerializeField] PlayerController _playerController;
    private float _lastHitTime = 0f;
    [SerializeField] private PlayerSkillController playerSkillController;
    public GameObject FireCombo_collider;
    private void Awake()
    {
        _playerStat = GetComponent<PlayerStat>();
        _playerController = GetComponent<PlayerController>();
        playerSkillController= GetComponent<PlayerSkillController>();
    }
    private void Start()
    {
        _playerStat.InitBaseStat(statData);
    }
    public void Update()
    {
        _playerController.DirectionCheck();
    }
    public void Attack()
    {
        AnimatorStateInfo currentState = _playerController._anim.GetCurrentAnimatorStateInfo(0);

        if (currentState.IsName("GetHit"))
            return;

        float attackSpeed = _playerStat.GetStatValue(PlayerStatType.AttackSpeed);
        float attackCooldown = 1 / attackSpeed;
        if (Time.time < _lastHitTime+attackCooldown) return;

        // 공격 시 플레이어의 방향을 마지막 이동 방향으로 설정
        Vector3 lastMoveDir = _playerController.GetLastMoveDirection();
        if (lastMoveDir.sqrMagnitude > 0.05f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lastMoveDir);
            transform.rotation = targetRotation;
        }

        _playerController.SetTrigger("Attack");
        _lastHitTime = Time.time;

        //튜토리얼 체크
        FindObjectOfType<TutorialController>()?.OnAttackPerformed();
    }

    public void Dash()
    {
        _playerController.Dash();
    }
    public void ActiveSkill()
    {
        GameManager.Instance.SkillManager.ActiveSkill();
    }
    public void SetActiveSkilltrue()
    {
        GameManager.Instance.SkillManager.SetActiveSkilltrue();
    }
    public void SetActiveSkillfalse()
    {
         GameManager.Instance.SkillManager.SetActiveSkillfalse();
    }
    public void AttackSound()
    {
        SoundManager.instance.PlayEffect(SoundEffectType.Attack);
    }
    public void FireComboSound()
    {
        SoundManager.instance.PlayEffect(SoundEffectType.FireCombo);
    }
    public void LightingSound()
    {
        SoundManager.instance.PlayEffect(SoundEffectType.Lighting);
    }
    public void GreenSlashSound()
    {
        SoundManager.instance.PlayEffect(SoundEffectType.GreenSlash);
    }
    public void RollSound()
    {
        SoundManager.instance.PlayEffect(SoundEffectType.Roll);
    }
}
