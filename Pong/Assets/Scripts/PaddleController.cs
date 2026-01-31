using UnityEngine;

public class PaddleController : MonoBehaviour
{
    [SerializeField] protected float speed = 8f;
    protected Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    
    void FixedUpdate()
    {
        float input = GetMovementInput();
        rb.velocity = new Vector2(0f, input * speed);
    }

    protected virtual float GetMovementInput()
    {
        return 0f;
    }
}
