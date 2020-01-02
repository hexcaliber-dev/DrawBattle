using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private int translate;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        VerticalMovement();
        AimMovement();
    }

    void VerticalMovement()
    {
        if ( Input.GetKey(KeyCode.W) ) {translate = 1;}
        if ( Input.GetKey(KeyCode.S) ) {translate = -1;}
    }

    void AimMovement()
    {

    }
}
