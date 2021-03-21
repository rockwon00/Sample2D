﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Audio;

public class PlayerMove : MonoBehaviour
{

    public GameManager gameManager;
    public float maxSpeed;
    public float jumpPower;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamage;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D capsuleCollider;
    AudioSource audiosource;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        audiosource = GetComponent<AudioSource>();

    }

    public void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audiosource.clip = audioJump;
                break;
            case "ATTACK":
                audiosource.clip = audioAttack;
                break;
            case "DAMAGED":
                audiosource.clip = audioDamage;
                break;
            case "ITEM":
                audiosource.clip = audioItem;
                break;
            case "DIE":
                audiosource.clip = audioDie;
                break;
            case "FINISH":
                audiosource.clip = audioFinish;
                break;
        }
        audiosource.Play();
    }


    void Update()
    {
        //jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
            PlaySound("JUMP");
        }

        //stop speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }
        //direction sprite
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        //animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);
    }

    void FixedUpdate()
    {
        //move speed
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h * 2, ForceMode2D.Impulse);

        //max speed
        if (rigid.velocity.x > maxSpeed)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1))
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        //landing platform
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayhit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (rayhit.collider != null)
            {
                if (rayhit.distance < 0.5f)
                    anim.SetBool("isJumping", false);
                //Debug.Log(rayhit.collider.name);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            //몬스터 보다 위에 있고, 아래로 낙하 중 = 밟음 (공격)
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
                PlaySound("ATTACK");
            }
            else
                OnDamege(collision.transform.position);
            PlaySound("DAMAGED");
            Debug.Log("플레이어가 맞았습니다!");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            //point
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if (isBronze)
                gameManager.stagePoint += 50;
            else if (isSilver)
                gameManager.stagePoint += 100;
            else if (isGold)
                gameManager.stagePoint += 150;
            PlaySound("ITEM");


            //deactive Item
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Finish")
        {
            //next stage
            PlaySound("FINISH");
            gameManager.NextStage();
        }
    }

    void OnAttack(Transform enemy)
    {
        //Point
        gameManager.stagePoint += 100;
        //reaction force
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
    }

    void OnDamege(Vector2 targetPos)
    {
        //health Down
        gameManager.HealthDown();
        //change layer 
        gameObject.layer = 11;

        //view alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //Reaction Force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);
        Invoke("OffDamage", 3);

    }

    void OffDamage()
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);

    }

    public void OnDie()
    {
        //Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //Sprite Flip Y
        spriteRenderer.flipY = true;
        //Collider Disable 
        capsuleCollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

}


