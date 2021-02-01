using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCModelInit : NetworkBehaviour
{
    public Transform parent;
    public GameObject hair;
    public GameObject[] characters;

    [SyncVar]
    public int ModelIndex;

    [SyncVar]
    public Color Color;

    [SyncVar]
    public bool HasHair;

    [SyncVar]
    public float Size;


    public override void OnStartServer()
    {
        base.OnStartServer();
            GenerateRandom();
    }

    void GenerateRandom()
    {

        ModelIndex = Random.Range(0, characters.Length);
        Color = Random.ColorHSV(0f, 1f, 0.5f, 0.8f, 0.3f, 0.8f);
        HasHair = Random.Range(0f, 1f) > 0.5f;

        Size = Random.Range(1.6f, 1.9f);
        transform.localScale = Vector3.one * Size;
    }
    private void ApplyRandom()
    {
        transform.localScale = Vector3.one * Size;

        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == ModelIndex);
            if (i == ModelIndex)
            {
                //Color color = colors[colorIndex];
                SkinnedMeshRenderer mr = characters[i].GetComponent<SkinnedMeshRenderer>();
                mr.material.color = Color;
            }
        }
        hair.SetActive(HasHair);
        if (HasHair)
        {
            hair.GetComponent<MeshRenderer>().material.color = Color;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        ApplyRandom();
    }
}
