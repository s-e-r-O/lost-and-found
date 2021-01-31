using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChange : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.ChangeBackgroundMusic("Gameplay");
    }
}
