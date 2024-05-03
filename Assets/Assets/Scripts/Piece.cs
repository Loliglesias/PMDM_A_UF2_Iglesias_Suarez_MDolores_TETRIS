using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; } // Instanciación de Board
    public TetrominoData data { get; private set; } // Instanciación dos tetrominos
    public Vector3Int[] cells { get; private set; } // Instanciación das celdas do tilemap 
    public Vector3Int position { get; private set; } // Posición
    public int rotationIndex { get; private set; } // Índice de rotación
    public float stepDelay; // Velocidade de caída da pieza
    public float moveDelay; // Velocidade de movemento lateral
    public float lockDelay; // Tempo antes de que se bloquee a peza no taboleiro
    private float stepTime; // Cálculos de movemento das pezas
    private float moveTime; // Cálculo de movemento das pezas 
    private float lockTime; // Cálculo de movemento das pezas
    [SerializeField] AudioClip sfxPiece; // SFX de posición da peza


    /*
    Inicialización da clase cun Board, posición e tetromino como parámetros:
    - Marcamos a velocidade da peza
    - Marcamos as velocidades de movemento e bloqueo
    - E instanciamos a lonxitude das celdas
    */
    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        SetStepDelay(stepDelay);
        this.data = data;
        this.board = board;
        this.position = position;

        rotationIndex = 0;
        stepTime = Time.time + stepDelay;
        moveTime = Time.time + moveDelay;
        lockTime = 0f;

        if (cells == null) {
            cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < cells.Length; i++) {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }

    /*
    Método para incrementar a velocidade pouco a pouco
    */
    public void IncreaseSpeed(float increaseAmount)
    {
        float newStepDelay = stepDelay - increaseAmount * 1.1f;

        newStepDelay = Mathf.Max(newStepDelay, board.minStepDelay);

        stepDelay = newStepDelay;
       // Debug.Log("Velocidad actual: " + stepDelay);
    }

    /*
    Constructor da peza cun Board asociado como parámetro
    */
    public Piece(Board board)
    {
        this.board = board;
    }

    /*
    Update: 
    - Limpamos o taboleiro
    - Marcamos o tempo de bloqueo
    - Manexamos a rotación
    - Manexamos o hard drop
    - Permitimos ao xogador ter certa marxe de movemento cando coloca a peza
    - Avance das pezas cada X segundos
    - Colocamos o Board
    */
    private void Update()
    {
        board.Clear(this);

        // O xogador pode facer cambios antes de que se bloquee a peza
        lockTime += Time.deltaTime;

        // Manexo rotación
        if (Input.GetKeyDown(KeyCode.Q)) {
            Rotate(-1);
        } else if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.UpArrow)) {
            Rotate(1);
        }

        // Manexo hard drop
        if (Input.GetKeyDown(KeyCode.Space)) {
            HardDrop();
        }

        // Podemos mover as pezas sen que se mova demasiado rápido
        if (Time.time > moveTime) {
            HandleMoveInputs();
        }

        // Avance das pezas cada X segundos
        if (Time.time > stepTime) {
            Step();
        }

        board.Set(this);
    }

    /*
    Setter para marcar a velocidade de caida das pezas
    */
    public void SetStepDelay(float stepDelay)
    {
        this.stepDelay = stepDelay;
    }

    /*
    Manexo das baixadas das pezas con S ou a frecha cara abaixo
    Manexo daos movementos laterais das pezas coas frechas ou as teclas A/D
    */
    private void HandleMoveInputs()
    {
        // Baixada
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            if (Move(Vector2Int.down)) {
                stepTime = Time.time + stepDelay;
            }
        }

        // Esquerda/dereita
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            Move(Vector2Int.left);
        } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            Move(Vector2Int.right);
        }
    }

    /*
    Manexo de posicións das pezas ao baixar con respecto á velocidade. Se a peza está inactiva moito tempo, bloquéase
    */
    private void Step()
    {
        stepTime = Time.time + stepDelay;

        Move(Vector2Int.down);

        if (lockTime >= lockDelay) {
            Lock();
        }
    }

    /*
    Baixada de repente da peza pulsando barra espaciadora. 
    Bloqueáse automáticamente
    */
    private void HardDrop()
    {
        while (Move(Vector2Int.down)) {
            continue;
        }

        Lock();
    }

/*
Método de bloqueo da peza: 
- Establécea no taboleiro
- Chama ao método de limpeza de liñas se é necesario
- Invoca unha nova peza
- Reproduce o son de colocación de pezas
*/
    private void Lock()
    {
        board.Set(this);
        board.ClearLines();
        board.SpawnPiece();

        if(!board.lineFull)
        {
            AudioSource.PlayClipAtPoint(sfxPiece, Camera.main.transform.position,1);

        }
    }

    /*
    Mover as pezas polo taboleiro verificando as posicións correctas
    */
    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = board.IsValidPosition(this, newPosition);

        // Só se move se a posición é válida
        if (valid)
        {
            position = newPosition;
            moveTime = Time.time + moveDelay;
            lockTime = 0f;
        }

        return valid;
    }

    /*
    Rotación das pezas
    */
    private void Rotate(int direction)
    {
        // Gardamos a rotación actual en caso de que a rotación falle e teñamos que restaurala
        int originalRotation = rotationIndex;

        // Rotamos as celdas usando o matrix de rotación
        rotationIndex = Wrap(rotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);

        //Se a rotación falla, volvemos atrás
        if (!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    /*
    Matriz de rotación e que ten en conta se é a peza I, O ou as demais
    */
    private void ApplyRotationMatrix(int direction)
    {
        float[] matrix = Data.RotationMatrix;

        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cell = cells[i];

            int x, y;

            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;
            }

            cells[i] = new Vector3Int(x, y, 0);
        }
    }

    /*
    Revisión de colisión cos bordes do taboleiro para o movemento e rotación das pezas
    */
    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = data.wallKicks[wallKickIndex, i];

            if (Move(translation)) {
                return true;
            }
        }

        return false;
    }

    /*
    Instanciación dos muros do taboleiro
    */
    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0) {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0));
    }

    /*
    Pechamos a rotación con mínimos e máximos
    */
    private int Wrap(int input, int min, int max)
    {
        if (input < min) {
            return max - (min - input) % (max - min);
        } else {
            return min + (input - min) % (max - min);
        }
    }

}