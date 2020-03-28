using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class playerCtroller : MonoBehaviour
{
    private Rigidbody2D plRb;
    private Transform plTf;
    private Animator plAm;
    public float moveSpeed;
    public float jumpHeight;
    public GameObject Checkpoint1;
    public LayerMask GroundLayer;
    public bool grounded;
    private bool jumpPress;
    private bool burstPress;
    public float burstSpeed;
    private Vector2 mpos;
    public float burstTime;
    private float timer = 0;
    //public bool dragging;
    public LayerMask enemyLayer;
    public float hurtForce;
    private bool hurting;
    public bool silence;
    private float hurtTime = 0.5f;
    private float burstCD = 0;

    public float barUpLength;
    public Slider healthSlider;


    // Start is called before the first frame update
    void Start()
    {
        plRb = GetComponent<Rigidbody2D>();
        plTf = GetComponent<Transform>();
        plAm = GetComponent<Animator>();
        healthSlider.value = 1;
    
    }

    // Update is called once per frame
    private void Update()
    {
        if (healthSlider.value == 0)
            plAm.SetBool("death", true);

        if (Input.GetButtonDown("Jump") && grounded /*(grounded||dragging)*/)
        {
            jumpPress = true;
        }
        if(Input.GetMouseButtonDown(1))
        {
            mpos = Input.mousePosition;
            mpos = Camera.main.ScreenToWorldPoint(mpos);
            timer = 0;
        }
        if (Input.GetMouseButton(1) && (burstCD <= 0))
        {
            burstPress = true;
        }

        Vector3 worldPos = new Vector3(transform.position.x, transform.position.y + barUpLength, transform.position.z);

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        healthSlider.transform.position = new Vector3(screenPos.x, screenPos.y, screenPos.z);

    }

    void FixedUpdate()
    {
        if (!burstPress)
            burstCD -= Time.fixedDeltaTime;
        stat_check();
        animCtrl();
        if (!hurting && !silence)
        {
            plAm.SetBool("hurting", false);
            move();
        }
        else if (hurting)
            hurt();
        else if (silence)
        {
            plAm.SetBool("hurting", true);
        }
        if (grounded)
            burstCD = 0;
        burst();
    }
    void move()
    {
        float hMove = Input.GetAxis("Horizontal") * moveSpeed * Time.fixedDeltaTime;
        //float v = Input.GetAxis("Vertical");
        //if(grounded)
        //{
        //    plRb.velocity = new Vector2(hMove, plRb.velocity.y);
        //}
        float currentSpeed = plRb.velocity.x;
        if (currentSpeed > 0)
        {
            plTf.localScale = new Vector3(1, 1, 1);
        }
        else if (currentSpeed < 0)
        {
            plTf.localScale = new Vector3(-1, 1, 1);
        }

        if (hMove != 0) 
            plRb.velocity = new Vector2(hMove, plRb.velocity.y);
        if (jumpPress)
        {
            plRb.velocity = new Vector2(plRb.velocity.x * 1.5f, jumpHeight);
            jumpPress = false;
            //dragging = false;
        }
        //if(!grounded)
        //{
        //    plrb.velocity = new vector2(hmove, plrb.velocity.y);
        //}
        plAm.SetFloat("currentSpeed", Mathf.Abs(currentSpeed));
    }

    void stat_check()
    {
        //grounded = Physics2D.OverlapCircle(Checkpoint1.transform.position, 0.1f, GroundLayer);
        grounded = Physics2D.OverlapBox(Checkpoint1.transform.position, new Vector2(0.73f, 0.12f), 0, GroundLayer);
        if(Physics2D.OverlapBox(Checkpoint1.transform.position, new Vector2(0.73f, 0.12f), 0, enemyLayer))
        {
            GameObject enemy = GameObject.FindGameObjectWithTag("enemy");
            for (int i = 0; i < enemy.transform.childCount; i++)
            {
                Destroy(enemy.transform.GetChild(i).gameObject);
            }
            Destroy(enemy);
            plRb.velocity = new Vector2(plRb.velocity.x * 1.5f, jumpHeight);
        }
        //Debug.Log(Checkpoint1.transform.position);
    }

    void animCtrl()
    {
        if (!grounded)
        {
            plAm.SetBool("grounded", false);
            if (plRb.velocity.y > 0)
            {
                plAm.SetBool("falling", false);
                plAm.SetBool("jumping", true);
            }
            if (plRb.velocity.y < 0)
            {
                plAm.SetBool("jumping", false);
                plAm.SetBool("falling", true);
            }
        }
        else
        {
            plAm.SetBool("grounded", true);
            plAm.SetBool("falling", false);
            plAm.SetBool("jumping", false);
        }

    }

    void burst()
    {
        if (burstPress && timer < burstTime)
        {
            //Vector2 tmp = new Vector2(mpos.x - plTf.position.x, mpos.y - plTf.position.y).normalized * burstSpeed * Time.fixedDeltaTime;
            //plRb.velocity = plRb.velocity + new Vector2(tmp.x, tmp.y);
            Vector2 tmp = new Vector2(mpos.x - plTf.position.x, mpos.y - plTf.position.y).normalized;
            if(grounded)
                plRb.AddForce(new Vector2(tmp.x * 2, tmp.y) * burstSpeed);
            else
                plRb.AddForce(new Vector2(tmp.x , tmp.y) * 2 * burstSpeed);

            timer += Time.fixedDeltaTime;
        }
        else if (burstPress && timer > burstTime)
        {
            burstCD = 0.5f;
            burstPress = false;
        }
    }

    void hurt()
    {
        if(hurtTime == 0.5f)
        {
            Vector2 tmp_pos = GameObject.FindGameObjectWithTag("boss").transform.position;
            if (grounded)
            {
                if (transform.position.x < tmp_pos.x)
                    tmp_pos = new Vector2(-1, 0);
                else
                    tmp_pos = new Vector2(1, 0);
            }
            else
            {
                tmp_pos = ((Vector2)transform.position - tmp_pos).normalized;
            }
            //plRb.velocity = new Vector2(tmp_pos.x, tmp_pos.y * 2) * hurtForce;
            plRb.AddForce(new Vector2(tmp_pos.x, tmp_pos.y * 1.5f) * hurtForce);
            plAm.SetBool("hurting", true);

        }
        if (timerFun())
        {
            hurting = false;
            plAm.SetBool("hurting", false);
        }
    }

    bool timerFun()
    {
        hurtTime -= Time.fixedDeltaTime;
        if(hurtTime<0)
        {
            hurtTime = 0.5f;
            return true;
        }
        return false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "door")
        {
            if(Input.GetButtonDown("Confirm") && grounded)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Room");
            }
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "hurt")
        {
            hurting = true;
            healthSlider.value -= 0.05f;
        }
        if(collision.tag == "crash")
        {
            Debug.Log("crash");
            Vector2 tmp_pos = GameObject.FindGameObjectWithTag("boss").transform.position;
            if (transform.position.x < tmp_pos.x)
                plRb.AddForce(new Vector2(-300, 0));
            else
                plRb.AddForce(new Vector2(300, 0));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "boss")
        {
            hurting = true;
            healthSlider.value -= 0.05f;
        }
    }

    void plend()
    {
        Destroy(healthSlider.gameObject);
        Invoke("restart", 0.5f);
    
    }

    void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}