using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Movement : MonoBehaviour
{

    //Movements X
    float velX = 5f;
    bool run;

    //Movements Skid
    int skid;
    int right;
    int left;
    bool sprint;

    //Interactions Shell
    float kick_Shell = 150f;

    //Movement Y
    bool crouch;
    bool watch_Up;
    float high_Jump;
    float jump = 12f;
    float max_Jump = 2.5f;
    float minimun_Jump = 2f;
    float jump_Down;
    bool sprint_jump;
    bool is_Grounded;

    //Components
    public Transform foot;
    float foot_Radio = 0.02f;
    public Animator animator;
    public Rigidbody2D rigid_Body2D;
    public GameObject character;
    public LayerMask platform_Mask;
    public CircleCollider2D circle_Collider2D;

    void Awake()
    {
        animator.GetComponent<Animator>();
        rigid_Body2D.GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical_Down");
        Vector2 dir = new Vector2(inputX, inputY);

    //Code to Movement X
            if (inputX > 0) {
                rigid_Body2D.velocity = (new Vector2(dir.x * velX, rigid_Body2D.velocity.y));
                character.transform.rotation = Quaternion.Euler(0, 0, 0);
                animator.SetFloat("velX", inputX);
            }

            if (inputX < 0) {
                rigid_Body2D.velocity = (new Vector2(dir.x * velX, rigid_Body2D.velocity.y));
                character.transform.rotation = Quaternion.Euler(0, 180, 0);
                animator.SetFloat("velX", Mathf.Abs(inputX));
        }

    //In case nothing is pressed 
    if (inputX != 0 && is_Grounded) {
        animator.SetFloat("velX", 1);
    } else {
        animator.SetFloat("velX", 0);
    }
     

    //Code to Run
    if (inputX > 0 || inputX < 0) {
        if (Input.GetButton("Fire1")) {
            velX = 7.7f;
            run = true;
            StartCoroutine(Sprint());
        } else {
            run = false;
            sprint = false;
            velX = 5f;
            
        }
    }

         //Code to Crouch
        if (inputX == 0 && Input.GetButton("Vertical_Down") && is_Grounded) {
            circle_Collider2D.enabled = false;
            crouch = true;
            animator.SetBool("crouch", true);
        } else {
            circle_Collider2D.enabled = true;
            crouch = false;
            animator.SetBool("crouch", false);
        }

        //Code to animator jump
        if (is_Grounded) {
            animator.SetBool("isGround", true);
        } else {
            animator.SetBool("isGround", false);
        }

      //Code to Jump better
    is_Grounded = Physics2D.OverlapCircle(foot.position, foot_Radio, platform_Mask);
            if (Input.GetButtonDown("Jump") && !crouch && is_Grounded) {
                rigid_Body2D.velocity = Vector2.up * jump;
                animator.SetBool("isGround", false);
            }
            if (rigid_Body2D.velocity.y < 0) {
                rigid_Body2D.velocity += Vector2.up * Physics2D.gravity.y * (max_Jump - 1) * Time.deltaTime;
            } else if (rigid_Body2D.velocity.y > 0 && !Input.GetButton("Jump")) {
                rigid_Body2D.velocity += Vector2.up * Physics2D.gravity.y * (minimun_Jump - 1) * Time.deltaTime;

            }
        //Code Jump down
        jump_Down = rigid_Body2D.velocity.y;
        if (jump_Down != 0 || jump_Down == 0) {
            animator.SetFloat("velY", jump_Down);
        }

        //Code skid
        if (!Input.GetButton("Horizontal")) {
            StartCoroutine(waitTime());
        }

        //Code skid to right to left
        if (inputX > 0.5f) {
            right = 1;
        }
        if (right == 1) {
            skid = 1;
        }
        if (skid == 1 && inputX < 0) {
            animator.SetBool("skid", true);
            StartCoroutine(waitTime());
        }

        //Code skid to left to right
        if (inputX < 0) {
            left = 1;
        }
        if (left == 1) {
            skid = -1;
        }
        if (skid == -1 && inputX > 0) {
            animator.SetBool("skid", true);
            StartCoroutine(waitTime());
        }

        //Code if no to press nothing
        if (inputX == 0) {
            animator.SetBool("turbo", false);
        }

        //Code to turbo
        if (inputX > 0 || inputX < 0) {
            if (sprint) {
                animator.SetBool("turbo", true);
            } else {

                animator.SetBool("turbo", false);
            }
        }
    }
    //Corrutime to Sprint
    public IEnumerator Sprint() {
        yield return new WaitForSeconds(0.5f);
        if (run) {
            velX = 8.5f;
            sprint = true;
            animator.SetBool("turbo", true);
        } else {
            StopCoroutine(Sprint());
            sprint = false;
            velX = 5f;
            animator.SetBool("turbo", false);
        }
    }

    //Corrutime to waitTime
    public IEnumerator waitTime() {
        yield return new WaitForSeconds(0.3f);
        skid = 0;
        right = 0;
        left = 0;
        animator.SetBool("skid", false);
    }
}