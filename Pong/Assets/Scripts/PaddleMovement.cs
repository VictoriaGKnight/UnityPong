using UnityEngine;

public class PaddleMovement : MonoBehaviour
{
    public float speed = 5.0f;

    // Update is called once per frame
    void Update()
    {
        float verticalInput = Input.GetAxis("Vertical");
        transform.position += new Vector3(0, verticalInput * speed * Time.deltaTime, 0);
    }
}