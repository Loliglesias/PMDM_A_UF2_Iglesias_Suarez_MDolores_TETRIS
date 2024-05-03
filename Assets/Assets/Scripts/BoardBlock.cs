using System.Threading;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BoardBlock : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public PieceBlock activePiece { get; private set; }
    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);

    // Novas funcionalidades
    [SerializeField] LevelManager levelManager; // Instanciar a clase LevelManager
    public GameObject[] piecePrefabs; // Prefabs para a seguinte peza
    public Transform nextPieceSpawnPoint; // Posición de onde estará a seguinte peza
    private int activePieceIndex;// índice que crearemos de xeito aleatorio para instanciar a peza activa
    private int nextPieceIndex;// índice que crearemos de xeito aleatorio para instanciar a seguinte peza
    public GameObject nextPieceInstance; // GameObject para instanciar a peza cada vez que se xere
    public Vector3 nextPieceScale = new Vector3(1f, 1f, 1f); // Tamaño da peza
    public float minStepDelay { get; private set; } = 0.1f;
    public int linhasEliminadas { get; private set; } = 0;// Contaxe das liñas que levamos eliminadas
    public int linhasXuntas = 0; // Contaxe intermedia se eliminamos máis dunha liña ao mesmo tempo
    public int linhasXuntasUp = 0;// Complementario a liñas xuntas pero que se elimina constantemente antes de actualizar a puntuación
    public float speedIncreasePerLine = 0.005f; // Incremento de velocidade por cada liña eliminada
    private const string DATA_FILE = "scoreData.json";// Arquivo onde gardamos os datos de puntuacións do xogador
    public GameManagerBlock game; // Instancia do GameManager
    public int level = 1; // Nivel co que inicializamos o xogo
    [SerializeField] AudioClip sfxClearLine; // SFX limpeza de liña
    [SerializeField] AudioClip sfxGameOver; // SFX limpeza de liña
    [SerializeField] AudioClip sfxLevelUp; // SFX subida de nivel
    [SerializeField] AudioClip sfxVictory; // SFX victoria
    public int score; // Puntuación
    public GameObject gameOverPanel; // Panel de Game Over
    public GameObject gamePausePanel; // Panel de Pausa
    public float startTime; // Tempo de comezo
    public bool gameOver = false;  // Verificador de se estamos en game over
    public bool lineFull = false; // Verificador de se temos a liña chea


    // Método para definir os límites do taboleiro
    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    /*
    Ao arrancar a clase instanciamos:
    - Tilemap
    - Peza activa
    - Instanciamos os tetrominos para ter todas as pezas
    */
    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<PieceBlock>();

        for (int i = 0; i < tetrominoes.Length; i++)
        {
            tetrominoes[i].Initialize();
        }
    }

    /*
    Ao iniciar a clase: 
    - Xeramos a peza de inicio
    - Desactivamos os paneles de game over e pausa
    - Arrancamos o movemento da peza
    */
    private void Start()
    {
        nextPieceIndex = GeneratePieceIndex();

        gameOverPanel.SetActive(false);
        gamePausePanel.SetActive(false);
        SpawnPiece();
    }

    /*
    En actualización continua temos tan só comprobar se o xogador pulsa P para pausar o xogo
    */
    private void Update()
    {
    /*
        if (Input.GetKeyDown(KeyCode.S) && gameOverPanel.activeSelf)
        {
            RestartGame();
        }
    */
        if (Input.GetKeyDown(KeyCode.P))
        {
            Pause();
        }
    }

    /*
    Iniciamos e activamos a peza para xogar con ela
    Iniciamos dúas pezas en paralelo aínda que só podemos xogar con unha: 
    - Activamos a peza de xogo
    - Xeramos o índice da seguinte para poder iniciala no lateral como pista para o xogador da seguinte peza que vai sair
    */
    public void SpawnPiece()
    {
        int activePieceIndex = nextPieceIndex;
        TetrominoData activePieceData = tetrominoes[activePieceIndex];

        activePiece.Initialize(this, spawnPosition, activePieceData);
        
        // Verificamos que a peza está nunha posición válida
        if (IsValidPosition(activePiece, spawnPosition))
        {
            Set(activePiece); // Activamos a peza no taboleiro
        }
        // De non estar, entramos en game over
        else
        {
            GameOver();
            return;
        }
        // Xeramos o índice da seguinte peza para logo xogar con ela e para representala no lateral
        nextPieceIndex = GeneratePieceIndex();

         // Invocamos o método para imprimir no lateral esta peza
        PrintNextPiece();
    }

    /*
    Creamos o método para imprimir a peza seguinte no lateral como pista.
    Pasos: 
    - Limpamos a que tiñamos creada
    - Instanciamos a peza co prefab, na posición indicada
    - E asociámola co empty object do taboleiro
    */
    public void PrintNextPiece()
    {
        ClearNextPiece();

        GameObject nextPiecePrefab = LoadNextPiecePrefab(nextPieceIndex);

        GameObject nextPieceObject = Instantiate(nextPiecePrefab, nextPieceSpawnPoint.position, Quaternion.identity);

        nextPieceObject.transform.localScale = new Vector3(0.70f, 0.70f, 1f);

        nextPieceInstance = nextPieceObject;
    }

    /*
    Xeramos ao azar un índice como int para xerar as pezas que están por saír
    */
    private int GeneratePieceIndex()
    {
        return Random.Range(0, tetrominoes.Length);
    }

    /*
    Xeramos a seguinte peza para a pista
    */
    private GameObject GenerateNextPiece(GameObject nextPiecePrefab)
    {
        GameObject nextPieceObject = Instantiate(nextPiecePrefab, nextPieceSpawnPoint.position, Quaternion.identity);

        nextPieceObject.transform.localScale = nextPieceScale;

        return nextPieceObject;
    }

    /*
    Cargamos a peza tendo en conta o índice creado aleatoriamente
    */
    private GameObject LoadNextPiecePrefab(int nextPieceIndex)
    {
        if (nextPieceIndex >= 0 && nextPieceIndex < piecePrefabs.Length)
        {
            return piecePrefabs[nextPieceIndex];
        }
        else
        {
            Debug.LogError("Índice de pieza fuera de rango: " + nextPieceIndex);
            return null;
        }
    }

    /*
    Limpamos a peza que acabamos de usar para que non se solapen entre elas e se vaian acumulando
    */
    private void ClearNextPiece()
    {
        if (nextPieceInstance != null)
        {
            Destroy(nextPieceInstance);
        }
    }


    /*
    Verificación do Game Over
    Paramos o xogo
    E reproducimos son para notificalo ao xogador
    */
    public void GameOver()
    {
        gameOver = true;
        Time.timeScale = 0f;
        AudioSource.PlayClipAtPoint(sfxGameOver, Camera.main.transform.position, 1);
    }

    /*
    Método para pausar o xogo:
    - Se o xogo está pausado, reanudamos o xogo
    - Se non está pausado, pausamos o xogo
    */
    public void Pause()
    {
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
            gamePausePanel.SetActive(false);
        }
        else
        {
            Time.timeScale = 0f;
            gamePausePanel.SetActive(true);
        }
    }

    /*
    Método para reiniciar o xogo: 
        - Restablecemos a orixe: puntuación, liñas eliminadas, nivel e tempo
        - Restablecemos o game over a false
        - Limpamos todas as liñas do taboleiro para empezar de cero
        - Lanzamos unha nova peza para xogar
    */
    public void RestartGame()
    {
        score = 0;
        linhasEliminadas = 0;
        level = 1;
        // Debug.Log("RestartGame en Board - startTime: " + startTime);
        gameOver = false; // Establece gameOver en false
        startTime = Time.time;
        tilemap.ClearAllTiles();
        SpawnPiece();
    }

    /*
    Colocamos a peza no taboleiro
    */
    public void Set(PieceBlock piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    /*
    Limpamos a peza para que saia a seguinte e non se solapen
    */
    public void Clear(PieceBlock piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }
   
    /*
    Revisamos se a posición da peza é a correcta:
        - Instanciamos os bordes do taboleiro
        - Verificamos:
            - Se hai unha peza ou un tile xa na posición. Se non o hai, devolve true e pode colocarse aí a nosa peza
            - Se intenta colocarse fora dos bordes, non é unha posición válida
            - Se ten outra peza ou tile, non é unha posición válida
    */
    public bool IsValidPosition(PieceBlock piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition)) {
                return false;
            }

            if (tilemap.HasTile(tilePosition)) {
                return false;
            }
        }

        return true;
    }

    /*
    Limpamos liñas. Primeiro verificamos se están completas. Para iso é fundamental usar os bordes
    Ademais, inicializamos a variable que nos suma ou recolle as liñas xuntas eliminadas dunha vez
    */
    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;
        linhasXuntasUp = 0;


        // Limpamos de abaixo a arriba
        while (row < bounds.yMax)
        {
            // Só avanza á seguinte liña se a actual non está limpa porque os tiles caerán cando a liña se limpe
           
            // Se a liña está chea: 
            if (IsLineFull(row)) {
                LineClear(row);  
                linhasEliminadas++;    // Sumamos liñas eliminadas
                linhasXuntasUp++;    // Gardamos na variable as liñas que se eliminaron dunha vez
            // Se a liña non está chea, pasamos á seguinte
            } else {
                row++;
            }
        }

        // Creamos unha parte de código para que se eliminamos unha ou máis liñas, reproducimos son e actualizamos: nivel e puntuación
            if(linhasXuntasUp != 0){
                linhasXuntas = linhasXuntasUp;

                AudioSource.PlayClipAtPoint(sfxClearLine, Camera.main.transform.position,1);

               // Debug.Log("Liñas eliminadas totais " + linhasEliminadas);
               // Debug.Log("Liñas xuntas eliminadas " + linhasXuntas);

                UpdateScore();
                UpdateLevel();
            }

    }

    /*
    Verificación de se a liña está chea.
    Creei un booleano lineFull para poder verificar se está ou non chea fora do método e sen o número de liña como condicionante. 
    */
    public bool IsLineFull(int row)
    {        
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            // Se hai un tile baleiro, non está chea
            if (!tilemap.HasTile(position)) {
                lineFull = false;
                return false;
            }
        }

        lineFull = true;
        return true;
    }

    /*
    Método para limpar todos os tiles dunha liña chea
    E ademais para baixar as liñas que ten por riba que non están cheas
    */
    public void LineClear(int row)
    {
        
        RectInt bounds = Bounds;

        // Limpamos todos os tiles da liña
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

        // Baixamos todo o que ten por riba unha liña (se se dá a opción de ter máis liñas eliminadas, baixaríaas todas)
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }
        // Aproveitamos para aumentar un pouco a velocidade cada vez que eliminamos unha liña

            activePiece.IncreaseSpeed(speedIncreasePerLine);

    }

    /*
    Método para actualizar a puntuación
    Hai varios condicionantes: depende do nivel
    E se hai máis dunha liña eliminada tería premio xa que se multiplica a puntuación deste xeito (punto por nivel * liña) * liña eliminada á vez
    */
    public void UpdateScore()
    {
        int points = 0;
        
        if(activePiece.stepDelay <= 0.2)
        {
            points = ((500 * linhasXuntas))*linhasXuntas;
        }
        if(activePiece.stepDelay < 0.4 && activePiece.stepDelay > 0.2)
        {
            points = ((350 * linhasXuntas))*linhasXuntas;
        }    
        if(activePiece.stepDelay > 0.4 && activePiece.stepDelay < 0.9)
        {
            points = ((200 * linhasXuntas))*linhasXuntas;
        }
        if(activePiece.stepDelay > 0.9)
        {
            points = ((100 * linhasXuntas))*linhasXuntas;
        }

        // Debug.Log("Sumamos: " + points);

        score += points;

        Victoria(); // Verificamos se entramos en modo de victoria

        linhasXuntas = 0; // Volvemos a inicializar a cero, agora que xa actualizamos a puntuación, a variable que contén as liñas eliminadas ao mesmo tempo
    }

    /*
    Método de verificación de Victoria da partida
        - Paramos o tempo do xogo
        - Activamos o panel de victoria
        - Reproducimos son de Victoria
    Para o level manager intentamos facer o mesmo pero a reproducción creaba certos problemas, por eso queda comentado
    */
        public void Victoria()
    {
        if(score >= 999999)
        {
            Time.timeScale = 0f;
            game.victoryPanel.SetActive(true);
            AudioSource.PlayClipAtPoint(sfxVictory, Camera.main.transform.position,1);
        }
/*
        else if(levelManager.victory)
        {
            Time.timeScale = 0f;
            AudioSource.PlayClipAtPoint(sfxVictory, Camera.main.transform.position,1);
        }
*/
        else 
        {
            Time.timeScale = 1f;
        }

    }

    /*
    Cantas máis liñas eliminamos, máis se incrementa o nivel
    Cada 10 liñas, aumenta o nivel
    E se cambiamos de nivel, aumentamos levemente a velocidade e reproducimos un efecto de son
    */
    public void UpdateLevel()
    {
        if (score == 0)
        {
            level = 1;
        }
        else
        {
            int newLevel = linhasEliminadas / 10 + 1;
            if (newLevel > level)
            {
                level = newLevel;
                activePiece.IncreaseSpeed(speedIncreasePerLine); 
                AudioSource.PlayClipAtPoint(sfxLevelUp, Camera.main.transform.position,0.5f);
            }
        }
    }

    /*
    Método para gardar a información da partida
    Obtemos primeiro a información do taboleiro
    Formateámolo
    Creamos unha instancia da clase GameData e asignamos aí os valores
    Convertimos os datos a json
    E escribímolo no documento
    */ 
    public void SavePlayerData(string playerName)
    {
        int score = this.score;
        float time = Time.time;
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);

        GameData.ScoreData data = new GameData.ScoreData();
        data.playerName = playerName;
        data.score = score;
        data.time = float.Parse(minutes.ToString("00") + "." + seconds.ToString("00"));

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(DATA_FILE, json);
    }
}
