using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float maxSpeed;
    [SerializeField] private float maxAngularSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float angularAccerlation;
    private float speed;
    private float angularSpeed;
    private float translate;
    private float turn;
    private float direction;
    private Vector3 changeVector;
    // Start is called before the first frame update
    void Start()
    {
        speed = 0;
    }


    void FixedUpdate()
    {
        Turning();
        Movement();
    }

    void Movement()
    {
        // Fix so that the player can't change direction the frame let go
        // and go the same speed
        if ( Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S) )
        {
            speed = 0;
        }
        else if ( Input.GetKey(KeyCode.W) ) 
        {
            speed += Accelerate(speed, maxSpeed);
            translate = 1f * speed;
        }
        else if ( Input.GetKey(KeyCode.S) )
        {
            speed += Accelerate(speed, maxSpeed);
            translate = -1f * speed;
        }
        else {
            translate = 0;
            speed = 0;
        }
        changeVector = new Vector2(0, translate);
        transform.position += changeVector;
    }
   
    void Turning()
    {
        // Fix so that the player can't change direction the frame let go
        // and go the same speed
        if ( Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D) )
        {
            angularSpeed = 0;
        }
        else if ( Input.GetKey(KeyCode.A) ) 
        {
            angularSpeed += Accelerate(angularSpeed, maxAngularSpeed);
            turn = 1f * angularSpeed;
        }
        else if ( Input.GetKey(KeyCode.D) )
        {
            angularSpeed += Accelerate(angularSpeed, maxAngularSpeed);
            turn = -1f * angularSpeed;
        }
        else {
            turn = 0;
            angularSpeed = 0;
        }
        //changeVector = new Vector3(0, 0 , turn);
        Debug.Log(turn);

        transform.Rotate(Vector3.forward * turn);
    }

    float Accelerate( float s, float max)
    {
        if (s < maxSpeed)
        {
            return acceleration;
        }
        return 0;
    }



}
