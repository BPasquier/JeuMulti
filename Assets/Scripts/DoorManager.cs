using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    private Animator animator;
    public float range;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player != null)
            {
                if (Vector3.Distance(player.transform.position, transform.position)<range)
                {
                    animator.SetBool("character_nearby", true);
                }
                else
                {
                    animator.SetBool("character_nearby", false);
                }
            }
        }
    }
}
