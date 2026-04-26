using UnityEngine;

public class Pacman : MonoBehaviour
{
    private const int AnimationFrameCount = 4;

    [Header("Animation")]
    [SerializeField] private Sprite[] eatSprites;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private float eatTime = 0.2f;
    private float timer = 0f;
    private int eatFrame = 0;

    [Header("Movement")]
    [SerializeField] private float speed = 100f;

    // Grille
    private Vector2Int currentTile;
    private Vector2Int targetTile;

    // Directions
    private Vector2Int gridDirection;
    private Vector2Int nextDirection;

    void Start()
    {
        // Direction initiale (droite)
        gridDirection = Vector2Int.right;
        nextDirection = gridDirection;

        // Position initiale dans la grille
        currentTile = WorldToGrid(transform.position);
        targetTile = currentTile + gridDirection;
        MapManager.Instance.TryEatPellet(currentTile);

        AnimateEat();
    }

    void Update()
    {
        Turn();
        Move();
        AnimateEat();
    }

    private void Turn()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.W))
            nextDirection = Vector2Int.down;

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            nextDirection = Vector2Int.up;

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.A))
            nextDirection = Vector2Int.left;

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            nextDirection = Vector2Int.right;
    }

    private void Move()
    {
        Vector2 targetWorldPos = MapManager.Instance.GridToWorld(targetTile);

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetWorldPos,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, targetWorldPos) < 0.01f)
        {
            transform.position = targetWorldPos;
            currentTile = targetTile;
            MapManager.Instance.TryEatPellet(currentTile);

            if (MapManager.Instance.IsTunnelExit(currentTile))
            {
                TeleportToOtherSide();
                return;
            }

            Vector2Int tryTurnTile = currentTile + nextDirection;

            if (!IsWall(tryTurnTile))
            {
                gridDirection = nextDirection;
            }

            Vector2Int nextTile = currentTile + gridDirection;

            if (!IsWall(nextTile))
            {
                targetTile = nextTile;
            }
            else
            {
                targetTile = currentTile;
            }
        }
    }

    private void TeleportToOtherSide()
    {
        currentTile = MapManager.Instance.GetOtherTunnelExit(currentTile);
        transform.position = MapManager.Instance.GridToWorld(currentTile);

        Vector2Int nextTile = currentTile + gridDirection;
        targetTile = IsWall(nextTile) ? currentTile : nextTile;
    }

    private bool IsWall(Vector2Int gridPos)
    {
        if (gridPos.y < 0 || gridPos.y >= MapManager.Instance.map.Length ||
            gridPos.x < 0 || gridPos.x >= MapManager.Instance.map[0].Length)
        {
            return true;
        }

        return MapManager.Instance.map[gridPos.y][gridPos.x] == '#';
    }

    private Vector2Int WorldToGrid(Vector2 worldPos)
    {
        Vector2 origin = MapManager.Instance.origin;
        float cellSize = MapManager.Instance.cellSize;

        int x = Mathf.RoundToInt((worldPos.x - origin.x) / cellSize);
        int y = Mathf.RoundToInt(-(worldPos.y - origin.y) / cellSize);

        return new Vector2Int(x, y);
    }

    private void AnimateEat()
    {
        if (eatSprites == null || eatSprites.Length < 9 || sr == null)
        {
            return;
        }

        int directionIndex = GetDirectionSpriteIndex();

        if (IsBlockedByWall())
        {
            timer = 0f;
            eatFrame = 2;
            sr.sprite = eatSprites[directionIndex];
            return;
        }

        timer += Time.deltaTime;

        if (timer >= eatTime)
        {
            timer = 0f;
            eatFrame++;

            if (eatFrame >= AnimationFrameCount)
            {
                eatFrame = 0;
            }
        }

        if (eatFrame == 0)
        {
            sr.sprite = eatSprites[0];
            return;
        }

        int mouthFrame = eatFrame == 2 ? 1 : 0;
        sr.sprite = eatSprites[directionIndex + mouthFrame];
    }

    private bool IsBlockedByWall()
    {
        Vector2Int nextTile = currentTile + gridDirection;
        return targetTile == currentTile && IsWall(nextTile);
    }

    private int GetDirectionSpriteIndex()
    {
        if (gridDirection == Vector2Int.right)
            return 1;

        if (gridDirection == Vector2Int.left)
            return 3;

        if (gridDirection == Vector2Int.down)
            return 5;

        return 7;
    }
}
