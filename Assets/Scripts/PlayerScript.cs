using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    // All variables/attributes held by the player such as health, score, or 
    // pixel amount goes here
    
    [SerializeField] public int health;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int ChangeHealth(int change)
    {
        health += change;
        return health;
    }

    public int SetHealth(int set)
    {
        health = set;
        return health;
    }
}
