using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviourSingleton<SceneTransition>
{
    public Animator Anim { get; private set; }

    private void Start()
    {
        Anim = GetComponent<Animator>();
    }

    public void Open()
    {
        Anim.ResetTrigger("Open");
        Anim.SetTrigger("Open");
        Anim.ResetTrigger("Close");
    }

    public void Close()
    {
        Anim.ResetTrigger("Close");
        Anim.SetTrigger("Close");
        Anim.ResetTrigger("Open");
    }
}
