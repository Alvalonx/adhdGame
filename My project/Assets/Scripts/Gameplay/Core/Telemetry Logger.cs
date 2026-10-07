using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TelemetryLogger : MonoBehaviour
{
    [Header("Sliding Window Settings")]
    public int windowSize = 15;

    [Header("Idle Fallback Settings")]
    public float idleTimeoutLimit = 6.0f;
    private float currentIdleTime = 0f;

    // Mengganti nama queue agar lebih relevan dengan konteks baru
    private Queue<float> rtQueue = new Queue<float>();
    private Queue<bool> accuracyQueue = new Queue<bool>(); // Menyimpan 'apakah tindakan pemain benar?'

    public static event Action<float, float> OnBatchReady;
    public static event Action OnIdleTimeout;

    private void Update()
    {
        currentIdleTime += Time.deltaTime;

        if (currentIdleTime >= idleTimeoutLimit)
        {
            TriggerIdleTimeout();
        }
    }

    /// <summary>
    /// Update: Sekarang menerima parameter isDistractor dari ObjectData Anda.
    /// playerClicked = true jika anak menyentuh layar, false jika objek hilang sendiri dari layar.
    /// </summary>
    public void RecordData(float responseTime, bool playerClicked, bool isDistractor)
    {
        // Reset timer karena ada objek yang selesai diproses (diklik atau dibiarkan lewat)
        currentIdleTime = 0f;

        // 1. EVALUASI AKURASI TINDAKAN
        bool isCorrectAction = false;

        if (!isDistractor && playerClicked)
        {
            isCorrectAction = true; // BENAR: Menekan Target
        }
        else if (isDistractor && !playerClicked)
        {
            isCorrectAction = true; // BENAR: Membiarkan Distractor lewat (Correct Rejection)
        }
        // Jika Target tapi tidak diklik -> Omission Error (isCorrectAction = false)
        // Jika Distractor tapi diklik -> Commission Error / Impulsif (isCorrectAction = false)

        accuracyQueue.Enqueue(isCorrectAction);

        // 2. EVALUASI RESPONSE TIME (RT)
        // Kita HANYA mencatat RT jika anak benar-benar menekan layar (baik itu target atau distractor).
        // Jika dia diam saja, kita masukkan nilai -1 sebagai penanda untuk diabaikan.
        if (playerClicked)
        {
            rtQueue.Enqueue(responseTime);
        }
        else
        {
            rtQueue.Enqueue(-1f);
        }

        // 3. BATASI JENDELA
        if (accuracyQueue.Count > windowSize)
        {
            accuracyQueue.Dequeue();
            rtQueue.Dequeue();
        }

        // 4. PICU DDA JIKA DATA SUDAH CUKUP
        if (accuracyQueue.Count == windowSize)
        {
            EvaluateBatch();
        }
    }

    private void EvaluateBatch()
    {
        // Hitung akurasi dari seluruh tindakan (baik menekan maupun mengabaikan)
        int correctCount = accuracyQueue.Count(correct => correct == true);
        float accuracy = (float)correctCount / windowSize;

        // Filter RT: Hanya hitung rata-rata dari objek yang BENAR-BENAR diklik (RT > 0)
        var validRTs = rtQueue.Where(rt => rt > 0).ToList();

        // Mencegah error jika dalam 15 window anak sama sekali tidak menekan (hanya memandangi layar)
        float avgRT = validRTs.Count > 0 ? validRTs.Average() : 0f;

        OnBatchReady?.Invoke(avgRT, accuracy);
    }

    private void TriggerIdleTimeout()
    {
        currentIdleTime = 0f;
        accuracyQueue.Clear();
        rtQueue.Clear();

        Debug.Log("[Telemetry Logger] Idle Timeout! Anak kehilangan fokus.");
        OnIdleTimeout?.Invoke();
    }
}