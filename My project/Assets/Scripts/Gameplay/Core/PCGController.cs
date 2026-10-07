using UnityEngine;

public class PCGController : MonoBehaviour
{
    [Header("Current Difficulty Parameters (Read Only / Base)")]
    [Tooltip("Waktu tunggu antar kemunculan objek (detik)")]
    public float currentSpawnInterval = 2.0f;
    [Tooltip("Kecepatan gerak objek di layar")]
    public float currentObjectSpeed = 3.0f;
    [Range(0f, 1f)]
    [Tooltip("Persentase kemungkinan munculnya distractor (0.2 = 20%)")]
    public float currentDistractorRatio = 0.2f;

    [Header("PCG Adjustment Steps (Langkah Perubahan)")]
    [Tooltip("Seberapa banyak interval dikurangi/ditambah setiap ganti level")]
    public float intervalStep = 0.2f;
    [Tooltip("Seberapa banyak kecepatan ditambah/dikurangi setiap ganti level")]
    public float speedStep = 0.5f;
    [Tooltip("Berapa persen penambahan/pengurangan distractor")]
    public float distractorStep = 0.1f;

    [Header("Safety Bounds (Batas Minimum & Maksimum)")]
    public float minInterval = 0.5f;
    public float maxInterval = 3.5f;
    public float minSpeed = 1.5f;
    public float maxSpeed = 8.0f;
    [Tooltip("Jangan sampai 100% distractor, nanti tidak ada target!")]
    public float maxDistractorRatio = 0.5f;

    private void OnEnable()
    {
        // Mendengarkan instruksi dari DDACore
        DDACore.OnDifficultyChange += HandleDifficultyChange;
    }

    private void OnDisable()
    {
        // Berhenti mendengarkan saat objek mati
        DDACore.OnDifficultyChange -= HandleDifficultyChange;
    }

    /// <summary>
    /// Fungsi yang dieksekusi setiap kali DDACore mengambil keputusan
    /// </summary>
    private void HandleDifficultyChange(DifficultyAction action)
    {
        switch (action)
        {
            case DifficultyAction.Increase:
                // Level Naik: Game makin cepat, interval makin rapat, distractor makin banyak
                currentSpawnInterval -= intervalStep;
                currentObjectSpeed += speedStep;
                currentDistractorRatio += distractorStep;
                Debug.Log("[PCG] Kesulitan NAIK. Kecepatan++, Distractor++");
                break;

            case DifficultyAction.Decrease:
                // Level Turun: Game melambat, jeda makin lama, distractor berkurang
                currentSpawnInterval += intervalStep;
                currentObjectSpeed -= speedStep;
                currentDistractorRatio -= distractorStep;
                Debug.Log("[PCG] Kesulitan TURUN. Kecepatan--, Distractor--");
                break;

            case DifficultyAction.ExtremeDecrease:
                // Fallback untuk Idle/Spacing Out: 
                // Langsung buat game sangat lambat dan hilangkan semua distractor agar anak fokus ke target
                currentSpawnInterval = maxInterval;
                currentObjectSpeed = minSpeed;
                currentDistractorRatio = 0f;
                Debug.Log("[PCG] EXTREME DECREASE! Mode atensi diaktifkan (0 Distractor).");
                break;

            case DifficultyAction.Maintain:
                // Tidak melakukan apa-apa
                Debug.Log("[PCG] Kesulitan DIPERTAHANKAN.");
                break;
        }

        // Terapkan Clamping agar nilai tidak menembus batas kewajaran yang bisa dimainkan manusia
        ApplyClamps();
    }

    private void ApplyClamps()
    {
        currentSpawnInterval = Mathf.Clamp(currentSpawnInterval, minInterval, maxInterval);
        currentObjectSpeed = Mathf.Clamp(currentObjectSpeed, minSpeed, maxSpeed);
        currentDistractorRatio = Mathf.Clamp(currentDistractorRatio, 0f, maxDistractorRatio);
    }
}