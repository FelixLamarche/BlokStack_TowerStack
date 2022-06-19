using UnityEngine;

public class FallingBlock : BlockTowerElement
{
    public bool followBlockBelow = true;

    bool followPerfectlyBlockBelow = true;
    float xDeltaBlockBelow = 0f; // DeltaX of the block below at the first frame of this block entering the tower
    BlockTowerElement blockBelow;

    Rigidbody2D rb;
    BoxCollider2D boxCollider2D;
    FixedJoint2D fixedJoint2D;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        fixedJoint2D = GetComponent<FixedJoint2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        Height = transform.lossyScale.y;
        Score = 1;
    }

    void FixedUpdate()
    {
        FollowTower();
    }

    public void FollowTower()
    {
        if(blockBelow is null || !followBlockBelow) return;

        // For "simultaneous movements" start tracking block above, and recursively call every single moveposition
        if(followPerfectlyBlockBelow)
            rb.MovePosition(new Vector3 (blockBelow.transform.position.x, transform.position.y, transform.position.z));
        else // Else remember the original offset
            rb.MovePosition(new Vector3 (blockBelow.transform.position.x + xDeltaBlockBelow, transform.position.y, transform.position.z));
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

        // Check if it was the top block of the blocktower then attach itself to it
        BlockTowerElement collidedBlockElement;
        if(collision2D.gameObject.TryGetComponent<BlockTowerElement>(out collidedBlockElement) &&
            collidedBlockElement.TowerIn != null &&
            collidedBlockElement.TowerIn.IsTopOfTower(collidedBlockElement))
        {
            // TODO check if block is close enough to the top to enter, touching the top block is not sufficient
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
        float newY = blockBelow.transform.position.y + blockBelow.Height / 2 + transform.lossyScale.y / 2;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        xDeltaBlockBelow = transform.position.x - blockBelow.transform.position.x;
        blockTower.AddBlockElement(this);
    }
}
