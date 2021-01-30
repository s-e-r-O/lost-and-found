using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCModelInitOffline : MonoBehaviour
{
    public GameObject hair;
    public GameObject[] characters;
    // Start is called before the first frame update
    void Start()
    {
        var modelIndex = Random.Range(0, characters.Length);
        //colorIndex = Random.Range(0, colors.Length);
        var color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        var hasHair = Random.Range(0f, 1f) > 0.5f;

        var size = Random.Range(1.6f, 1.9f);
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
}
