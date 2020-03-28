using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bossCtrl : MonoBehaviour
{
    public GameObject circleAttack;
    private float timer1time = 1;
    private Rigidbody2D rb;
    public Transform pl;
    public float dashSpeed;
    private bool dashing = false;
    private Vector2 aim;
    private Vector2 startpoint;
    public float distance;
    private float timer2time = 1f;
    private float timer3time = 2f;
    private bool[] crashing = new bool[5];
    private int bulletNum = 3;
    public GameObject bullet;
    public float bulletSpeed;
    private bool shooting;
    private bool[] actionPool = new bool[10];
    public float bossSpeed;
    public bool drag;
    private Shot shot;
    public float dragForce;
    private float timer4time = 1;
    public BoxCollider2D buttomCheck;
    private Animator anim;

    public float barUpLength;
    public Slider healthSlider;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        shot = GameObject.FindGameObjectWithTag("gem").GetComponent<Shot>();
        anim = GetComponent<Animator>();
        healthSlider.value = 1f;
    }

    private void Update()
    {
        Vector3 worldPos = new Vector3(transform.position.x  , transform.position.y + barUpLength, transform.position.z);

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        healthSlider.transform.position = new Vector3(screenPos.x, screenPos.y, screenPos.z);

        if(healthSlider.value == 0)
        {
            anim.SetBool("death", true);
        }
    }

    void FixedUpdate()
    {
        if (healthSlider.value != 0)
        {
            moveCrtl();
            if (actionPool[0])
            {
                circle();
            }
            if (actionPool[1])
            {
                if (!dashing)
                {
                    aim = pl.position;
                    startpoint = transform.position;
                    dashing = true;
                }
            }
            if (dashing)
            {
                if (aim.x < startpoint.x)
                {
                    transform.localScale = new Vector3(2, 2, 1);
                }
                if (aim.x > startpoint.x)
                {
                    transform.localScale = new Vector3(-2, 2, 1);
                }
                dash();
            }
            else
            {
                rb.gravityScale = 1;
            }
            if (actionPool[2])
            {
                crashing[0] = true;
            }
            if (crashing[0])
            {
                crashing[0] = false;
                rb.velocity = new Vector2(0, 10);
                crashing[1] = true;
            }
            if (crashing[1])
            {
                if (rb.velocity.y <= 0.1f)
                {
                    crashing[2] = true;
                    crashing[1] = false;
                }
            }
            if (crashing[2])
            {
                crash();
            }
            if (actionPool[3])
                shooting = true;
            if (shooting)
            {
                if (timer2())
                    shoot();
            }
            if (actionPool[4])
            {
                if (Random.Range(0, 2) == 0)
                {
                    rb.velocity = new Vector2(2, 5);
                }
                else
                {
                    rb.velocity = new Vector2(-2, 5);
                }
            }

            if (rb.velocity.x > 0)
            {
                transform.localScale = new Vector3(-2, 2, 1);
            }
            else if (rb.velocity.x < 0)
            {
                transform.localScale = new Vector3(2, 2, 1);
            }
            attacked();
            for (int i = 0; i < 5; i++)
                actionPool[i] = false;
        }
    }

    void moveCrtl()
    {
        if (!actionPool[5])
        {
            if (transform.position.x < pl.position.x - 5)
            {
                rb.velocity = new Vector2(bossSpeed * Time.fixedDeltaTime, rb.velocity.y);
            }
            else if (transform.position.x > pl.position.x + 5)
            {
                rb.velocity = new Vector2(-bossSpeed * Time.fixedDeltaTime, rb.velocity.y);
            }
        }
        if(timer3())
        {
            anim.speed = 0;
            Invoke("randAct", 0.7f);
        }
        if (timer1())
            actionPool[5] = false;
    }
    void randAct()
    {
        anim.speed = 1;
        int i = Random.Range(0, 5);
        actionPool[i] = true;
    }
    void dash()
    {
        rb.gravityScale = 0;
        transform.position = Vector2.MoveTowards(transform.position, aim, dashSpeed);
        if(Vector2.Distance(startpoint,transform.position)>distance)
        {
            dashing = false;
        }
    }

    void circle()
    {
        GameObject go = Instantiate(circleAttack, transform.position, Quaternion.identity);
        go.transform.parent = transform;
        go.transform.localScale = new Vector3(-0.13f, 0.105f, 0);
        go.transform.localScale = new Vector3(1, 1, 0);
    }

    void shoot()
    {
        if (bulletNum > 0)
        {
            GameObject go = Instantiate(bullet, transform.position, Quaternion.identity);
            go.GetComponent<Rigidbody2D>().velocity = (pl.position - transform.position).normalized * bulletSpeed;
            bulletNum--;
        }
        else
        {
            shooting = false;
            bulletNum = 3;
        }
    }

    void crash()
    {
        rb.gravityScale = 8;
        crashing[3] = true;
        buttomCheck.enabled = true;
        if(crashing[4])
        {
            pl.gameObject.GetComponent<playerCtroller>().silence = true;
            //shot.enabled = false;
            if(timer2())
            {
                //shot.enabled = true;
                pl.gameObject.GetComponent<playerCtroller>().silence = false;
                for (int i = 0; i < 5; i++)
                {
                    crashing[i] = false;
                }
                rb.gravityScale = 1;
                buttomCheck.enabled = false;
            }
        }
    }
    void attacked()
    {
        if(shot.attacking)
        {
            if(drag)
            {
                shot.attacking = false;
                if (pl.gameObject.GetComponent<playerCtroller>().grounded)
                {
                    if (transform.position.x < pl.position.x)
                    {
                        rb.AddForce(new Vector2(dragForce, 0));
                    }
                    else
                    {
                        rb.AddForce(new Vector2(-dragForce, 0));
                    }
                }
                drag = false;
                actionPool[5] = true;
                timer1time = 1;
            }
        }
    }

    bool timer1()
    {
        timer1time -= Time.fixedDeltaTime;
        if (timer1time <= 0)
        {
            timer1time = 1f;
            return true;
        }
        return false;
    }

    bool timer2()
    {
        timer2time -= Time.fixedDeltaTime;
        if (timer2time <= 0)
        {
            timer2time = 1f;
            return true;
        }
        return false;
    }
    bool timer3()
    {
        timer3time -= Time.fixedDeltaTime;
        if (timer3time <= 0)
        {
            timer3time = Random.Range(3, 7);
            return true;
        }
        return false;
    }

    bool timer4()
    {
        timer4time -= Time.fixedDeltaTime;
        if (timer4time <= 0)
        {
            timer4time = 1f;
            return true;
        }
        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "player")
        {
            dashing = false;
            rb.gravityScale = 1;
        }
        if(collision.collider.tag == "leftWall")
        {
            //if (Mathf.Abs(rb.velocity.x) > 2)
            //{
            //    healthSlider.value -= 0.1f;
            //}
            healthSlider.value -= 0.1f;
            rb.AddForce(new Vector2(10000, 10000));
        }
        if (collision.collider.tag == "rightWall")
        {
            //if (Mathf.Abs(rb.velocity.x) > 2)
            //{
            //    healthSlider.value -= 0.1f;
            //}
            healthSlider.value -= 0.1f;
            rb.AddForce(new Vector2(-10000, 10000));
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if ((collision.collider.tag == "Ground" || collision.collider.tag == "player") && crashing[3])
        {
            crashing[3] = false;
            crashing[4] = true;
        }
    }

    void end()
    {
        Destroy(gameObject);
        Destroy(healthSlider.gameObject);
    }
}
