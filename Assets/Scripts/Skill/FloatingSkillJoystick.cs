using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingSkillJoystick : Joystick
{
    public int index;
    public SkillManager skillManager;
    public Vector3 FixedInput;

    public void Awake()
    {
        skillManager = FindAnyObjectByType<SkillManager>();

        if (skillManager != null )
        {
            Debug.Log("스킬 조작 UI에 스킬매니저가 제대로 들어갔습니다.");
        }
    }

    protected override void Start()
    {
        base.Start();
        background.gameObject.SetActive(false);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        //마지막 방향 저장
        Vector3 InputJoystick = Vector3.forward * this.Vertical + Vector3.right * this.Horizontal;
        //Vector3 InputKeyboard = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 inputDir = InputJoystick;
        if (inputDir.sqrMagnitude > 0.01f)
        {
            inputDir = inputDir.normalized;
            FixedInput = inputDir;
        }
        if (skillManager.enabledSkills[index].skill == null)
        {
            Debug.Log($"{index + 1}번째의 조이스틱에 스킬이 없습니다.");
        }
        Debug.Log($"현재 방향:{FixedInput}");
        //해당 방향에 스킬 시전
        skillManager.OnSkillClick(skillManager.enabledSkills[index].skill, FixedInput);
        //이후 해당 스킬에 쿨타임 적용

        if (skillManager.enabledSkills[index].skill != null)
        {
            skillManager.enabledSkills[index].skill.projectilePrefabs.GetComponent<SkillProjectile>().ShootBullet(skillManager.player.transform.position, inputDir);
        }
        else
        {
            Debug.Log("스킬매니저의 enabledSkills가 비어 있습니다.");
        }
        background.gameObject.SetActive(false);
        base.OnPointerUp(eventData);
    }
}
