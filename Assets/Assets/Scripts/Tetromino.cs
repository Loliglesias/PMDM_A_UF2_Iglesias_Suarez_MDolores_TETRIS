using UnityEngine.Tilemaps;
using UnityEngine;


// Enumeración dos tetrominos de xogo
public enum Tetromino
{
    I, 
    O,
    T,   
    J, 
    L, 
    S,    
    Z,   

}

// Enumeración dos prefabs dos tetrominos

public enum TetrominoPrefab
{
    IPrefab,
    OPrefab,
    TPrefab,
    JPrefab,
    LPrefab,
    SPrefab,
    ZPrefab
}

// Estructura para a inicialización dos tetrominos
[System.Serializable]
public struct TetrominoData
{
    public Tetromino tetromino;
    public TetrominoPrefab prefabEnum;
    public Tile tile;
    public Vector2Int[] cells {get; private set;}
    public Vector2Int[,] wallKicks {get; private set;}

    public void Initialize() 
    {
        this.cells = Data.Cells[this.tetromino];
        this.wallKicks = Data.WallKicks[this.tetromino];
    }
}

