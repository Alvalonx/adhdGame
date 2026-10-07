using UnityEngine;
using UnityEngine.U2D;

public class UnitBehaviour : MonoBehaviour
{   
    public ObjectData objectData;   
    public void OnMouseDown()
    {
        gameObject.SetActive(false);
    }
}
