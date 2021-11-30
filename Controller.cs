using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public class Controller : MonoBehaviour {
    // Use this for initialization
    [Header("Movimiento X")]
    [Space]
    public float movX;
    public float velX = 5f;
    public bool run;
    public bool mirando_derecha = true;
    float TopSpeedBound = 6f;

    [Header("Variables derrapar")]
    //Variables de deslizar
    public int derrape;
    public int derecha;
    public int izquierda;
    public bool turbo;

    [Header("Movimiento Y")] 
    [Space]
    public bool agachado;
    public bool mirarArriba;
    public float voltereta = 16f;
    public float salto = 20f;
    public float KickPower = 150f;
    public float saltoMaximo = 2.5f;
    public Transform pie;
    float radioPie = 0.02f;
    bool enSuelo;
    public float saltoMinimo = 2f;
    float caida;
    bool turboJump;

    //Variables turbo salto
    bool turboSalto;

    [Header("Componentes Player")]
    [Space]
    public Animator anim;
    public Rigidbody2D rb;
    public SpriteRenderer sp;
    public GameObject Player, item_held;
    public LayerMask PlatformMask;
    public CircleCollider2D c_collider2D;

    void Awake () 
    {
        anim.GetComponent<Animator>();
        TopSpeedBound = velX;
    }
    void Start () 
    {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    //Función para agarrar la caracola
    public void pick_item_up (GameObject item) {
        if (item_held == null) {
            item_held = item;
            var hand_joint = this.transform.Find("hand_joint");

            item_held.transform.parent = this.transform;
            var position = hand_joint.transform.localPosition;
            item_held.transform.localPosition = new Vector2(position.x + 6 * hand_joint.localScale.x, 
                                                            position.y - 7);

            var shell_rb = item.GetComponent<Rigidbody2D>();
           
        }
    }
    public void pick_item_down () {
         if (item_held != null)
        {
            item_held.transform.parent = null;
            item_held.gameObject.AddComponent<Rigidbody2D>();

            var item_rigid_body = item_held.GetComponent<Rigidbody2D>();
            item_rigid_body.freezeRotation = true;

            kick_item(item_held);

            item_held = null;
        }
    }
    public void kick_item(GameObject item)
    {
        var item_rigid_body = item.GetComponent<Rigidbody2D>();
        var rigid_body = GetComponent<Rigidbody2D>();
        var local_scale = rigid_body.transform.localScale;

        item_rigid_body.AddForce(new Vector2(KickPower * local_scale.x, 0));

    }

    void FixedUpdate()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical_Down");
        Vector2 dir = new Vector2(inputX, inputY);
    

        if (!agachado) { 
            if (inputX > 0) {
                rb.velocity = (new Vector2(dir.x * velX, rb.velocity.y));
                Player.transform.rotation = Quaternion.Euler(0, 0, 0);
                anim.SetFloat("velX", inputX);
            }
            if (inputX < 0) {
                rb.velocity = (new Vector2(dir.x * velX, rb.velocity.y));
                Player.transform.rotation = Quaternion.Euler(0, 180, 0);
                anim.SetFloat("velX", Mathf.Abs (inputX));
            }
        }

        if (inputX != 0 && enSuelo) {
            anim.SetFloat("velX", 1);
        } else
        {
            anim.SetFloat("velX", 0);
        }
        //Turbo
        if (inputX > 0 || inputX < 0) {
            if (turbo) {
                anim.SetBool("turbo", true);
                anim.SetFloat("velX", 0);
            } else
            {
                anim.SetBool("turbo", false);
            }
        }

        //Correr
        if (inputX > 0 || inputX < 0) {
            if (Input.GetButton("Fire1")) {
                velX = 7.5f;
                run = true;
                StartCoroutine(Turbo());
            } else {
                run = false;
                turbo = false;
                velX = 5f;
         
            }
        }

        //Quieto
        if (inputX == 0) {
            anim.SetBool("turbo", false);
            anim.SetBool("turboJump", false);
            turbo = false;
            
        }
    

        enSuelo = Physics2D.OverlapCircle(pie.position, radioPie, PlatformMask);
        if (enSuelo) {
            anim.SetBool("enSuelo", true);
             if (Input.GetButtonDown("Jump") && !agachado) {
         rb.velocity = Vector2.up * salto;
         
        }
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (saltoMaximo - 1) * Time.deltaTime;
        } else if (rb.velocity.y > 0 && !Input.GetButton("Jump")) {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (saltoMinimo - 1) * Time.deltaTime;
        }
        } else {
            anim.SetBool("enSuelo", false);
        }


        //Agacharse
        if (inputX == 0 && Input.GetButton("Vertical_Down") && enSuelo) {
            anim.SetBool("crouch", true);
            agachado = true;
            c_collider2D.enabled = false;
        } else {
            anim.SetBool("crouch", false);
            agachado = false;
            c_collider2D.enabled = true;
        }
        //Animacion de caida
        caida = rb.velocity.y;

        if (caida != 0 || caida == 0) {
            anim.SetFloat("velY", caida);
        }

       //Derrapar
       if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow)) {
           StartCoroutine(TimeForWait());
       }

       //Derrapar derecha a izquierda
       if (inputX > 0.5f) {
           derecha = 1;
       }
       if (derecha == 1) {
           derrape = 1;
       }

       if (derrape == 1 && Input.GetKey(KeyCode.LeftArrow)) {
           anim.SetBool("skid", true);
           StartCoroutine(TimeForWait());
       }

       //Derrapar izquierda a derecha
       if (inputX < 0) {
           izquierda = 1;
       }
       if (izquierda == 1) {
           derrape = -1;
       }
       if (derrape == -1 && Input.GetKey(KeyCode.RightArrow)) {
           anim.SetBool("skid", true);
           StartCoroutine(TimeForWait());
       }
       //Turbo jump
       
       if (inputX < 0 || inputX > 0) {
            if (turbo && Input.GetButton("Fire1")) {
                anim.SetBool("turboJump", true);
                turboJump = true;
        } else {
            anim.SetBool("turboJump", false);
        }
    }

    }
    public IEnumerator TimeForWait()
    {
        yield return new WaitForSeconds(0.2f);
        derrape = 0;
        derecha = 0;
        izquierda = 0;
        anim.SetBool("skid", false);
    }
    public IEnumerator Turbo() {
        yield return new WaitForSeconds(0.5f);
        if (run) {
            velX = 8.5f;
            turbo = true;
            anim.SetBool("turbo", true);
        } else {
            StopCoroutine(Turbo());
            turbo = false;
            velX = 5;
            anim.SetBool("turbo", false);
        }
    }
}