using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class GameManagerBlock : MonoBehaviour
{
    [SerializeField] Text txtMessage; // Campo para asociar unha mensaxe
    [SerializeField] Text txtScore; // Campo para asociar a puntuación
    [SerializeField] Text txtLevel; // Campo para asociar o nivel
    [SerializeField] InputField nameInputField; // Input field para introducir o nome do xogador
    [SerializeField] public GameObject gameOverPanel; // Panel de Game Over
    [SerializeField] public GameObject victoryPanel; // Panel de Victoria
    [SerializeField] public GameObject levelPanel; // Panel de nivel
    [SerializeField] public GameObject GUI; // Panel de textos 
    [SerializeField] public GameObject EnvioNome; // Panel de game over para o envío do nome
    [SerializeField] public GameObject ReinicioSair; // Panel de opcións de reiniciar ou sair
    public AudioSource backgroundMusic; // Controlador de background music
    private bool isMusicPlaying = true; // Boleano de comprobación de se está soando a música
    public Sprite playSprite; // Sprite de imaxe para o botón en play
    public Sprite pauseSprite; // Sprite de imaxe para o botón en pause
    public Image buttonImage; // Asociación da imaxe para o botón
    public BoardBlock board; // Instancia da clase boardblock
    private const string DATA_FILE = "scoreData.json"; // Instancia do documento onde gardamos puntuacións
    private float startTime = 0; // Tempo de inicio 
    private bool isNameInputSubmitted = false; // Verificación de se mandamos o nome
    private bool isGameOverHandled = false; // Verificación de Game Over manexado
    public TimeDisplay timeDisplay; // Instancia da clase TimeDisplay
    public RandomizeTiles randomizeTiles;

    /*
    Ao iniciar a clase: 
    - Establecemos o tempo de inicio
    - Desactivamos os paneles de victoria, envío de nome, reinicio, input do nome
    - Actualizamos a imaxe da música
    - Iniciamos o método de Selección de nivel
    */
    private void Start()
    {
        startTime = Time.time;
        victoryPanel.SetActive(false);
        nameInputField.gameObject.SetActive(false);
        ReinicioSair.gameObject.SetActive(false);
        EnvioNome.gameObject.SetActive(false);
        UpdateButtonImage();
        SelectLevel();
    }

    /*
    Activamos a selección de nivel:
    - Paramos o tempo de xogo
    - Activamos o panel de nivel
    - Desactivamos os controles de texto da partida
    - Desactivamos o panel de game over
    */
    public void SelectLevel()
    {
        Time.timeScale = 0f;
        levelPanel.SetActive(true);
        GUI.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    /*
    Inicio do xogo: 
    - Desactivamos o panel de victoria
    - Iniciamos o tempo
    - Desactivamos o panel de selección de nivel
    - Desactivamos o panel de game over
    - Activamos os controis de texto da partida
    - Reanudamos o tempo de xogo
    */
    public void StartGame()
    {
        victoryPanel.SetActive(false);
        startTime = Time.time;
        levelPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        GUI.SetActive(true);
        Time.timeScale = 1f; 
    }

    /*
    Para a actualización continua do xogo temos: 
    - Engadir os textos formateados de: 
        - Puntuación
        - Nivel
    - Verificamos constantemente que non estamos en Game over. Se estivéramos, imos ao método HandleGameOver
    */
    private void Update()
    {
        txtScore.text = string.Format("{0,6:D6}", board.score);
        txtLevel.text = string.Format("{0,3:D3}", board.level);

        if (board.gameOver && !isGameOverHandled)
        {
            HandleGameOver();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("01_Intro");
        }
    }

    /*
    Manexo do game over: 
    - Paramos o tempo
    - Activamos o panel de game over
    - Activamos a opción de enviar o nome e seleccionamos o input field
    - Unha vez enviamos o nome: 
        - Desactivamos o input field
        - Desactivamos o panel de nome
        - Activamos panel de reinicio e sair
        - Asegurámonos de que o tempo está parado
    */
    public void HandleGameOver()
    {
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);

        nameInputField.gameObject.SetActive(true);
        EnvioNome.gameObject.SetActive(true);
        nameInputField.Select();

        if (isNameInputSubmitted)
        {
            nameInputField.gameObject.SetActive(false);
            EnvioNome.gameObject.SetActive(false);
            ReinicioSair.gameObject.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    /*
    Método para reiniciar xogo:
    - Se o tempo é null, reiníciao
    - Pecha o panel de reinicio e saír
    - Reinicia tempo de novo a 0
    - Activa a velocidade da peza
    - Activa o panel de nivel
    - Desactiva o panel de game over
    - Chama ao método de reiniciar nivel de board
    - Desactiva o input field
    - Desactiva a opción de que o nome do xogador se enviou
    */
    public void RestartGame()
    {

        if (timeDisplay != null)
        {
            timeDisplay.RestartTime();
        }
        ReinicioSair.gameObject.SetActive(false);
        Time.timeScale = 0f;
        startTime = Time.time;
        gameOverPanel.SetActive(false);
        nameInputField.gameObject.SetActive(false);
        isNameInputSubmitted = false;
        board.activePiece.SetStepDelay(1f);
        board.RestartGame();
        SelectLevel();
    }

    /*
    Recargamos a escena para reiniciar xogo
    */
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /*
    Método para cargar o menú principal
    */
    public void LoadIntroScene()
    {
        SceneManager.LoadScene("01_Intro");
    }

    /*
    Método para enviar o nome e gardalo
    */
    public void SubmitName()
    {
        gameOverPanel.SetActive(true);
        string playerName = nameInputField.text;
        if (!string.IsNullOrEmpty(playerName))
        {
            SaveData(playerName, board.score, Time.time);
            isNameInputSubmitted = true;
            // Debug.Log("Nombre enviado: " + playerName);
        }
    }

    /*
    Método para poder activar e desactivar a música durante o xogo
    */
    public void ToggleMusic()
    {
        isMusicPlaying = !isMusicPlaying;

        if (isMusicPlaying)
        {
            backgroundMusic.Play();
        }
        else
        {
            backgroundMusic.Pause();
        }

        UpdateButtonImage();
    }
    
    /*
    Actualizar o botón se está activada ou desactivada
    */
    private void UpdateButtonImage()
    {
        if (isMusicPlaying)
        {
            buttonImage.sprite = playSprite;
        }
        else
        {
            buttonImage.sprite = pauseSprite;
        }
    }

    /*
    Método para gardar a información da partida
    - Pasamos como parámetro: nome, puntuación e tempo
    - Cargamos a info que hai no documento
    - Creamos unha nova instancia dunha nova puntuación
    - Engadímola ás que hai
    - Escribimos o arquivo
    */
    public void SaveData(string playerName, int score, float time)
    {
        GameData.ScoreDataList dataList = new GameData.ScoreDataList();
        if (File.Exists(DATA_FILE))
        {
            string jsonData = File.ReadAllText(DATA_FILE);
            dataList = JsonUtility.FromJson<GameData.ScoreDataList>(jsonData);
        }

        GameData.ScoreData newScore = new GameData.ScoreData();
        newScore.playerName = playerName;
        newScore.score = score;
        newScore.time = time;
        if (dataList.scores == null)
        {
            dataList.scores = new List<GameData.ScoreData>();
        }
        dataList.scores.Add(newScore);

        string json = JsonUtility.ToJson(dataList);
        File.WriteAllText(DATA_FILE, json);
    }

    }
