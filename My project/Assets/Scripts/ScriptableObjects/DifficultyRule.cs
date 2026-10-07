using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyRule", menuName = "Scriptable Objects/DifficultyRule")]
public class DifficultyRule : ScriptableObject
{
    [Header("Fokus Tinggi (Syarat Naik Level)")]
    [Tooltip("Minimal akurasi untuk naik level (contoh: 0.8 = 80%)")]
    public float highAccuracyThreshold = 0.8f;
    [Tooltip("Maksimal rata-rata response time untuk naik level (detik)")]
    public float fastRTThreshold = 1.0f;

    [Header("Kelelahan/Frustrasi (Syarat Turun Level)")]
    [Tooltip("Akurasi di bawah angka ini akan menurunkan level")]
    public float lowAccuracyThreshold = 0.5f;
    [Tooltip("Rata-rata response time di atas angka ini akan menurunkan level")]
    public float slowRTThreshold = 1.8f;
}
