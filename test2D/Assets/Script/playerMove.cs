using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMove : MonoBehaviour
{
    public float speed;
    public float height;
    public Transform CheckPoint;
    public float CheckRadius;
    private bool isGrounded;
    public LayerMask WhatIsGround;
    private Transform playerTrans;
    private Rigidbody2D playerRig;
    private Animator playerAnim;
    // Start is called before the first frame update
    void Start()
    {
        playerTrans = this.GetComponent<Transform>();
        playerRig = this.GetComponent<Rigidbody2D>();
        playerAnim = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if(h>0)
        {
            playerTrans.localScale = new Vector3(1,1,1);
        }
        if(h<0)
        {
            playerTrans.localScale = new Vector3(-1,1,1);
        }
        playerTrans.Translate(new Vector2(h,0) * speed * Time.deltaTime);
        if(Input.GetButtonDown("Jump"))
        {
            playerRig.AddForce(new Vector2(0,height));
        }
        isGrounded = Physics2D.OverlapCircle(CheckPoint.position,CheckRadius,WhatIsGround);
        playerAnim.SetBool("Grounded",isGrounded);
        playerAnim.SetFloat("RunSpeed",Mathf.Abs(h*speed));
    }
}
