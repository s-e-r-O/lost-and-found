
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour
    where T : MonoBehaviourSingleton<T>
{
    public static T Instance { get; private set; }

    protected virtual bool DontDestroy => true;

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            if(DontDestroy){
            Object.DontDestroyOnLoad(this.gameObject);

            }
        }
        else if (Instance != this)
        {
            Object.Destroy(this.gameObject);
        }
    }
}