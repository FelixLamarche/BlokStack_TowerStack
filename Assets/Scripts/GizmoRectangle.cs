using UnityEngine;

public class GizmoRectangle : MonoBehaviour
{
    [SerializeField]
    Color color;

    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawCube(transform.position, transform.lossyScale);
    }
}
