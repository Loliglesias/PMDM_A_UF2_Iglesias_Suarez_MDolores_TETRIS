using UnityEngine;
using UnityEngine.Tilemaps;


/*
Clase para crear una primeira animación de fondo dos bloques
*/
public class TileAnimation : MonoBehaviour
{
    public Sprite[] tileSprites; // Array dos distintos tiles que se poden instanciar
    public float fallSpeed = 2f; // Velocidade á que caen os tiles
    public Tilemap tilemap; // Referencia ao tilemap
    public Vector2Int boardSize = new Vector2Int(10, 20); // Tamaño do borde

    /*
    Ao arrancar a clase xera os tiles cada 0.05 segundos
    */
    void Start()
    {
        InvokeRepeating("GenerateTile", 0.0f, 0.05f);
    }

    /*
    Marcamos os bordes
    */
    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    /*
    Método para xerar os bloques aleatorios e movelos polo tilemap
    */
    private void GenerateTile()
    {
        // Instancia o bloque e pono nunha posición aleatoria
        GameObject tile = new GameObject("Tile");
        SpriteRenderer renderer = tile.AddComponent<SpriteRenderer>();
        renderer.sprite = tileSprites[Random.Range(0, tileSprites.Length)];

        float randomX = Random.Range(-7,8); // Axusta a posición de X aleatoriamente 
        Vector3 randomPosition = new Vector3(randomX, 12, 0);
        Vector3Int cellPosition = tilemap.WorldToCell(randomPosition);
        tile.transform.position = tilemap.GetCellCenterWorld(cellPosition); // Aliña a peza no medio da celda

        // Agregar un Rigidbody2D para que caia o bloque cara abaixo
        Rigidbody2D rb = tile.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0.5f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Bloquear a rotación

        // Obtemos o tamaño do sprite
        Vector2 spriteSize = renderer.sprite.bounds.size;
        BoxCollider2D collider = tile.AddComponent<BoxCollider2D>();

        // Reduce o tamaño do collider para que non se solapen
        float reductionFactor = 0.9f;
        Vector2 colliderSize = new Vector2(spriteSize.x * reductionFactor, spriteSize.y * reductionFactor);
        collider.size = colliderSize;

        // Agrégalle o script para mover os tiles
        tile.AddComponent<TileMovement>();
    }
}
