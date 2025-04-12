using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float jumpSpeed = 20f;
    [SerializeField] float climbSpeed = 10f;


    Vector2 moveInput;
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    CapsuleCollider2D myCollider;

    BoxCollider2D myFeetCollider;
    LayerMask myLayer;
    float gravityAtStart;
    bool isALive = true;
    
    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityAtStart = myRigidBody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if(isALive) {
            Run();
            FlipSprite();
            ClimbLadder();
            Die();
        }
    }

    void OnMove(InputValue value) {
        if(isALive) {
            moveInput = value.Get<Vector2>();
            Debug.Log(moveInput);
        }
    }

    void OnJump(InputValue value) {
        if(isALive) {
            if(!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
                return;
            if (value.isPressed){
                myRigidBody.velocity += new Vector2(0f, jumpSpeed);
            }
        }
    }

    void Run(){
        Vector2 playerVelocity = new Vector2(moveInput.x * speed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning",playerHasHorizontalSpeed);
    }

    void FlipSprite(){
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        if(playerHasHorizontalSpeed){
            transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1f);
        }
    }

    void ClimbLadder(){
        if(!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))){
            myRigidBody.gravityScale = gravityAtStart;
            myAnimator.SetBool("isClimbing",false);
            return;
        }

        Vector2 climbVelocity = new Vector2(myRigidBody.velocity.x, moveInput.y * climbSpeed);
        myRigidBody.velocity = climbVelocity;
        myRigidBody.gravityScale = 0f;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isClimbing",playerHasVerticalSpeed);
        
    }
    void Die(){
        if(myCollider.IsTouchingLayers(LayerMask.GetMask("Enemy"))){
            isALive=false;
        }
    }
}
