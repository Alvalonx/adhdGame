using UnityEngine;
using PrimeTween;

public class UnitMove : MonoBehaviour
{
    [SerializeField]
    private float duration = 5f;

    public void Move(Vector3 targetPosition)
    {
        Tween.Position(transform, targetPosition, duration, Ease.Linear)
             .OnComplete(() => this.gameObject.SetActive(false));
        //Debug.Log($"Moving {gameObject.name} to {targetPosition}");
    }
}
