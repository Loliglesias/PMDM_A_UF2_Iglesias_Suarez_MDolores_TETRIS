using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    [SerializeField] Text txtMessage; // Campo para asociar unha mensaxe
    [SerializeField] Text txtScore; // Campo para asociar a puntuación
    [SerializeField] Text txtLevel; // Campo para asociar o nivel
    [SerializeField] Text linhasEl; // Campo para asociar as liñas eliminadas 
    [SerializeField] InputField nameInputField; // Input field para introducir o nome do xogador
    [SerializeField] public GameObject gameOverPanelRem; // Panel de game over con vidas pendentes
    [SerializeField] public GameObject gameOverPanel; // Panel de game over
    [SerializeField] public GameObject gameVictory; // Panel de victoria
    public AudioSource backgroundMusic; // Controlador de background music
    private bool isMusicPlaying = true; // Boleano de comprobación de se está soando a música
    public Sprite playSprite; // Sprite de imaxe para o botón en play
    public Sprite pauseSprite; // Sprite de imaxe para o botón en pause
    public Image buttonImage; // Asociación da imaxe para o botón
    public Board board; // Instancia da clase board
    private float startTime = 0; // Tempo de inicio 
    public GameObject instructionsPanel; // Panel de instruccións 
    [SerializeField] public GameObject GUI; // Panel de textos 
    public int lives; // Vidas
    public TimeDisplay timeDisplay; // Instancia da clase TimeDisplay
    [SerializeField] GameObject[] LivesPrefabs; // Prefabs de vidas 
    public bool isGameOver(){return gameOver;}
    [SerializeField] public int targetScore; // Obxectivo de puntuación 
    [SerializeField] public float targetTime; // Obxectivo de tempo
    [SerializeField] public int targetLines; // Número liñas a eliminar
    [SerializeField] bool scoreObjectiveEnabled = true; // Boleando para verificar se está marcado un obxectivo de puntuación
    [SerializeField] bool timeObjectiveEnabled = true; // Boleando para verificar se está marcado un obxectivo de tempo
    [SerializeField] bool linesObjectiveEnabled = true; // Boleando para verificar se está marcado un obxectivo de liñas
    private bool gameOver = false; // Verificación de game over
    public bool victory = false; // Verificación de victoria
    [SerializeField] PlayerData playerData; // Instanciación da clase Player data para gardar as vidas dun nivel a outro

    // Enumeración dos dous tipos de xogo que hai: instruccións e xogo
    private enum GameState
    {
        Instructions,
        Playing
    }

    // Por defecto arrancamos con Instruccións
    private GameState gameState = GameState.Instructions;


    /*
    Arrancamos con: 
    - Reseteando a información do xogador de vidas
    - Se hai, cargamos a información das vidas
    - E asociámolas á nosa variable local
    - Reseteamos o tempo
    - Desactivamos o campo de nome
    - Actualizamos o botón da música
    */
    private void Start()
    {
        playerData.ResetPlayerData();
        playerData.LoadPlayerData();
        lives = playerData.lives;
        startTime = Time.time;
        nameInputField.gameObject.SetActive(false);
        UpdateButtonImage();
    }

    /*
    Para actualizar en todo momento: 
    - Actualizamos: 
        - Puntuación
        - Nivel
        - Vidas
    - Desactivamos todos os prefabs de vidas
    - Cargamos o os das vidas reais
    - Verificamos o estado do xogo dependendo de se estamos en instruccións ou xogo
    - E se xa non estamos en game over e pulsamos S, reiniciamos a partida
    */
    private void Update()
    {
        txtScore.text = string.Format("{0,6:D6}", board.score);
        txtLevel.text = string.Format("{0,3:D3}", board.level);
        linhasEl.text = string.Format("{0,3:D3}", board.linhasEliminadas);
        
        for (int i = 0; i < LivesPrefabs.Length; i++)
        {
            LivesPrefabs[i].SetActive(false);
        }

        for (int i = 0; i < lives; i++)
        {
            LivesPrefabs[i].SetActive(true);
        }

        switch (gameState)
        {
            case GameState.Instructions:
                Instruccions();
                break;
            case GameState.Playing:
                Playing();
                break;
        }

        if (!gameOver && Input.GetKeyDown(KeyCode.S))
        {
            //Debug.Log("Restart game key pressed...");
            RestartGame();
        }

    }

    /*
    Se estamos xogando, verificamos se estamos en game over ou se gañamos a partida
    */
    public void Playing()
    {   
        if (!gameOver && board.gameOver)
        {
            gameOver = true;
            GameOver();
        }
        else if (!gameOver)
        {
            StartNextLevel();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("03_World");
        }
    }

    /*
    Se estamos en instruccións: 
    - Desactivamos o panel de información de xogador
    - Desactivamos o panel de victoria
    - Desactivamos os paneis de game over
    - Pausamos o xogo
    - E se pulsamos o botón, comeza o xogo
    */
    public void Instruccions()
    {
        GUI.SetActive(false);
        gameVictory.SetActive(false);
        gameOverPanelRem.SetActive(false);
        gameOverPanel.SetActive(false);
        Time.timeScale = 0f; 
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }
    /*
    Manexamos o game over: 
    - Primeiro situamos xa o xogo fora de game over
    - Se temos vidas restantes:
        - Activamos o panel de game over con vidas restantes
        - Restamos unha vida
        - Restauramos o tempo
        - Se o xogador pulsa S, reinicia a partida
    - Se non temos vidas activamos o panel de game over normal para poder volver ao menú principal ou reiniciar o mapa
    */
    public void GameOver()
    {
        // Debug.Log("Estado xogo: Game Over");
        gameOver = false;
        board.gameOver = false;

        if (lives > 0)
        {
            gameOverPanelRem.SetActive(true);
            lives--;
            playerData.lives--;
           // Debug.Log("Game Over: Lives Remaining = " + lives);

            timeDisplay.RestartTime();

            if (Input.GetKeyDown(KeyCode.S))
            {
                RestartGame();
            }
        }
        else
        {
           // Debug.Log("Game Over: No Lives Remaining");
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    /*
    Reinicia o xogo: 
    - Game over é falso
    - Desactivamos os paneles de game over
    - Desactivamos o panel de victoria
    - Reiniciamos o xogo na clase Board
    - Reiniciamos o tempo
    - Activamos os campos de texto
    - Cambiamos o estado de xogo a Xogando
    - Reactivamos o tempo de xogo
    - Desactivamos o campo de nome
    */
    public void RestartGame()
    {
        gameOver = false;
        gameOverPanel.SetActive(false);
        gameOverPanelRem.SetActive(false);
        gameVictory.SetActive(false);
        board.RestartGame();
        timeDisplay.RestartTime();
        GUI.SetActive(true);
        gameState = GameState.Playing;
        Time.timeScale = 1f;
        nameInputField.gameObject.SetActive(false);
    }

    /*
    Iniciamos o xogo: 
    - Desactivamos o panel de instruccións
    - Desactivamos os paneles de game over
    - Desactivamos o panel de victoria
    - Activamos o panel de campos de texto
    - Reiniciamos o tempo de xogo
    - Pasamos o estado do xogo a Xogando
    */
    public void StartGame()
    {
        // Debug.Log("Iniciamos xogo");
        instructionsPanel.SetActive(false);
        gameOverPanelRem.SetActive(false);
        gameOverPanel.SetActive(false);
        gameVictory.SetActive(false);
        GUI.SetActive(true);
        Time.timeScale = 1f;
        
        gameState = GameState.Playing;
    }

    /*
    Método para activar e desactivar a música
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
    Actualizamos a imaxe do botón
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
    Cargamos a escena de menú principal e a principal do mundo
    */
    public void LoadIntroScene()
    {
        SceneManager.LoadScene("01_Intro");
    }

    public void LoadReto()
    {
        SceneManager.LoadScene("03_World");
    }

    /*
    Verificación de se chegou ao obxectivo de puntuación
    */
    bool PlayerScoreReachedTarget()
    {
        return board.score >= targetScore;
    }

    /*
    Verificación de se chegou ao obxectivo de liñas
    */
    bool PlayerOutOfLines()
    {
        return board.linhasEliminadas >= targetLines;
    }

    /*
    Verificación de se chegou rematou o tempo
    */
    bool TimeRanOut()
    {
        if (timeObjectiveEnabled && targetTime > 0)
        {
            return Time.time - startTime >= targetTime;
        }
        else
        {
            return false; 
        }
    }

    /*
    Manexo do comezo de outro nivel: 
    - Primeiro verificamos se estamos en Game over, se estamos, non se executa o método
    - Iniciamos o boleano de que completamos o nivel a true. E arrancamos os condicionales: 
        - Verificamos se temos activado o obxectivo de puntuación e se non o conseguimos: nivel non completado
        - Verificamos se temos activado o obxectivo de tempo e se non o conseguimos: nivel non completado
        - Verificamos se temos activado o obxectivo de liñas e se non o conseguimos: nivel non completado
    - Combinamos as opcións de obxectivos de liñas ou puntuación coa conta atrás de tempo
    - Verificamos se non cumplimos os obxectivos e nos quedamos sen tempo e pasamos a game over
    - Manexamos a Victoria con: 
        - Establecemos a variable victoria a true
        - Chamamos ao método victoria de board
        - Gardamos as vidas
        - Paramos o xogo
        - Desactivamos o panel de información de xogo
    */
    void StartNextLevel()
    {
        // Game over
        if (gameOver)
        {
            return;
        }

        // Boleanos de verificación
        bool levelCompleted = true;

        if (scoreObjectiveEnabled && !PlayerScoreReachedTarget())
        {
            levelCompleted = false;
        }

        if (timeObjectiveEnabled && !TimeRanOut())
        {
            levelCompleted = false;
        }

        if (linesObjectiveEnabled && !PlayerOutOfLines())
        {
            levelCompleted = false;
        }

        // Obxectivo de liñas cumplido antes de acabar o tempo: 

        if (linesObjectiveEnabled && PlayerOutOfLines() && !TimeRanOut())
        {
            levelCompleted = true;
        }

        // Obxectivo de score cumplido antes de acabar o tempo: 

        if (scoreObjectiveEnabled && PlayerScoreReachedTarget() && !TimeRanOut())
        {
            levelCompleted = true;
        }

        // Obxectivo de liñas e score cumplido antes de acabar o tempo: 

        if (linesObjectiveEnabled && PlayerOutOfLines() && scoreObjectiveEnabled && PlayerScoreReachedTarget() && !TimeRanOut())
        {
            levelCompleted = true;
        }

        // Se non se cumpliron os obxectivos e se acabou o tempo
        if (!levelCompleted && TimeRanOut())
        {
            GameOver();
            gameOver = true;
            return;
        }

        // Se si que cumplimos o obxectivo
        if (levelCompleted)
        {
            victory = true;
            board.Victoria();
            playerData.SavePlayerData();
            gameVictory.SetActive(true); 
            Time.timeScale = 0f;
            GUI.SetActive(false);
        }
    }

    // CARGA NIVEIS

    public void LoadEEUU()
    {
        SceneManager.LoadScene("05_Tetris_EEUU");
    }

    public void LoadChina()
    {
        SceneManager.LoadScene("06_Tetris_China");
    }

    public void LoadExipto()
    {
        SceneManager.LoadScene("07_Tetris_Exipto");
    }
    public void LoadMexico()
    {
        SceneManager.LoadScene("08_Tetris_Mexico");
    }
    public void LoadFrancia()
    {
        SceneManager.LoadScene("09_Tetris_Francia");
    }

    public void LoadVictoria()
    {
        SceneManager.LoadScene("01A_Cortinilla");
    }

}
