using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playermovement : MonoBehaviour
{
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
     rb = GetComponent<Rigidbody2D>();
     rb.bodyType = RigidbodyType2D.Kinematic;   
    }

    // Update is called once per frame
    void Update()
    {
       if (Input.GetKeyDown(KeyCode.W))
       {
        rb.velocity = new Vector2(5, 0);
       }
       else if (Input.GetKeyDown(KeyCode.S))
       {
        rb.velocity = new Vector2(-5, 0);
       }

       
    }
}
