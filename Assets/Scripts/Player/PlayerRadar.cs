using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerRadar : NetworkBehaviour
{
    [SerializeField] private RadarUI radarUI;
    [SerializeField] private FollowArrow arrow;
    [SerializeField] private float cooldown = 10f;
    [SerializeField] private float arrowDuration = 2f;
    [SerializeField] private float range = 50f;
    [SerializeField] private Animator radarAnim;

    private NetworkGamePlayerLostFound gamePlayer;

    private bool readyToSearch = false;

    private NetworkManagerLostFound room;
    private NetworkManagerLostFound Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLostFound;
        }
    }

    private bool ShouldUseRadar { get { return gamePlayer.hasAuthority && gamePlayer.PlayerType == "FINDER"; } }
    // Start is called before the first frame update
    void Start()
    {
        gamePlayer = GetComponent<NetworkGamePlayerLostFound>();
        if (gamePlayer == null)
        {
            Debug.LogError($"The component {nameof(NetworkGamePlayerLostFound)} is not found", gameObject);
            return;
        }
        radarUI.gameObject.SetActive(false);
        arrow.gameObject.SetActive(ShouldUseRadar);
        radarAnim.gameObject.SetActive(ShouldUseRadar);
        radarUI.gameObject.SetActive(false);
        readyToSearch = ShouldUseRadar;
    }

    void OnRadar()
    {
        if (hasAuthority && ShouldUseRadar && readyToSearch)
        {
            readyToSearch = false;
            StartCoroutine(StartSearchPlayers());

        }
    }

    IEnumerator StartSearchPlayers()
    {
        Searching();
        yield return new WaitForSeconds(2f);
        CmdSearchPlayers();
    }

    IEnumerator StartCooldown()
    {
        radarUI.Cooldown(cooldown);
        yield return new WaitForSeconds(cooldown);
        readyToSearch = true;
        radarUI.gameObject.SetActive(false);
    }

    IEnumerator SearchPlayersRoutine()
    {
        while (true)
        {

            yield return new WaitUntil(() => readyToSearch);
            radarUI.Cooldown(cooldown - 2f);
            yield return new WaitForSeconds(cooldown - 2f);
            Searching();
            yield return new WaitForSeconds(2f);
            CmdSearchPlayers();
            readyToSearch = false;

        }
    }

    [Command]
    private void CmdSearchPlayers()
    {
        var itemsInGame = Room.GamePlayers.Where(player => player.PlayerType == "ITEM" && !player.IsCaught);
        NetworkGamePlayerLostFound closestPlayer = null;
        var minDistance = range + 1f;
        foreach (var playerItem in itemsInGame)
        {
            var distance = Vector3.Distance(playerItem.transform.position, transform.position);
            if (distance <= range && distance < minDistance)
            {
                minDistance = distance;
                closestPlayer = playerItem;
            }
        }
        if (closestPlayer != null)
        {
            TargetPlayerFound(closestPlayer.transform);
            closestPlayer.TargetHasBeenFoundByRadar(gamePlayer);
        }
        else
        {
            TargetNoPlayerFound();
        }
    }

    [TargetRpc]
    private void TargetPlayerFound(Transform playerTransform)
    {
        radarUI.Found();
        arrow.Show(playerTransform);
        StartCoroutine(HideArrow());

    }

    [TargetRpc]
    private void TargetNoPlayerFound()
    {
        StartCoroutine(NotFoundMessage());
    }

    IEnumerator NotFoundMessage()
    {
        radarUI.NotFound();
        yield return new WaitForSeconds(2f);
        //readyToSearch = true;
        StartCoroutine(StartCooldown());
    }

    IEnumerator HideArrow()
    {
        yield return new WaitForSeconds(arrowDuration);
        arrow.Hide();
        //readyToSearch = true;
        StartCoroutine(StartCooldown());
    }

    private void Searching()
    {
        radarUI.gameObject.SetActive(ShouldUseRadar);
        radarUI.Searching();
        radarAnim.ResetTrigger("Search");
        radarAnim.SetTrigger("Search");
    }
}
