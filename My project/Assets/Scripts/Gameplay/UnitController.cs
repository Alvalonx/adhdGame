using UnityEngine;

public class UnitController : MonoBehaviour
{
    private GameObject unit = null;

    private void Awake()
    {
        unit = this.gameObject;
    }
}
