using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlock : BlockTowerElement
{
    public bool FollowBlockBelow { get; set; }

    [SerializeField]
    bool rememberOriginalHorizontalOffset = false;

    float xDeltaBlockBelow = 0f; // DeltaX of the block below at the first frame of this block entering the tower
    BlockTowerElement blockBelow;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Score = 1;
    }

    void FixedUpdate()
    {
        FollowTower();
    }

    public void FollowTower()
    {
        if(blockBelow is null || !FollowBlockBelow) return;

        // For "simultaneous movements" start tracking block above, and recursively call every single moveposition
        if(rememberOriginalHorizontalOffset)
            rb.MovePosition(new Vector3 (blockBelow.transform.position.x + xDeltaBlockBelow, transform.position.y, transform.position.z));
        else // Else remember the original offset
            rb.MovePosition(new Vector3 (blockBelow.transform.position.x, transform.position.y, transform.position.z));
    }

    void OnCollisionEnter2D(Collision2D collision2D)
    {
        CheckForTowerCollision(collision2D);
    }

    void OnCollisionStay2D(Collision2D collision2D)
    {
        CheckForTowerCollision(collision2D);
    }

    void CheckForTowerCollision(Collision2D collision2D) 
    {
        if(TowerIn != null) return;

        if (collision2D.gameObject.TryGetComponent(out BlockTowerElement collidedBlockElement) &&
            collidedBlockElement.TowerIn != null &&
            collidedBlockElement.TowerIn.IsTopOfTower(collidedBlockElement))
        {
            const float verticalMarginToEnterTower = 0.25f;
            float distanceCenters = collidedBlockElement.Size.y / 2 + Size.y / 2; // Required height over the tower's highest block
            float margin = transform.position.y - collidedBlockElement.transform.position.y;

            if(margin > distanceCenters - verticalMarginToEnterTower)
                EnterTower(collidedBlockElement.TowerIn);
        }
    }

    void EnterTower(BlockTower blockTower)
    {
        TowerIn = blockTower;
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        rb.bodyType = RigidbodyType2D.Kinematic;
        transform.rotation = Quaternion.Euler(new Vector3 (transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0f));

        // Assure that the blocks touch each others with no gaps whatsoever
        blockBelow = blockTower.GetTopBlockElement();
        float newY = blockBelow.transform.position.y + blockBelow.Size.y / 2 + Size.y / 2;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        xDeltaBlockBelow = transform.position.x - blockBelow.transform.position.x;
        blockTower.AddBlockElement(this);

        if (!rememberOriginalHorizontalOffset)
            StartCoroutine(SmoothEntryInTower());
        else
            FollowBlockBelow = true;
    }

    IEnumerator SmoothEntryInTower()
    {
        const float entryTime = 0.08f;
        float time = 0;
        float originalPos = transform.position.x;

        while(time <= entryTime)
        {
            yield return new WaitForFixedUpdate();
            time += Time.fixedDeltaTime;

            float newX = Mathf.Lerp(originalPos, blockBelow.transform.position.x, time / entryTime);

            rb.MovePosition(new Vector3(newX, transform.position.y, transform.position.z));
        }
        FollowBlockBelow = true;
    }
}
