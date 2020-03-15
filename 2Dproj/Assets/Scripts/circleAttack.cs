using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class circleAttack : MonoBehaviour
{

    void attack1()
    {
        transform.GetChild(0).gameObject.GetComponent<PolygonCollider2D>().enabled = true;
        transform.GetChild(1).gameObject.GetComponent<PolygonCollider2D>().enabled = false;
    }

    void attack2()
    {
        transform.GetChild(0).gameObject.GetComponent<PolygonCollider2D>().enabled = false;
        transform.GetChild(1).gameObject.GetComponent<PolygonCollider2D>().enabled = true;
    }

    void attackDisable()
    {
        transform.GetChild(0).gameObject.GetComponent<PolygonCollider2D>().enabled = false;
        transform.GetChild(1).gameObject.GetComponent<PolygonCollider2D>().enabled = false;
    }

    void end()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        Destroy(gameObject);
    }
}
