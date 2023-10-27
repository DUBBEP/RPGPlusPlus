using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum PickupType
{
    Gold,
    Health,
    Axe
}
public class Pickup : MonoBehaviourPun
{
    public PickupType type;
    public int value;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!PhotonNetwork.IsMasterClient)
            return;
        if(collision.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            if (type == PickupType.Gold)
                player.photonView.RPC("GiveGold", RpcTarget.All, value, player.id);
            else if (type == PickupType.Health)
                player.photonView.RPC("Heal", player.photonPlayer, value);
            else if (type == PickupType.Axe)
                player.photonView.RPC("ChangeWeapons", RpcTarget.All, "Axe", player.id);

            PhotonNetwork.Destroy(gameObject);
        }
    }
}
