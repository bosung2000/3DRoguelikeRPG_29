using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator _anim;

    public void SetBool(string name, bool value)
    {
        _anim.SetBool(name, value);
    }
    public void SetTrigger(string name)
    {
        if (!_anim.GetCurrentAnimatorStateInfo(0).IsName(name))
        {
            _anim.SetTrigger(name);
        }
    }
}
