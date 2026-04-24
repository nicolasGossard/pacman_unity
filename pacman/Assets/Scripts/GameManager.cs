using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject pelletPrefab;
    public GameObject[,] pelletObjects;

    public GameObject powerPelletPrefab;
    public GameObject[,] powerPelletObjects;

    public GameObject pacmanPrefab;

    public GameObject blinkyPrefab;
    public GameObject pinkyPrefab;
    public GameObject inkyPrefab;
    public GameObject clydePrefab;

    public float cellSize = 8f;

    public string[] originalMap = {
        "############################",
        "#............##............#",
        "#.####.#####.##.#####.####.#",
        "#o####.#####.##.#####.####o#",
        "#.####.#####.##.#####.####.#",
        "#..........................#",
        "#.####.##.########.##.####.#",
        "#.####.##.########.##.####.#",
        "#......##....##....##......#",
        "######.##### ## #####.######",
        "     #.##### ## #####.#     ",
        "     #.##          ##.#     ",
        "     #.## ######## ##.#     ",
        "######.## #      # ##.######",
        "      .   #      #   .      ",
        "######.## #      # ##.######",
        "     #.## ######## ##.#     ",
        "     #.##          ##.#     ",
        "     #.## ######## ##.#     ",
        "######.## ######## ##.######",
        "#............##............#",
        "#.####.#####.##.#####.####.#",
        "#.####.#####.##.#####.####.#",
        "#o..##.......  .......##..o#",
        "###.##.##.########.##.##.###",
        "###.##.##.########.##.##.###",
        "#......##....##....##......#",
        "#.##########.##.##########.#",
        "#.##########.##.##########.#",
        "#..........................#",
        "############################"
    };

    public char[][] map;

    private void Start()
    {
        map = new char[originalMap.Length][];

        for (int i = 0; i < originalMap.Length; i++)
        {
            map[i] = originalMap[i].ToCharArray();
        }

        pelletObjects = new GameObject[map.Length, map[0].Length];
        powerPelletObjects = new GameObject[map.Length, map[0].Length];

        Vector2 origin = new(-224f / 2f + cellSize / 2f, 248f / 2f - cellSize / 2f);

        SpawnCharacters(origin);
        
        for (int y = 0; y < map.Length; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                char tile = map[y][x];

                Vector2 position = origin + new Vector2(x * cellSize, -y * cellSize);

                if (tile == '.')
                {
                    pelletObjects[y, x] = Instantiate(pelletPrefab,position,Quaternion.identity);
                }
                else if (tile == 'o')
                {
                    powerPelletObjects[y, x] = Instantiate(powerPelletPrefab, position, Quaternion.identity);
                }
            }
        }
    }

    private void SpawnCharacters(Vector2 origin)
    {
        Spawn(pinkyPrefab, new Vector2Int(13, 14), origin);
        Spawn(blinkyPrefab, new Vector2Int(13, 11), origin);
        Spawn(inkyPrefab, new Vector2Int(11, 14), origin);
        Spawn(clydePrefab, new Vector2Int(15, 14), origin);
        Spawn(pacmanPrefab, new Vector2Int(13, 23), origin);
    }

    void Spawn(GameObject prefab, Vector2Int gridPos, Vector2 origin)
    {
        Vector2 worldPos = origin + new Vector2(gridPos.x * cellSize, -gridPos.y * cellSize);
        worldPos.x += cellSize / 2f;
        Instantiate(prefab, worldPos, Quaternion.identity);
    }
}
