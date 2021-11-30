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
    float jump = 25f;
    float max_Jump = 2.5f;
    float minimun_Jump = 2f;
    float jump_Down;
    bool sprint_jump;
    bool is_Grounded;

    //Components
    Transform feet;
    float feet_Radio = 0.02f;
    Animator animator;
    Rigidbody2D rigid_Body2D;
    public GameObject character;
    public LayerMask platform_Mask;
    public BoxCollider2D box_Collider2D;


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
