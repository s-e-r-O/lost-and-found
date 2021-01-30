using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    [SerializeField]
    Transform[] startingPositions;
    [SerializeField]
    Transform[] spawnerStartingPositions;

    [SerializeField]
    NPCSpawnerOffline spawner;

    void Awake()
    {
        var index = Random.Range(0, startingPositions.Length);
        transform.position = startingPositions[index].position;
        transform.rotation= startingPositions[index].rotation;

        spawner.transform.position = spawnerStartingPositions[index].position;
        spawner.transform.rotation = spawnerStartingPositions[index].rotation;
    }
}
