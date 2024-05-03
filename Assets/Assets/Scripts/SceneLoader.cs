using System.Net.Mime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    // CARGA DE ESCENAS PRINCIPAIS
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("01_Intro");
    }

    public void LoadTetrisScene()
    {
        SceneManager.LoadScene("03_Tetris Classic");
    }
    public void LoadBlockTetrisScene()
    {
        SceneManager.LoadScene("04_Tetris_Block");
    }

    public void LoadScores()
    {
        SceneManager.LoadScene("03_Scores");
    }
    public void LoadInstructions()
    {
        SceneManager.LoadScene("02_Instruccions");
    }

    public void LoadWorldMap()
    {
        SceneManager.LoadScene("03_World");
    }

    
    // CARGA NIVEIS

    public void LoadRusia()
    {
        SceneManager.LoadScene("04_Tetris_Rusia");
    }

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


    //SAÍR DA APLICACIÓN
    public void Exit()
    {
        Application.Quit();    
    }   

}