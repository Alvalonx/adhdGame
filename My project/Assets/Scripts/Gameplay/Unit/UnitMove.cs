using UnityEngine;
using PrimeTween;

public class UnitMove : MonoBehaviour
{
    public float Speed;
    private float spawnTime;
    private bool isClicked = false;
    public ObjectData objectData;
    public TelemetryLogger telemetryLogger;

    public void Move(Vector3 targetPosition)
    {
        spawnTime = Time.time;
        float distance = Vector3.Distance(transform.position, targetPosition);
        float duration = distance / Speed;

        if (duration <= 0f || float.IsInfinity(duration)) return;

        Tween.Position(transform, targetPosition, duration, Ease.Linear)
             .OnComplete(() => {
                 if (!isClicked)
                 {
                     HandleMiss();
                 }
                 this.gameObject.SetActive(false);
             });
    }
    private void OnMouseDown()
    {
        if (isClicked) return;
        isClicked = true;

        float reactionTime = Time.time - spawnTime;

        if (telemetryLogger != null)
        {
            telemetryLogger.RecordData(reactionTime, true, objectData.isDistractor);
        }

        Tween.StopAll(transform);
        Destroy(gameObject);
    }

    private void HandleMiss()
    {
        // Hitung waktu sejauh ini (meskipun untuk miss, RT akan diabaikan oleh TelemetryLogger)
        float reactionTime = Time.time - spawnTime;

        // MENGIRIM DATA MISS: (Waktu, Diklik = false, Status Distractor)
        if (telemetryLogger != null)
        {
            telemetryLogger.RecordData(reactionTime, false, objectData.isDistractor);
        }
    }
}
