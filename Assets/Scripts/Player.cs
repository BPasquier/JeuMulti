using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int HP;
    public bool hunter;
    // Start is called before the first frame update
    void Start()
    {
        HP = 5;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        /*if (!hunter)
        {
            if (collision.transform.tag == "Bullet")
            {
                HP--;
                print("AYE BORDEL DE MERDE !! STOP TK");
            }
        }*/
    }
}
