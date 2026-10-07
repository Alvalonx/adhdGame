using UnityEngine;

public class UnitControler : MonoBehaviour
{
    [SerializeField]
    private GameObject[] unitPrefabs;
    private GameObject unit;
    private GameObject spawnedUnit;
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
        float targetX = 0f;
        float targetY = 0f;
        int randomizeDirection = Random.Range(0, 4);
        int randomizeUnit = Random.Range(0, unitPrefabs.Length);
        unit = unitPrefabs[randomizeUnit];
        switch (randomizeDirection)
        {
            case 0:
                randomX = Random.Range(0f, 1f);
                randomY = 1f + offset;
                targetX = randomX;
                targetY = 0f - offset;
                break;
            case 1:
                randomX = Random.Range(0f, 1f);
                randomY = 0f - offset;
                targetX = randomX;
                targetY = 1f + offset;
                break;
            case 2:
                randomX = 0f - offset;
                randomY = Random.Range(0f, 1f);
                targetX = 1f + offset;
                targetY = randomY;
                break;
            case 3:
                randomX = 1f + offset;
                randomY = Random.Range(0f, 1f);
                targetX = 0f - offset;
                targetY = randomY;
                break;
        }
        Vector3 spawnPosition = Camera.main.ViewportToWorldPoint(new Vector3(randomX, randomY, 10f));
        spawnedUnit = Instantiate(unit, spawnPosition, Quaternion.identity);
        if (spawnedUnit.TryGetComponent(out UnitMove unitMover))
        {
            unitMover.Move(Camera.main.ViewportToWorldPoint(new Vector3(targetX, targetY, 10f)));
        }
    }
}
