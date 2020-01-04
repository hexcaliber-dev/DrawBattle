using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimController : MonoBehaviour
{
    [SerializeField] private float maxAngularSpeed;
    [SerializeField] private float angularAccerlation;
    private float angularSpeed;
    private float turn;

    // Start is called before the first frame update
    void Start()
    {
        angularSpeed = 0;
    }


    void FixedUpdate()
    {
        Aiming();
    }


    void Aiming()
    {
        // Fix so that the player can't change direction the frame let go
        // and go the same speed
        if ( Input.GetKey(KeyCode.J) && Input.GetKey(KeyCode.K) )
        {
            angularSpeed = 0;
        }
        else if ( Input.GetKey(KeyCode.J) ) 
        {
            angularSpeed += Accelerate(angularSpeed, maxAngularSpeed, angularAccerlation);
            turn = 1f * angularSpeed;
        }
        else if ( Input.GetKey(KeyCode.K) )
        {
            angularSpeed += Accelerate(angularSpeed, maxAngularSpeed, angularAccerlation);
            turn = -1f * angularSpeed;
        }
        else {
            turn = 0;
            angularSpeed = 0;
        }
        //changeVector = new Vector3(0, 0 , turn);
        //Debug.Log(turn);
        transform.Rotate(Vector3.forward * turn);
    }

    float Accelerate( float s, float m, float a)
    {
        if (s < m)
        {
            return a;
        }
        return 0;
    }

}
