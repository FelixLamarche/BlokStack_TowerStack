using System;
using System.Collections;
using UnityEngine;

// TODO unused Code
public class BlockWithPhysics : BlockTowerElement
{
    [SerializeField]
    private float timeToSlideToCenter = 0.2f;

    FixedJoint2D fixedJoint2D;
    // TOD make blocks fall at a constant rate
    // TOD check if moving center of mass makes it easier
    //      Locking the Z-rotation also allows the tower to stick together while ignoring the center of mass
    void Awake()
    {
        fixedJoint2D = GetComponent<FixedJoint2D>();
        Score = 1;
    }

    void OnCollisionEnter2D (Collision2D collision2D)
    {
        if(TowerIn == null)
        {
            // Check if it was the top block or the blocktower then attach itself to it
            BlockTowerElement collidedBlockElement;
            if(collision2D.gameObject.TryGetComponent<BlockTowerElement>(out collidedBlockElement) &&
                collidedBlockElement.TowerIn != null &&
                collidedBlockElement.TowerIn.IsTopOfTower(collidedBlockElement))
            {
                StartCoroutine(EnterTower(collidedBlockElement));
            }
        }
    }

    IEnumerator EnterTower(BlockTowerElement blockBelow)
    {
        TowerIn = blockBelow.TowerIn;
        TowerIn.AddBlockElement(this); // Keep line after getting the top block of the tower
        yield return StartCoroutine(SlideToCenterOfTower(blockBelow));

        fixedJoint2D.connectedBody = blockBelow.GetComponent<Rigidbody2D>();
        fixedJoint2D.connectedAnchor = Vector2.up * (blockBelow.Size.y / 2 + Size.y / 2);
        fixedJoint2D.enabled = true;
    }


    // TOD stop the blocks from rotating while sliding
    // TOD make the blocks center in the tower while the tower is at rest, by making all of them slide to the tower's center
    // Slides the block to the center of the underneath's platform center
    IEnumerator SlideToCenterOfTower(BlockTowerElement blockBelow){
        float t = 0f;
        float deltaY = blockBelow.Size.y / 2 + Size.y / 2;

        while(t < 1)
        {
            t += Time.fixedDeltaTime / timeToSlideToCenter;

            float frameNewX = Mathf.SmoothStep(0, blockBelow.transform.position.x - transform.position.x, t);
            float frameNewY = Mathf.SmoothStep(0, blockBelow.transform.position.y - transform.position.y + deltaY, t);
            transform.Translate(frameNewX * Vector3.right + Vector3.up * frameNewY);
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, 
                blockBelow.transform.rotation.eulerAngles.z));
            yield return new WaitForFixedUpdate();
        }
    }
}
