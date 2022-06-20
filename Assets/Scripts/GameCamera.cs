using System.Collections;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    float startingOrthogonalSize;
    [SerializeField]
    float maximumOrthogonalSize = 13f;
    [SerializeField, Tooltip("Height between the top of the screen and the top of the tower")]
    float marginOrthogonalSize = 3f;

    const float minTimeToFitTowerInFrame = 1f;

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
        startingOrthogonalSize = cameraView.orthographicSize;
        ResizeWalls();

    }

    void Start()
    {
        ResetCamera();
    }

    void LateUpdate()
    {
        ResizeCameraToFitTower();
    }

    // Calculate the height of the edge of the screen of the given direction
    public float CalculateVerticalEdgeOfScreen(float zPos, VerticalDirection direction)
    {
        // Remove angle, tan(angle) * adjacent = opposite -> y displacement
        float yAngleDisplacement = Mathf.Abs(Mathf.Tan(cameraView.transform.rotation.eulerAngles.x * Mathf.Deg2Rad) * 
            (zPos - cameraView.transform.position.z));
        float yPos = cameraView.transform.position.y;

        if(direction == VerticalDirection.above)
            yPos += cameraView.orthographicSize / Mathf.Cos(cameraView.transform.rotation.eulerAngles.x * Mathf.Deg2Rad);
        else if(direction == VerticalDirection.below)
            yPos -= cameraView.orthographicSize / Mathf.Cos(cameraView.transform.rotation.eulerAngles.x * Mathf.Deg2Rad);
        if(cameraView.transform.rotation.eulerAngles.x >= 0)
            yPos -= yAngleDisplacement;
        else if(cameraView.transform.rotation.eulerAngles.x < 0)
            yPos += yAngleDisplacement;

        return yPos;
    }

    public float CameraWidth()
    {
        return this.cameraView.aspect * this.cameraView.orthographicSize * 2;
    }

    // Factors in the angle of the orthographic camera
    // Returns the world position
    // screenPos.z is the depth of the desired object
    public Vector2 ScreenPositionToWorldPoint2D(Vector3 screenPos)
    {
        float angle = cameraView.transform.rotation.eulerAngles.x;
        
        // length of the Y displacement of the difference between the current z depth and the camera depth
        // caused by the angle of the camera
        float yAngleDisplacement = Mathf.Tan(angle * Mathf.Deg2Rad) * 
            (screenPos.z - cameraView.transform.position.z);

        // This represents the additional height gained from the projection of a straight plane onto the angled camera plane
        float projectedHeightMaximum = Mathf.Tan(angle * Mathf.Deg2Rad) * 2 * cameraView.orthographicSize;
        projectedHeightMaximum *= Mathf.Sin(angle * Mathf.Deg2Rad);

        float screenViewPortHeightClamped = Mathf.Clamp01(cameraView.ScreenToViewportPoint(screenPos).y);
        // Need to reverse, as the maximum angle discrepancy comes at the bottom of the screen which maps to 0 in viewport.
        // So we reverse it.
        float lerpValueHeight = Mathf.Abs(1 - screenViewPortHeightClamped);

        float ySecondAngleDisplacement = Mathf.Lerp(0, projectedHeightMaximum, lerpValueHeight);

        Vector2 worldPoint = cameraView.ScreenToWorldPoint(screenPos);
        worldPoint -= Vector2.up * (yAngleDisplacement + ySecondAngleDisplacement);

        return worldPoint;
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
        float yHeight;

        float timeToFitTower = minTimeToFitTowerInFrame;
        BlockSpawner blockSpawner = FindObjectOfType<BlockSpawner>();
        if(blockSpawner != null) 
            timeToFitTower = Mathf.Min(timeToFitTower, blockSpawner.CurTimeBetweenSpawns);

        while(t < 1)
        {
            t += Time.deltaTime / timeToFitTower;
            cameraView.orthographicSize = Mathf.Lerp(sizeBefore, orthogonalSizeGoal, t);

            yHeight = Mathf.Lerp(yHeightBefore, yHeightGoal, t);
            transform.position = new Vector3(transform.position.x, yHeight, transform.position.z);

            background.ScaleToFitOrthographicCameraView(cameraView.orthographicSize);
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

}
public enum VerticalDirection {above, below};
