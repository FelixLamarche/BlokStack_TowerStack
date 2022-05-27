using UnityEngine;

// Fits an object to take up the entirerity of the camera view only by scaling the object as to not move it
[ExecuteInEditMode]
public class FitToPerspectiveCameraView : MonoBehaviour
{
    void Awake()
    {
        ScaleToFitScreen();
    }

    void ScaleToFitScreen()
    {
        float distanceToCamera = Vector3.Distance(transform.position, Camera.main.transform.position);
        float cameraFOV = Camera.main.fieldOfView;
        float aspectRatio = Camera.main.aspect;

        float heightScale = Mathf.Tan(Mathf.Deg2Rad * cameraFOV / 2) * distanceToCamera / 5;
        float widthScale = heightScale * aspectRatio;

        transform.localScale = new Vector3(widthScale, 1, heightScale);
    }
}
