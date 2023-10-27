using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ProjectileScript : MonoBehaviourPun
{
    [Header("Stats")]
    public int damage;
    private int attackerId;
    private bool isMine;

    public void Initialize(int attackerId, bool isMine)
    {
        this.attackerId = attackerId;
        this.isMine = isMine;
        
        Destroy(this.gameObject, 5f);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isMine)
        {
            PlayerController player = GameManager.instance.GetPlayer(collision.gameObject);

            if (player.id != attackerId)
            {
                player.photonView.RPC("TakeDamage", player.photonPlayer, attackerId, damage);

                Destroy(this.gameObject);
            }

        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            enemy.photonView.RPC("TakeDamage", RpcTarget.MasterClient, damage);
        }
    }
}
