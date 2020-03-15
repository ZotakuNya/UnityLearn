using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{
    private bool isOut = false;
    private Vector2 mpos;
    public Transform GemTs;
    public Transform origin;
    private bool aiming = true;
    public Rigidbody2D GemRb;
    public float shotSpeed;
    private bool catched;
    private Vector3 originPosition;
    private bool moving;
    private Vector3 stayPoint;
    //private DistanceJoint2D joint;
    private float totalDistance;
    private Vector2 catchPoint;
    GameObject player;
    GameObject lastNode;
    public GameObject prefab;
    private Vector2 aim;
    public bool attacking;
    private playerCtroller pl;

    void Start()
    {
        GemRb.isKinematic = true;
        //joint = GameObject.Find("playerIdle").GetComponent<DistanceJoint2D>();
        player = GameObject.FindGameObjectWithTag("player");
        pl = player.GetComponent<playerCtroller>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && aiming)
        {
            mpos = Input.mousePosition;
            mpos = Camera.main.ScreenToWorldPoint(mpos);
            isOut = true;
            aiming = false;
            moving = true;
            aim = (mpos - (Vector2)GemTs.position).normalized * shotSpeed * Time.fixedDeltaTime;
        }
        if (Input.GetMouseButton(0) && moving && !catched && Vector2.Distance(GemTs.position, origin.position) <= 7)
        {

        }
        else if (Input.GetMouseButton(0) && moving && catched && !pl.silence)
        {
            moving = true;
        }
        else
        {
            moving = false;
        }
    }
    private void FixedUpdate()
    {
        if (catched && moving)
            stay();
        else if (attacking && moving)
            attack();
        else
            shot();
    }

    void shot()
    {
        //joint.enabled = false;
        transform.gameObject.GetComponent<HingeJoint2D>().enabled = false;
        if (aiming)
        {
            GemTs.position = origin.position;
            mpos = Input.mousePosition;
            mpos = Camera.main.ScreenToWorldPoint(mpos);
            GemTs.up = new Vector3(mpos.x, mpos.y, 0) - GemTs.position;
        }
        if (isOut && moving)
        {
            if (!catched)
            {
                GemRb.velocity = aim;
            }
            //GemTs.position = Vector2.MoveTowards(GemTs.position, mpos, shotSpeed * Time.fixedDeltaTime);
        }
        else if (!moving)
        {
            GemTs.position = Vector2.MoveTowards(GemTs.position, origin.position, 50 * Time.fixedDeltaTime);
            if (GemTs.position == origin.position)
            {
                aiming = true;
                isOut = false;
            }
        }
    }

    void stay()
    {
        //if (joint.enabled == false)
        //{
        //    joint.enabled = true;
        //    joint.connectedBody = GemRb;
        //    joint.connectedAnchor = origin.position;
        //    joint.distance = Vector2.Distance(GemTs.position, origin.position);
        //}
        if (totalDistance > 1)
        {
            Vector2 tmp = lastNode.transform.position - (lastNode.transform.position - player.transform.position).normalized;
            GameObject go = (GameObject)Instantiate(prefab, tmp, Quaternion.identity);
            go.transform.SetParent(transform);
            lastNode.GetComponent<HingeJoint2D>().connectedBody = go.GetComponent<Rigidbody2D>();
            lastNode = go;
            totalDistance = Vector2.Distance(lastNode.transform.position, player.transform.position);
        }
        else
        {
            lastNode.GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
        }
    }

    void attack()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "catchable" && !aiming)
        {
            catched = true;
            GemRb.velocity = new Vector2(0, 0);
            catchPoint = GemTs.transform.position;
            totalDistance = Vector2.Distance(player.transform.position, catchPoint);
            lastNode = transform.gameObject;
            lastNode.GetComponent<HingeJoint2D>().enabled = true;
        }
        if (collision.tag == "boss" && !aiming)
        {
            GemRb.velocity = new Vector2(0, 0);
            attacking = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "catchable")
        {
            //player.GetComponent<playerCtroller>().dragging = true;
            if (moving == false)
            {
                transform.gameObject.GetComponent<HingeJoint2D>().enabled = false;
                GemTs.position = Vector2.MoveTowards(GemTs.position, origin.position, 30 * Time.fixedDeltaTime);
            }
        }
        if (collision.tag == "boss" && !aiming)
        {
            if (moving == false)
            {
                GemTs.position = Vector2.MoveTowards(GemTs.position, origin.position, 30 * Time.fixedDeltaTime);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "catchable")
        {
            catched = false;
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            //player.GetComponent<playerCtroller>().dragging = true;
        }
        if (collision.tag == "boss" && !aiming)
        {
            //attacking = false;
            GameObject.FindGameObjectWithTag("boss").GetComponent<bossCtrl>().drag = true;
            moving = false;
        }
    }
}
