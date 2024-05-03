using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class PlayerData : MonoBehaviour
{
    private const string SAVE_FILE = "playerData.json";
    public int lives = 3; // Número inicial de vidas

    // Método para gardar os datos do xogador
    public void SavePlayerData()
    {
        PlayerSaveData data = new PlayerSaveData();
        data.lives = lives;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(SAVE_FILE, json);
    }

    // Iniciar as vidas a 3 se non hai info
    public void SavePlayerDataCero()
    {
        PlayerSaveData data = new PlayerSaveData();
        data.lives = 3;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(SAVE_FILE, json);
    }

    // Método para cargar os datos gardados
    public void LoadPlayerData()
    {
        if (File.Exists(SAVE_FILE))
        {
            string json = File.ReadAllText(SAVE_FILE);
            PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);
            lives = data.lives;
        }

        else 
        {
            SavePlayerDataCero();
        }
    }

    // Se arrancamos o nivel de Rusia, reseteamos as vidas
    public void ResetPlayerData()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "04_Tetris_Rusia")
        {            
            if (File.Exists(SAVE_FILE))
            {
                File.Delete(SAVE_FILE);
            }
            else
            {
                SavePlayerDataCero();
                Debug.Log("Save file not found: " + SAVE_FILE);
            }

            lives = 3;
        }
    }

}

[System.Serializable]
public class PlayerSaveData
{
    public int lives;
}
