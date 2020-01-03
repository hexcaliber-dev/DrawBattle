using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScript : MonoBehaviour
{

    PlayerScript player;    

    // Start is called before the first frame update
    void Start()
    {
        player = gameObject.GetComponentInParent(typeof(PlayerScript)) as PlayerScript;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {        
        player.ChangeHealth(-1);
        Debug.Log("Health: " + player.health);
        Destroy(col.gameObject);
    }


}
