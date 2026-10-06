using UnityEngine;

public class UnitControler : MonoBehaviour
{
    [SerializeField]
    private GameObject[] unitPrefabs;
    private GameObject unit;
    private float offset = 0.1f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ObjectSpawn();
        }
    }

    private void ObjectSpawn()
    {
        float randomX = 0f;
        float randomY = 0f;
        int randomizeDirection = Random.Range(0, 4);
        int randomizeUnit = Random.Range(0, unitPrefabs.Length);
        unit = unitPrefabs[randomizeUnit];
        switch (randomizeDirection)
        {
            case 0:
                randomX = Random.Range(0f, 1f);
                randomY = 1f + offset;
                break;
            case 1:
                randomX = Random.Range(0f, 1f);
                randomY = 0f - offset;
                break;
            case 2:
                randomX = 0f - offset;
                randomY = Random.Range(0f, 1f);
                break;
            case 3:
                randomX = 1f + offset;
                randomY = Random.Range(0f, 1f);
                break;
        }
        Vector3 spawnPosition = Camera.main.ViewportToWorldPoint(new Vector3(randomX, randomY, 0f));
        Instantiate(unit, spawnPosition, Quaternion.identity);

    }
}
