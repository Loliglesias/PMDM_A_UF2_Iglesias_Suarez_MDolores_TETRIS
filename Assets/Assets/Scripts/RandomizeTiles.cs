using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

/*
Clase para crear os tiles block random dependendo da dificultade
*/
public enum DifficultyLevel
{
    Easy,
    Medium,
    Hard
}

public class RandomizeTiles : MonoBehaviour
{
    public Tilemap tilemap; // Instancia do tilemap onde estarán as pezas de bloqueo situadas e que é a mesma onde se crean as normais
    [SerializeField] PieceBlock piece; // Instancia de PieceBlock
    [SerializeField] GameManagerBlock game; // Instancia do Game Manager
    public DifficultyLevel difficultyLevel = DifficultyLevel.Easy; // Instanciamos por defecto a dificultade en fácil
    private int easyTileCount = 10; // Marcamos 10 tiles se é dificultade fácil
    private int mediumTileCount = 20; // Marcamos 20 tiles se é dificultade media
    private int hardTileCount = 30; // Marcamos 30 tiles se é dificultade difícil


    void Start()
    {

    }

    /*
    Obtemos as posicións dos tiles pintados
    Obtemos os tiles que temos que deixar dependendo da dificultade
    Seleccionamos as posicións aleatoriamente
    Desactivanse os demais
    */
    public void RandomizeTilesOnDifficulty()
    {

        List<Vector3Int> paintedTilePositions = GetPaintedTilePositions();

        int targetTileCount = GetTargetTileCount();

        List<Vector3Int> selectedPositions = SelectRandomTilePositions(paintedTilePositions, targetTileCount);

        DeactivateOrClearNonSelectedTiles(paintedTilePositions, selectedPositions);

    }

    /*
    Lista de posicións de tiles pintados
    */
    List<Vector3Int> GetPaintedTilePositions()
    {
        List<Vector3Int> paintedPositions = new List<Vector3Int>();

        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                paintedPositions.Add(pos);
            }
        }

        return paintedPositions;
        
    }

    /*
    Switch de definición de cantidade de tiles dependendo do nivel
    */
    int GetTargetTileCount()
    {
        switch (difficultyLevel)
        {
            case DifficultyLevel.Easy:
                return easyTileCount;
            case DifficultyLevel.Medium:
                return mediumTileCount;
            case DifficultyLevel.Hard:
                return hardTileCount;
            default:
                return 0;
        }
    }

    /*
    Selección random de posición de tiles
    */
    List<Vector3Int> SelectRandomTilePositions(List<Vector3Int> positions, int count)
    {
        List<Vector3Int> selectedPositions = new List<Vector3Int>();
        while (selectedPositions.Count < count && positions.Count > 0)
        {
            int randomIndex = Random.Range(0, positions.Count);
            selectedPositions.Add(positions[randomIndex]);
            positions.RemoveAt(randomIndex);
        }
        return selectedPositions;
    }

    /*
    Desactivar os tiles que non quedaran seleccionados
    */
    void DeactivateOrClearNonSelectedTiles(List<Vector3Int> allPositions, List<Vector3Int> selectedPositions)
    {
        foreach (Vector3Int position in allPositions)
        {
            if (!selectedPositions.Contains(position))
            {
                tilemap.SetTile(position, null); 
            }
        }
    }


    //Establecer niveles coa velocidade e tiles
    public void SetDifficultyEasy()
    {
        difficultyLevel = DifficultyLevel.Easy;
        piece.stepDelay = 1;
        RandomizeTilesOnDifficulty();
        game.StartGame();
    }

    public void SetDifficultyMedium()
    {
        difficultyLevel = DifficultyLevel.Medium;
        piece.stepDelay = 0.5f;
        RandomizeTilesOnDifficulty();
        game.StartGame();
    }

    public void SetDifficultyHard()
    {
        difficultyLevel = DifficultyLevel.Hard;
        piece.stepDelay = 0.3f;
        RandomizeTilesOnDifficulty();
        game.StartGame();
    }

}
