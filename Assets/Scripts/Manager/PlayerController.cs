using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator _anim;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }
    public void SetBool(string name, bool value)
    {
        _anim.SetBool(name, value);
    }
    public void SetTrigger(string name)
    {
        _anim.SetTrigger(name);
    }

    public void SetFloat(string name , float speed)
    {
        _anim.SetFloat(name, speed);
    }

}
