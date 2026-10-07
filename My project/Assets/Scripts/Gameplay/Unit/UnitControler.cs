using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class UnitControler : MonoBehaviour
{
    [SerializeField]
    private GameObject distractorPrefab, stimulusPrefab;
    private PCGController pcgController;
    private GameObject spawnedUnit;
    private float offset = 0.1f;
    private bool isPlaying = false;
    [SerializeField] private TextMeshProUGUI TextMeshProUGUI;

    private void Start()
    {
        pcgController = GetComponent<PCGController>();
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Play();
        }
    }

    public void Play()
    {
        isPlaying = !isPlaying;
        if (isPlaying)
        {
            StartCoroutine(SpawnLoop());
            TextMeshProUGUI.text = "Playing";
        }
    }

    private void ObjectSpawn()
    {
        float randomX = 0f;
        float randomY = 0f;
        float targetX = 0f;
        float targetY = 0f;
        int randomizeDirection = Random.Range(0, 4);
        bool spawn = Random.value < pcgController.currentDistractorRatio;
        GameObject spawnedPrefab = spawn ? distractorPrefab : stimulusPrefab;
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
        spawnedUnit = Instantiate(spawnedPrefab, spawnPosition, Quaternion.identity);
        if (spawnedUnit.TryGetComponent(out UnitMove unitMover))
        {
            unitMover.Move(Camera.main.ViewportToWorldPoint(new Vector3(targetX, targetY, 10f)));
            unitMover.telemetryLogger = this.GetComponent<TelemetryLogger>();
            unitMover.Speed = pcgController.currentObjectSpeed;
        }
    }
    IEnumerator SpawnLoop()
    {
        while (isPlaying)
        {
            ObjectSpawn();
            yield return new WaitForSeconds(pcgController.currentSpawnInterval);
        }
    }

}
