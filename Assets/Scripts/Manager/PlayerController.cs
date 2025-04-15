using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator anim;

    public void SetAnimatorBool(string name, bool value)
    {
        anim.SetBool(name, value);
    }
}
