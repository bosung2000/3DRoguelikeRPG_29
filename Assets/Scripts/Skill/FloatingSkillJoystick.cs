using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class FloatingSkillJoystick : Joystick
{
    public int index;
    public SkillManager skillManager;
    public Vector3 FixedInput;
    private Player _player;
    public void Awake()
    {
        skillManager = FindAnyObjectByType<SkillManager>();

    }

    protected override void Start()
    {
        base.Start();
        background.gameObject.SetActive(false);
        _player = FindObjectOfType<Player>();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (skillManager._isBoolSkill == true)
        {
            return;
        }
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (skillManager._isBoolSkill == true)
        {
            return;
        }
        //마지막 방향 저장
        Vector3 InputJoystick = Vector3.forward * this.Vertical + Vector3.right * this.Horizontal;
        //Vector3 InputKeyboard = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 inputDir = InputJoystick;

        if (inputDir.sqrMagnitude > 0.01f)
        {
            inputDir = inputDir.normalized;
            FixedInput = inputDir;
        }
        StartCoroutine(SetPlayerSkillDirection(FixedInput));

        //_player.transform.rotation = Quaternion.LookRotation(FixedInput);

        //var rb = _player.GetComponent<Rigidbody>();
        //if (rb != null)
        //{
        //    rb.velocity = Vector3.zero;
        //    rb.angularVelocity = Vector3.zero;
        //}

        // 안전하게 스킬 접근
        Skill currentSkill = skillManager.GetSkillAtSlot(index);
        
        if (currentSkill == null)
        {
            Debug.Log($"{index + 1}번째의 조이스틱에 스킬이 없습니다.");
        }
        else
        {
            //Debug.Log($"현재 방향:{FixedInput}");
            //해당 방향에 스킬 시전
            skillManager.InitSkillData(currentSkill, FixedInput);
            if (skillManager.IsCkSkill())
            {
                if (currentSkill._name == "Lighting")
                {
                    _player.GetComponent<PlayerController>().SetTrigger("Lighting");
                }
                else if (currentSkill._name == "GreenSlash")
                {
                    _player.GetComponent<PlayerController>().SetTrigger("GreenSlash");
                }
                else if (currentSkill._name == "FireCombo")
                {
                    _player.GetComponent<PlayerController>().SetTrigger("FireCombo");
                }
            }
        }
        
        background.gameObject.SetActive(false);
        base.OnPointerUp(eventData);
    }
    private IEnumerator SetPlayerSkillDirection(Vector3 direction)
    {
        yield return new WaitForFixedUpdate(); // FixedUpdate 이후 회전

        if (_player == null || direction == Vector3.zero)
            yield break;

        direction.y = 0f;
        _player.transform.rotation = Quaternion.LookRotation(direction);

        var rb = _player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        var controller = _player.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.LockRotationFor(0.2f); // 0.2초간 DirectionCheck() 회전 막음
        }
    }
}
