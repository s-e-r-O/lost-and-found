using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCModelInit : NetworkBehaviour
{
    public GameObject hair;
    public GameObject[] characters;

    [SyncVar]
    public int modelIndex;

    [SyncVar]
    public Color color;

    [SyncVar]
    public bool hasHair;

    [SyncVar]
    public float size;


    public override void OnStartServer()
    {
        base.OnStartServer();
            GenerateRandom();
    }

    void GenerateRandom()
    {

        modelIndex = Random.Range(0, characters.Length);
        //colorIndex = Random.Range(0, colors.Length);
        color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        hasHair = Random.Range(0f, 1f) > 0.5f;

        size = Random.Range(1.6f, 1.9f);
    }

    void ApplyRandom()
    {
        transform.localScale = Vector3.one * size;

        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == modelIndex);
            if (i == modelIndex)
            {
                //Color color = colors[colorIndex];
                SkinnedMeshRenderer mr = characters[i].GetComponent<SkinnedMeshRenderer>();
                mr.material.color = color;
            }
        }
        hair.SetActive(hasHair);
        if (hasHair)
        {
            hair.GetComponent<MeshRenderer>().material.color = color;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
            ApplyRandom();
        
    }
}
