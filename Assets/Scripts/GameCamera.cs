using System.Collections;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    const float timeToFitTower = 2f;

    [SerializeField]
    float startingOrthogonalSize = 7.5f;
    [SerializeField]
    float maximumOrthogonalSize = 13f;
    [SerializeField]
    float marginOrthogonalSize = 3f;

    [SerializeField]
    BackgroundGradient background;
    [SerializeField]
    BlockTower blockTower;

    [SerializeField]
    PlatformWall leftSidePlatformWall;
    [SerializeField]
    PlatformWall rightSidePlatformWall;

    [SerializeField]
    Camera cameraView;

    Coroutine resizingCameraCoroutine;
    float oldTowerHeight = 0f;
    Vector3 startingPosition;

    void Awake()
    {
        startingPosition = transform.position;
        ResizeWalls();
    }

    void Start()
    {
        ResetCamera();
    }

    void Update()
    {
        ResizeCameraToFitTower();
    }

    // Calculate the height difference required to spawn the blocks out of the camera's frame
    public float CalculateEdgeOfScreenHeight(float zPos, VerticalDirection direction)
    {
        // Remove angle, tan(angle) * adjacent = opposite -> y displacement
        float yDeltaByAngle = Mathf.Tan(cameraView.transform.rotation.eulerAngles.x * Mathf.Deg2Rad) * 
            Mathf.Abs(cameraView.transform.position.z - zPos);
        float yPos = cameraView.transform.position.y;

        if(direction == VerticalDirection.above)
            yPos += cameraView.orthographicSize;
        else if(direction == VerticalDirection.below)
            yPos -= cameraView.orthographicSize;
        if(cameraView.transform.rotation.eulerAngles.x >= 0)
            yPos -= yDeltaByAngle;
        else if(cameraView.transform.rotation.eulerAngles.x < 0)
            yPos += yDeltaByAngle;

        return yPos;
    }

    // Only works for orthographic camera
    public void ResetCamera()
    {
        if(resizingCameraCoroutine != null)
            StopCoroutine(resizingCameraCoroutine);

        resizingCameraCoroutine = null;
        cameraView.orthographicSize = startingOrthogonalSize;
        transform.position = startingPosition;
        background.ScaleToFitOrthographicCameraView(cameraView.orthographicSize);
        ResizeWalls();
    }

    void ResizeCameraToFitTower()
    {
        float topOfTowerY = blockTower.GetTowerHeight();
        if(oldTowerHeight.Equals(topOfTowerY))
            return;

        oldTowerHeight = topOfTowerY;
        float newOrthographicSize = Mathf.Max(topOfTowerY, startingOrthogonalSize);
        newOrthographicSize = Mathf.Min(newOrthographicSize, maximumOrthogonalSize);
        float yHeightCamera = Mathf.Max(topOfTowerY + marginOrthogonalSize, startingPosition.y);

        if(resizingCameraCoroutine != null)
            StopCoroutine(resizingCameraCoroutine);
        resizingCameraCoroutine = StartCoroutine(ResizingCameraToFit(newOrthographicSize, yHeightCamera));
    }

    IEnumerator ResizingCameraToFit(float orthogonalSizeGoal, float yHeightGoal)
    {
        float t = 0f;
        float sizeBefore = cameraView.orthographicSize;
        float yHeightBefore = transform.position.y;
        float yHeight = 0f;

        while(t < 1)
        {
            t += Time.deltaTime / timeToFitTower;
            cameraView.orthographicSize = Mathf.Lerp(sizeBefore, orthogonalSizeGoal, t);
            yHeight = Mathf.Lerp(yHeightBefore, yHeightGoal, t);
            background.ScaleToFitOrthographicCameraView(cameraView.orthographicSize);
            transform.position = new Vector3(transform.position.x, yHeight, transform.position.z);
            ResizeWalls();
            yield return null;
        }
        resizingCameraCoroutine = null;
    }

    void ResizeWalls()
    {
        float newDeltaXWalls = cameraView.orthographicSize * 9 / 16 + leftSidePlatformWall.transform.lossyScale.x / 2 + 
            blockTower.GetComponent<BoxCollider2D>().size.x / 2;
        leftSidePlatformWall.MoveToNewX(newDeltaXWalls);
        rightSidePlatformWall.MoveToNewX(newDeltaXWalls);
    }

    void OnValidate()
    {
        cameraView.orthographicSize = startingOrthogonalSize;
    }

}
public enum VerticalDirection {above, below};
