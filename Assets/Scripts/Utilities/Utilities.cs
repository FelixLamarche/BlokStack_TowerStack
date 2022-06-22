using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static Vector3 GetSize(GameObject gameObject)
    {
        Bounds bounds = GetBounds(gameObject);
        return bounds.size;
    }
    public static Bounds GetBounds(GameObject gameObject)
    {
        Renderer render = gameObject.GetComponent<Renderer>();
        if (render != null)
            return render.bounds;

        Renderer childRender;
        Bounds bounds = new(gameObject.transform.position, Vector3.zero);
        foreach (Transform child in gameObject.transform)
        {
            childRender = child.GetComponent<Renderer>();
            if (childRender != null)
                bounds.Encapsulate(childRender.bounds);
            else
                bounds.Encapsulate(GetBounds(child.gameObject));
        }
        return bounds;
    }
}
