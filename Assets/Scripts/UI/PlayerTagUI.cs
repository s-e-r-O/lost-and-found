using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTagUI : CanvasFollow
{
    public void Init(Transform target, string name, Color color)
    {
        this.target = target;
        text.text = name;
        text.color = color;
    }
}
