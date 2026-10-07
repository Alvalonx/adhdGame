using System;
using UnityEngine;


public class DDACore : MonoBehaviour
{
    [Header("Aturan DDA")]
    [Tooltip("Masukkan file ScriptableObject aturan di sini")]
    public DifficultyRule rules;

    // EVENT: Perintah untuk PCG Controller
    public static event Action<DifficultyAction> OnDifficultyChange;

    private void OnEnable()
    {
        // Berlangganan (Subscribe) ke event milik TelemetryLogger
        TelemetryLogger.OnBatchReady += EvaluatePerformance;
        TelemetryLogger.OnIdleTimeout += HandleIdleExtreme;
    }

    private void OnDisable()
    {
        // Berhenti berlangganan saat objek mati (mencegah memory leak)
        TelemetryLogger.OnBatchReady -= EvaluatePerformance;
        TelemetryLogger.OnIdleTimeout -= HandleIdleExtreme;
    }

    /// <summary>
    /// Dipanggil otomatis ketika TelemetryLogger selesai mengumpulkan 1 batch data
    /// </summary>
    private void EvaluatePerformance(float avgRT, float accuracy)
    {
        if (rules == null)
        {
            Debug.LogError("[DDA Core] Difficulty Rules belum diisi di Inspector!");
            return;
        }

        Debug.Log($"[DDA Core] Menerima Data: Akurasi {accuracy * 100}%, Rata-rata RT {avgRT}s");

        DifficultyAction decision = DifficultyAction.Maintain;

        // KONDISI A: Fokus Tinggi (Cepat & Akurat)
        if (accuracy >= rules.highAccuracyThreshold && avgRT <= rules.fastRTThreshold)
        {
            decision = DifficultyAction.Increase;
            Debug.Log("[DDA Core] Keputusan: NAIKKAN KESULITAN");
        }
        // KONDISI B: Kelelahan/Frustrasi (Lambat ATAU Banyak Salah)
        // Catatan: Menggunakan OR (||) karena anak ADHD bisa saja akurat tapi sangat lambat, 
        // atau cepat tapi asal menekan (impulsif).
        else if (accuracy <= rules.lowAccuracyThreshold || avgRT >= rules.slowRTThreshold)
        {
            decision = DifficultyAction.Decrease;
            Debug.Log("[DDA Core] Keputusan: TURUNKAN KESULITAN");
        }
        // KONDISI C: Optimal (Di antara batas atas dan bawah)
        else
        {
            decision = DifficultyAction.Maintain;
            Debug.Log("[DDA Core] Keputusan: PERTAHANKAN KESULITAN");
        }

        // Kirim perintah ke PCG Controller
        OnDifficultyChange?.Invoke(decision);
    }

    /// <summary>
    /// Dipanggil otomatis ketika anak terdeteksi bengong / tidak fokus lama
    /// </summary>
    private void HandleIdleExtreme()
    {
        Debug.Log("[DDA Core] Anak kehilangan fokus! Mengirim perintah Extreme Decrease.");
        OnDifficultyChange?.Invoke(DifficultyAction.ExtremeDecrease);
    }
}