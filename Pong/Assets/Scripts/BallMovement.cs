using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(3f, 3f);
    }

    void OlisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        
        rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
        rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
    }
}
