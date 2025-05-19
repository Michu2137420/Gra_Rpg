using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingItem : MonoBehaviour
{
    public int amount = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Object colided with " + collision);

        PlayerController player = collision.GetComponent<PlayerController>();

        if (player != null && player.currentHealth < player.maxHealth)
        {
            player.FunctionToManageHealth(amount);
            Destroy(gameObject);
        }
    }
}
