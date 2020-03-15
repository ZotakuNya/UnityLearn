using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyCtrl : MonoBehaviour
{
    private Rigidbody2D rb;
    public Transform left, right;
    private bool faceLeft = true;
    // Start is called before the first frame update
    public Vector2 jumpForce;
    private float timer = 4f;
    private float lx, rx;
    void Start()
    {
        lx = left.position.x;
        rx = right.position.x;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < lx)
        {
            faceLeft = false;
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (transform.position.x > rx)
        {
            faceLeft = true;
            transform.localScale = new Vector3(1, 1, 1);
        }
        if (timer <= 0)
        {
            move();
            timer = 4f;
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }
    void move()
    {
        if (faceLeft)
        {
            rb.velocity = new Vector2(-jumpForce.x, jumpForce.y);
        }
        else
        {
            rb.velocity = jumpForce;
        }
    }
}
