using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float speed;
    private float translate;
    private Vector3 changeVector;
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
        if ( Input.GetKey(KeyCode.W) ) {translate = 1f * speed;}
        else if ( Input.GetKey(KeyCode.S) ) {translate = -1f * speed;}
        else {translate = 0;}
        changeVector = new Vector2(0, translate);
        transform.position += changeVector;
    }

    void AimMovement()
    {

    }
}
