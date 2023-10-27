using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviourPun
{
    [HideInInspector]
    public int id;

    [Header("Info")]
    public float moveSpeed;
    public int curAttackerId;
    public int kills;
    public int gold;
    public int curHp;
    public int maxHp;
    public bool dead;
    public bool DisableControls;
    public bool facingLeft;
    //public float bounceForce;


    private bool SlideCheck;

    [Header("Weapon Stats")]
    public bool weaponThrow;
    public Sprite SwordSprite;
    public Sprite AxeSprite;
    public SpriteRenderer CurrentWeapon;

    [Header("Attack")]
    public int damage;
    public float attackRange;
    public float attackRate;
    private float lastAttackTime;
    public GameObject WeaponProjectilePefab;
    public float throwSpeed;

    [Header("Components")]
    public Rigidbody2D rig;
    public Player photonPlayer;
    public SpriteRenderer sr;
    public Animator weaponAnim;
    public HeaderInfo headerInfo;

    // local player
    public static PlayerController me;

    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;
        GameManager.instance.players[id - 1] = this;

        gold = 0;
        // initialize the health bar
        headerInfo.Initialize(player.NickName, maxHp);

        if (player.IsLocal)
            me = this;
        else
            rig.isKinematic = true;

    }


    void Update()
    {
        if (!photonView.IsMine)
            return;

        if (DisableControls)
        {
            if (SlideCheck && rig.velocity.magnitude != 0)
            {
                rig.velocity = new Vector2(0, 0);
                SlideCheck = false;
            }
            return;
        }

        Move();

        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime > attackRate)
            Attack();

        float mouseX = (Screen.width / 2) - Input.mousePosition.x;


        if (mouseX < 0 && !facingLeft)
            photonView.RPC("ChangeDirection", RpcTarget.All, "left", id);
        else if (mouseX >= 0 && facingLeft)
            photonView.RPC("ChangeDirection", RpcTarget.All, "right", id);


    }

    [PunRPC]
    void ChangeDirection(string directionToFace, int id)
    {
        PlayerController player = GameManager.instance.GetPlayer(id);
        Debug.Log(directionToFace);
        if (directionToFace == "right")
        {
            player.weaponAnim.transform.parent.localScale = new Vector3(-1, 1, 1);
            facingLeft = false;
        }
        else if (directionToFace == "left")
        {
            player.weaponAnim.transform.parent.localScale = new Vector3(1, 1, 1);
            facingLeft = true;
        }


    }

    [PunRPC]
    public void ChangeWeapons(string weapon, int playerId)
    {
        PlayerController player = GameManager.instance.GetPlayer(playerId);
        switch (weapon)
        {
            case "Sword":
                player.CurrentWeapon.sprite = SwordSprite;
                player.weaponThrow = false;
                break;
            case "Axe":
                player.CurrentWeapon.sprite = AxeSprite;
                player.weaponThrow = true;
                break;
        }
    }

    void Move()
    {
        if (!SlideCheck)
        {
            SlideCheck = true;
        }
        // get the horizontal and vertical inputs
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        rig.velocity = new Vector2(x, y) * moveSpeed;
    }

    void Attack()
    {
        lastAttackTime = Time.time;

        // calculate the direction
        Vector3 dir = (Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position)).normalized;

        // shoot a raycast in the direction
        RaycastHit2D hit = Physics2D.Raycast(transform.position + dir, dir, attackRange);

        
        // did we hit an enemy?
        if (hit.collider != null && hit.collider.gameObject.CompareTag("Enemy"))
        {
            // get the enemy and damage them
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            enemy.photonView.RPC("TakeDamage", RpcTarget.MasterClient, damage);
        }


        // play the attack animation
        photonView.RPC("PlayAttackAnimation", RpcTarget.All, id, dir);
    }

    void ThrowWeapon(Vector3 direction)
    {
        GameObject weaponObj = Instantiate(WeaponProjectilePefab, transform.position + direction, Quaternion.identity);
        if (facingLeft)
        {
            weaponObj.transform.localScale = new Vector3(-1, 1, 1);
        }
        weaponObj.GetComponent<Rigidbody2D>().velocity = direction * throwSpeed;

        weaponObj.GetComponent<ProjectileScript>().Initialize(id, this.photonView.IsMine);
    }


    [PunRPC]
    void PlayAttackAnimation(int id, Vector3 dir)
    {
        PlayerController player = GameManager.instance.GetPlayer(id);
        player.weaponAnim.SetTrigger("Attack");

        if (weaponThrow)
        {
            StartCoroutine(DisableWeaponInHand());

            IEnumerator DisableWeaponInHand()
            {
                yield return new WaitForSeconds(0.2f);
                ThrowWeapon(dir);
                CurrentWeapon.gameObject.SetActive(false);
                yield return new WaitForSeconds(0.3f);
                CurrentWeapon.gameObject.SetActive(true);
            }
        }
    }

    [PunRPC]
    public void TakeDamage(int attackerid, int damage)
    {
        curHp -= damage;

        curAttackerId = attackerid;
        // update the health bar
        headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHp);

        if (curHp <= 0)
            Die();
        else
        {
            photonView.RPC("FlashDamage", RpcTarget.All);
        }
    }

    [PunRPC]
    void FlashDamage()
    {
        StartCoroutine(DamageFlash());

        IEnumerator DamageFlash()
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            sr.color = Color.white;
        }
    }

    void Die()
    {
        curHp = 0;
        dead = true;
        rig.isKinematic = true;

        photonView.RPC("GiveGold", RpcTarget.All, -50, this.id);

        transform.position = new Vector3(0, 99, 0);

        if (curAttackerId != 0 && SceneManager.GetActiveScene().name == "AxeThrowers")
            GameManager.instance.GetPlayer(curAttackerId).photonView.RPC("AddKill", RpcTarget.All, curAttackerId);

        Vector3 spawnPos = GameManager.instance.spawnPoints[Random.Range(0, GameManager.instance.spawnPoints.Length)].position;
        StartCoroutine(Spawn(spawnPos, GameManager.instance.respawnTime));
    }

    [PunRPC]
    void AddKill(int playerId)
    {
        PlayerController player = GameManager.instance.GetPlayer(playerId);
        player.kills++;

        // update ui
        if (player == me)
            AxeThrowersUI.instance.UpdateKillText(kills);
    }

    IEnumerator Spawn(Vector3 spawnPos, float timeToSpawn)
    {
        yield return new WaitForSeconds(timeToSpawn);

        dead = false;
        transform.position = spawnPos;
        curHp = maxHp;
        rig.isKinematic = false;

        // update the health bar
        headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHp);
    }

    [PunRPC]
    void Heal(int ammountToHeal)
    {
        curHp = Mathf.Clamp(curHp + ammountToHeal, 0, maxHp);

        // update the health bar
        headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHp);
    }

    [PunRPC]
    void GiveGold(int goldToGive, int playerid)
    {
        PlayerController player = GameManager.instance.GetPlayer(playerid);
        player.gold += goldToGive;


        // update UI if this is us
        if (player == me)
        {
            if (GameUI.instance != null)
            {
                GameUI.instance.UpdateGoldText(gold);
            }
            else if (GoldGrindersUI.instance != null)
            {
                GoldGrindersUI.instance.UpdateGoldText(gold);
            }
        }

    }
}

    //potential bounce mechanic
/*    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Players Collided");
            if (PlayerController.m)
                rig.velocity = (new Vector2(collision.gameObject.transform.position.x, collision.gameObject.transform.position.y) - new Vector2(transform.position.x, transform.position.y)) * bounceForce;
        }
    }
}*/


//new Vector2(collision.gameObject.transform.position.x, collision.gameObject.transform.position.y) - new Vector2(transform.position.x, transform.position.y)