﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public float speed;
    public float rotateSpeed;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode rotateLeft;
    public KeyCode rotateRight;
    public KeyCode shootButton;
    public KeyCode defenceKey;
    public Text playerTitle;
    public Text energyText;
    public float jumpForce;
    public string playerName;
    public float shootForce;
    public Transform rotatePoint;
    public GameObject bullet;
    public GameObject shield;
    public GameObject deathParticle;
    public Color color;
    public int maxJumpNum = 5;
    // public GameObject bulletGameManager;
    public int EnergyCount { get; set; }

    private AudioSource[] sound;
    private float rotateClockwise = 1;
    private float moveHorizontal = 0;
    private Rigidbody2D rb2d;
    private Vector3 zAxis;
    private int jumpCollectCounter = 0;
    private Animator animator;
    private bool isJump = false;
    private SpriteRenderer sprite;
    

	void Start () {
        color = new Color(Random.Range(0.7f, 1f), Random.Range(0.7f, 1f), Random.Range(0.7f, 1f));
        sound = GetComponents<AudioSource>();
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        zAxis = new Vector3(0, 0, 1);
        playerTitle = transform.GetChild(2).GetChild(0).GetComponent<Text>();
        playerTitle.text = playerName;
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = color;
	}
	
	void Update () {
        sprite.color = color;
        MoveMementControl();
        RotateShootPoint();
        ShootBullet();
        DefenceShield();
        PlayerAnimation();
    }

    private void PlayerAnimation()
    {
        if (rb2d.velocity.y > 0)
            animator.SetBool("isJump", true);
        else
            animator.SetBool("isJump", false);

    }

    private void ShootBullet()
    {
        // var bulletCount = bulletGameManager.GetComponent<BulletManager>().PlayerLaser[playerName];
        if (Input.GetKey(shootButton) && rotatePoint.childCount < 1 && transform.parent.childCount < 2)
        {
            //Debug.Log(playerName + " " + bulletCount);
            var child = (GameObject)Instantiate(bullet, rotatePoint.position, Quaternion.Euler(Vector3.zero));
            child.GetComponent<LaserController>().PlayerName = playerName;
            // child.transform.parent = bulletGameManager.transform;
            child.transform.parent = transform.parent;
            var childRb2d = child.GetComponent<Rigidbody2D>();
            childRb2d.velocity = Vector2.zero;
            var direction = rotatePoint.position - transform.position;
            var shootingSpeed = direction.normalized * shootForce;
            childRb2d.AddForce(shootingSpeed * (EnergyCount + 1));
            EnergyCount = 0;
            sound[0].Play();
             energyText.text = playerName + " energy: " + EnergyCount;   
            // Debug.Log("Energy: " + EnergyCount);
            // bulletGameManager.GetComponent<BulletManager>().PlayerLaser[playerName]++;
        }
    }

    private void DefenceShield()
    {
        // Debug.Log("Shield num: " + rotatePoint.childCount);
        if (Input.GetKey(defenceKey) && rotatePoint.childCount == 0)
        {
            var child = (GameObject)Instantiate(shield, rotatePoint.position, rotatePoint.rotation);
            child.transform.parent = rotatePoint;
        }
    }

    private void RotateShootPoint()
    {
        if (Input.GetKey(rotateLeft))
            rotateClockwise = -1;
        else if (Input.GetKey(rotateRight))
            rotateClockwise = 1;
        else
            rotateClockwise = 0;
        rotatePoint.RotateAround(transform.position, zAxis, rotateSpeed * rotateClockwise);
    }

    private void MoveMementControl()
    {
        if (Input.GetKey(leftKey))
            moveHorizontal = -1;
        else if (Input.GetKey(rightKey))
            moveHorizontal = 1;
        else
            moveHorizontal = 0;
        rb2d.velocity = new Vector2(moveHorizontal * speed, rb2d.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (jumpCollectCounter < maxJumpNum && collision.CompareTag("PowerUp"))
        {
            sound[1].Play();
            jumpCollectCounter++;
            if (rb2d.velocity.y > 0)
                rb2d.AddForce(new Vector2(0, jumpForce));
            else
                rb2d.AddForce(new Vector2(0, -jumpForce));
            
            Destroy(collision.gameObject);
        }

        else if (jumpCollectCounter < maxJumpNum && collision.CompareTag("Energy"))
        {
            EnergyCount++;
            Debug.Log(playerName + " energy count: " + EnergyCount);
            energyText.text = playerName + " energy: " + EnergyCount;
            Destroy(collision.gameObject);
            sound[1].Play();
        }

        else if (collision.CompareTag("Head"))
        {
            Instantiate(deathParticle, collision.transform.position, Quaternion.identity);
            var otherPlayer = collision.transform.parent.gameObject;
            Destroy(otherPlayer.transform.parent.gameObject);
            Debug.Log(playerName + " Win!");
            sound[3].Play();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Laser"))
        {
            Instantiate(deathParticle, transform.position, Quaternion.identity);
            Destroy(transform.parent.gameObject);
            sound[3].Play();
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            sound[2].Play();
        }
            
    }
}
