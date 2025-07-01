﻿using System;
using System.Collections;
using APA.Core;
using Unity.VisualScripting;
using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    [Header("Key Bindings")]
    [SerializeField] private KeyCode moveLeftKey = KeyCode.A;
    [SerializeField] private KeyCode moveRightKey = KeyCode.D;
    [SerializeField] private KeyCode jumpKey = KeyCode.W;

    [Header("Movement Settings")]
    public float acceleration = 50f;
    public float maxSpeed = 7f;
    public float jumpForce = 12f;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    protected Rigidbody2D rb;
    private Animator anim;

    private bool isTryJump;
    private float horizontalInput;
    private bool isGrounded;
    private bool wasInAir = false;

    protected Coroutine coroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coroutine = StartCoroutine(FixedUpdateCoroutine());
    }
    void Update()
    {
        if (Input.GetKey(moveRightKey))
            horizontalInput = 1f;
        else if (Input.GetKey(moveLeftKey))
            horizontalInput = -1f;
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            horizontalInput = 0f;
        }

        isTryJump = Input.GetKey(jumpKey);
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        UpdateAnimations();
        FlipSprite();
    }
    protected IEnumerator FixedUpdateCoroutine()
    {
        var waitForFixedUpdate = new WaitForFixedUpdate();
        while (true)
        {
            if (horizontalInput != 0)
            {
                rb.AddForce(new Vector2(horizontalInput * acceleration, 0f));
            }

            float clampedX = Mathf.Clamp(rb.linearVelocity.x, -maxSpeed, maxSpeed);
            rb.linearVelocity = new Vector2(clampedX, rb.linearVelocity.y);

            if (isGrounded && isTryJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                // anim.SetTrigger("JumpTrigger");
                wasInAir = true;
            }

            yield return waitForFixedUpdate;
        }
    }
    private void UpdateAnimations()
    {
        float speedX = Mathf.Abs(rb.linearVelocity.x);
        anim.SetFloat("Speed", speedX);

        // if (!isGrounded && !wasInAir)
        // {
        //     // anim.SetTrigger("JumpTrigger");
        //     wasInAir = true;    
        // }

        if (isGrounded && wasInAir)
        {
            wasInAir = false;
            // anim.ResetTrigger("JumpTrigger");
        }
    }
    private void FlipSprite()
    {
        if (horizontalInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (horizontalInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}