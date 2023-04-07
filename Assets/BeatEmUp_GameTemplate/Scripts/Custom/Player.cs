using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
//using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public state currentState;
    private Animator animator;
    private SpriteRenderer sr;
    public enum state
    { 
        idle,
        moving,
        jumping,
        attacking,
        blocking
    }
    private enum currentAttackMove
    { 
        lightPunch = 5, // should set these values in inspector.
        heavyPunch = 10,
        lightKick = 20,
        heavyKick = 40,
        specialMove = 80
    }

    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        currentState = state.idle;
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case state.idle:
                transitionToMove();
                transitionToAttack();
                break;
            case state.moving:
                transitionToMove();
                break;
        }
    }
    private void transitionToAttack()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack(currentAttackMove.lightPunch);  
        }
    }
    private void transitionToMove()
    {
        bool left, right, up, down;
        currentState = state.moving;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Move(new Vector2(1, 0)); right = true;
        } else right = false;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Move(new Vector2(-1, 0));left = true;
        } else left = false;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Move(new Vector2(0, 1));up = true;
        } else up = false;
        if (Input.GetKey(KeyCode.DownArrow))
        {
            Move(new Vector2(0, -1)); down = true;
        } else down = false;

        if (up == false && down == false && left == false && right == false) { currentState = state.idle; animator.SetBool("Walk", false); } // Go back to idle when no key is pressed.
    }
    private void Move(Vector2 direction) 
    {
        transform.position = new Vector2(transform.position.x + speed * Time.deltaTime * direction.x, transform.position.y + speed * Time.deltaTime * direction.y);
        if (direction.x != 0) // don't modify X sprite direction if moving along Y 
        {
            if (direction.x < 0) { sr.flipX = true; } else sr.flipX = false; // Flip the sprite to correct direction
        }
        animator.SetBool("Walk", true);
    }
    private void Attack(currentAttackMove attackMove)
    {
        currentState = state.attacking;
        animator.SetBool("Punch0", true);
        var currentFrame = GetCurrentFrame();
        if (currentFrame == 14) {
            Debug.Log("Frame Value " + currentFrame);
            currentState = state.idle;
        }
      
    }

    private int GetCurrentFrame()
    {
        AnimatorClipInfo[] animationClip = animator.GetCurrentAnimatorClipInfo(0);
        return (int)(animator.GetCurrentAnimatorStateInfo(0).normalizedTime * (animationClip[0].clip.length * animationClip[0].clip.frameRate));
    }
}
