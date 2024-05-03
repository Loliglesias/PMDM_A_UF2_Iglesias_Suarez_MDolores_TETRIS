using UnityEngine;
using UnityEngine.Tilemaps;

public class GhostBlock : MonoBehaviour
{
    /*
    Clase para crear a peza fantasma que axuda a marcar a posición final da peza activa
    */
    public Tile tile; // Instancia do tile
    public BoardBlock board; // Instancia de board
    public PieceBlock trackingPiece; // Instancia de peza
    public Tilemap tilemap {get; private set; } // Instancia do tilemap
    public Vector3Int[] cells {get; private set;} // Instancia das celdas do grid
    public Vector3Int position {get; private set;} // Posición


    /*
    Ao arrancar, instanciamos as celdas e o tilemap
    */
    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.cells = new Vector3Int[4];

    }
    /*
    Update: 
    - Limpamos a peza antiga
    - Copiamos a activa
    - Baixámola ao fondo do taboleiro
    - Situámola onde poidamos sen ocupar o sitio de outra peza
    */
    private void LateUpdate()
    {
            Clear();
            Copy();
            Drop();
            Set();
    }

    /*
    Limpamos a peza para que non se solapen coas seguintes que vaian saíndo
    */
    private void Clear()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePosition = this.cells[i] + this.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }

    /*
    Copiamos a peza activa
    */
    private void Copy()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            this.cells[i] = this.trackingPiece.cells[i];
        }
    }
    /*
    Colocar esta peza fantasma na parte máis baixa
    */
    private void Drop()
    {
        if (board != null)
        {
            Vector3Int position = trackingPiece.position;
            int current = position.y;
            int bottom = -board.boardSize.y / 2 - 1;
            board.Clear(trackingPiece);
            for (int row = current; row >= bottom; row--)
            {
                position.y = row;
                if (board.IsValidPosition(trackingPiece, position))
                {
                    this.position = position;
                }
                else
                {
                    break;
                }
            }
            board.Set(trackingPiece);
        }
    }

    /*
    Verificación da posición por se hai algo nese sitio e establece a peza fantasma onde non hai outras pezas ocupando
    */
    private void Set()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePosition = this.cells[i] + this.position;

            if (this.board.tilemap.HasTile(tilePosition))
            {
                continue;
            }
            this.tilemap.SetTile(tilePosition, this.tile);
        }
    }

}
