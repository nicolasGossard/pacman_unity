using UnityEngine;

public class MapManager : MonoBehaviour
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

    public Vector2 origin;
    public float cellSize = 8f;

    // On utilise le pattern singleton pour pouvoir accéder à MapManager depuis d'autres scripts sans avoir à faire des références publiques
    public static MapManager Instance;

    // Un tableau de string est immutable, on ne peut pas le modifier, c'est pour ça
    // qu'on va faire un tableau de tableau de char,pour pouvoir modifier les éléments
    // # = mur, . = pellet, o = power pellet, @ = tunnel exit, ' ' = vide
    public string[] originalMap = {
        "############################", // 0
        "#............##............#", // 1
        "#.####.#####.##.#####.####.#", // 2
        "#o####.#####.##.#####.####o#", // 3
        "#.####.#####.##.#####.####.#", // 4
        "#..........................#", // 5
        "#.####.##.########.##.####.#", // 6
        "#.####.##.########.##.####.#", // 7
        "#......##....##....##......#", // 8
        "######.##### ## #####.######", // 9
        "     #.##### ## #####.#     ", // 10
        "     #.##          ##.#     ", // 11
        "     #.## ######## ##.#     ", // 12
        "######.## #      # ##.######", // 13
        "@     .   #      #   .     @", // 14
        "######.## #      # ##.######", // 15
        "     #.## ######## ##.#     ", // 16
        "     #.##          ##.#     ", // 17
        "     #.## ######## ##.#     ", // 18
        "######.## ######## ##.######", // 19
        "#............##............#", // 20
        "#.####.#####.##.#####.####.#", // 21
        "#.####.#####.##.#####.####.#", // 22
        "#o..##.......  .......##..o#", // 23
        "###.##.##.########.##.##.###", // 24
        "###.##.##.########.##.##.###", // 25
        "#......##....##....##......#", // 26
        "#.##########.##.##########.#", // 27
        "#.##########.##.##########.#", // 28
        "#..........................#", // 29
        "############################"  // 30
    };

    // On fait un tableau de tableaux de caractères pour pouvoir modifier les éléments
    // de la map, contrairement à un tableau de string qui est immutable
    public char[][] map;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // On dit que le nombre de lignes de map est originalMap.Length, donc autant de lignes que dans lui, 
        // donc 31 lignes, par contre le nombres de colonnes on met rien pour l'instant, parce que c'est un
        // tableau de tableaux, et chaque ligne peut avoir un nombre différent de colonnes
        map = new char[originalMap.Length][];

        // Pour chaque ligne de originalMap, on convertit la ligne en tableau de caractères
        // avec ToCharArray(), et on l'assigne à la ligne correspondante de map
        for (int i = 0; i < originalMap.Length; i++)
        {
            map[i] = originalMap[i].ToCharArray();
        }

        // Ici on peut utiliser un tableau 2D classique [,] parce que toutes les lignes ont le même nombre
        // de colonnes, mais si on avait des lignes avec un nombre différent de colonnes, on aurait pas pu
        // faire ça, et on aurait du faire un tableau de tableaux de caractères comme map
        pelletObjects = new GameObject[map.Length, map[0].Length];
        powerPelletObjects = new GameObject[map.Length, map[0].Length];

        origin = new(-224f / 2f + cellSize / 2f, 248f / 2f - cellSize / 2f);

        SpawnCharacters(origin);

        for (int y = 0; y < map.Length; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                char tile = map[y][x];

                Vector2 position = GridToWorld(new Vector2Int(x, y));

                if (tile == '.')
                {
                    pelletObjects[y, x] = Instantiate(pelletPrefab, position, Quaternion.identity);
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

    private void Spawn(GameObject prefab, Vector2Int gridPos, Vector2 origin)
    {
        Vector2 worldPos = GridToWorld(gridPos);
        worldPos.x += cellSize / 2f;
        Instantiate(prefab, worldPos, Quaternion.identity);
    }

    public Vector2 GridToWorld(Vector2Int gridPos)
    {
        return origin + new Vector2(gridPos.x * cellSize, -gridPos.y * cellSize);
    }

    public void TryEatPellet(Vector2Int gridPos)
    {
        if (gridPos.y < 0 || gridPos.y >= map.Length ||
            gridPos.x < 0 || gridPos.x >= map[0].Length)
        {
            return;
        }

        char tile = map[gridPos.y][gridPos.x];

        if (tile == '.')
        {
            Destroy(pelletObjects[gridPos.y, gridPos.x]);
            pelletObjects[gridPos.y, gridPos.x] = null;
            map[gridPos.y][gridPos.x] = ' ';
            GameManager.Instance?.AddScore(10);
        }
        else if (tile == 'o')
        {
            Destroy(powerPelletObjects[gridPos.y, gridPos.x]);
            powerPelletObjects[gridPos.y, gridPos.x] = null;
            map[gridPos.y][gridPos.x] = ' ';
            GameManager.Instance?.AddScore(50);
        }
    }

    public bool IsTunnelExit(Vector2Int gridPos)
    {
        if (gridPos.y < 0 || gridPos.y >= map.Length ||
            gridPos.x < 0 || gridPos.x >= map[0].Length)
        {
            return false;
        }

        return map[gridPos.y][gridPos.x] == '@';
    }

    public Vector2Int GetOtherTunnelExit(Vector2Int tunnelExit)
    {
        int lastX = map[0].Length - 1;
        int destinationX = tunnelExit.x == 0 ? lastX : 0;

        return new Vector2Int(destinationX, tunnelExit.y);
    }
}
