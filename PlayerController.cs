using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //Start Variables

    //Rigidbody2D for the spirte body
    private Rigidbody2D rb;
    //Animator for the sprite animations
    private Animator anim;
    //Collider2D for sprite collitions
    private Collider2D coll;

    //States for player
    //Idle = (0)standing still
    //Running = (1)moving
    //Jumping = (2)Jumping in air or off enemy
    //Falling = (3)Falling
    //Hurt = (4)touching enemy
    private enum State {idle, running, jumping, falling, hurt}
    private State state = State.idle;

    //Inspector Variables
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private int cherries = 0;
    [SerializeField] private Text cherryText;
    [SerializeField] private float hurtForce = 10f;
    

    //Components for code implementation
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
    }

    //In game updates for player state
    private void Update()
    {
        //If player gets hurt he moves in opposite direction of enemy
        if(state != State.hurt)
        {
            Movement();
        }
        AnimationState();
        anim.SetInteger("state", (int)state);
    }

    //Collectable Objects in Game
    private void OnTriggerEnter2D(Collider2D collision)
    {
       if(collision.tag == "Collectible")
        {
            //When you collide with the object it gets destroyed
            Destroy(collision.gameObject);
            cherries += 1;
            //Counter for the Cherries
            cherryText.text = cherries.ToString();
        }
    }

    //Enemy Object
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            if(state == State.falling)
            {
                //If you jump on their head you destroy them
                Destroy(other.gameObject);
                Jump();
            }
            else
            {
                state = State.hurt;
                if(other.gameObject.transform.position.x > transform.position.x)
                {
                    //Enemy is to the right, take damage
                    rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
                }
                else
                {
                    //Enemy is to the left, take damage
                    rb.velocity = new Vector2(hurtForce, rb.velocity.y);
                }
            }
        }
    }

    //Movement code
    private void Movement()
    {
        //hDirection is for horizontal
        float hDirection = Input.GetAxis("Horizontal");

        //Moving Left
        if (hDirection < 0)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);
        }

        //Moving Right
        else if (hDirection > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);
        }

        //Jumping
        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            state = State.jumping;
        }
    }

    //Jumping
    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        state = State.jumping;
    }      

    //Animation changes
    private void AnimationState()
    {


        //Jumping animation
        if (state == State.jumping)
        {
            if (rb.velocity.y < .1f)
            {
                state = State.falling;
            }       
        }
        //falling animation
        else if (state == State.falling)
        {
            if(coll.IsTouchingLayers(ground))
            {
                state = State.idle;
            }
        }
        else if (state == State.hurt)
        {
            if(Mathf.Abs(rb.velocity.x) < .1f)
            {
                state = State.idle;
            }
        }
        else if(Mathf.Abs(rb.velocity.x) > 2f)
        {
            //Moving to the right or left with running animation
            state = State.running;
        }
        else
        {
            //standing / idle animation
            state = State.idle;
        }
        
    }
}



