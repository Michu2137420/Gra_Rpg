using UnityEngine;

public class ResourcesController : MonoBehaviour
{
    public GameObject resourcePrefab;
    public int maxDurability = 100;
    public int currentDurability;

    private float holdTimer = 0f;
    public float holdInterval = 0.5f; // czas w sekundach miêdzy wywo³aniami


    void Start()
    {
        currentDurability = maxDurability;
    }
    private void FunctionToManageResourceDurability(int amount)
    {
        currentDurability = Mathf.Clamp(currentDurability + amount, 0, maxDurability);
        Debug.Log("Current Health: " + currentDurability);
        if (currentDurability <= 0)
        {
            spawnResource();
            Destroy(gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                holdTimer += Time.deltaTime;
                if (holdTimer >= holdInterval)
                {
                    FunctionToManageResourceDurability(-20);
                    holdTimer = 0f;
                }
            }
            else
            {
                holdTimer = 0f;
            }
        }
        else
        {
            holdTimer = 0f;
        }
    }
        
    public void spawnResource()
    {
        float spawnPositionX = transform.position.x + Random.Range(-1f, 1f);
        float spawnPositionY = transform.position.y + Random.Range(-1f, 1f);
        Vector3 spawnPosition = new Vector3(spawnPositionX, spawnPositionY, transform.position.z);
        Instantiate(resourcePrefab, spawnPosition, Quaternion.identity);
    }
}
