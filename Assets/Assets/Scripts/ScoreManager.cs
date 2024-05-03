using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


    /*
    Clase para gardar os datos do xogador con esta información: 
    - Nome do xogador
    - Puntuación
    - Tempo
    */
public class ScoreManager : MonoBehaviour
{
    [System.Serializable]
    public class ScoreData
    {
        public string playerName;
        public int score;
        public float time;
    }

    // Instanciamos a lista de puntuacións
    [System.Serializable]
    public class ScoreDataList
    {
        public List<ScoreData> scores;
    }

    public GameObject scoreEntryPrefab; // Prefab de score
    public Transform content; // Posición
    public string fileName = "scoreData.json"; // Arquivo de almacenamento
    public int maxEntriesToShow = 10; // Número máximo de entradas para mostrar

/*
Ao iniciar a clase: 
- Lemos o documento json
- E se o documento existe: 
    - Cargamos os datos
    - Ordeamos os datos por puntuación
    - Cargamos os 10 primeiros
    - E instáncianse en no game object (que é un panel vertical configurado en unity)
    - Cargamos as informacións e asociámolas ao game object
*/
    void Start()
    {
        // Lemos o documento
        string json = LoadJsonFromFile(fileName); // Cargar el JSON desde el archivo
        if (json != null)
        {
            ScoreDataList scoreDataList = JsonUtility.FromJson<ScoreDataList>(json);

            // Ordeamos os datos
            scoreDataList.scores.Sort((x, y) => y.score.CompareTo(x.score));

            // Limitamos as puntuacións a 10
            int numEntriesToShow = Mathf.Min(maxEntriesToShow, scoreDataList.scores.Count);

            // Instanciamos o prefab onde se van ver 
            for (int i = 0; i < numEntriesToShow; i++)
            {
                ScoreData scoreData = scoreDataList.scores[i];
                string timeFormatted = FormatTime(scoreData.time);
                string scoreEntry = $"{scoreData.playerName}: {scoreData.score} - Time: {timeFormatted}";

                // Créase un novo obxecto co material
                GameObject newEntry = Instantiate(scoreEntryPrefab, content);

                // Configurar a información que queremos ver
                Text[] texts = newEntry.GetComponentsInChildren<Text>();
                texts[0].text = scoreData.playerName;
                texts[1].text = scoreData.score.ToString();
                texts[2].text = timeFormatted;

                // E esta nova información é filla do contido orixinal
                newEntry.transform.SetParent(content, false);
            }
        }
        else
        {
            Debug.LogError("Failed to load JSON data.");
        }
    }
    /*
    Carga o arquivo json
    */
    private string LoadJsonFromFile(string fileName)
    {
        string filePath = fileName;
        if (File.Exists(filePath))
        {
            return File.ReadAllText(filePath);
        }
        else
        {
            Debug.LogError($"File not found: {filePath}");
            return null;
        }
    }

    /*
    Formateo do tempo
    */
    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        int remainingSeconds = Mathf.FloorToInt(seconds % 60);
        return string.Format("{0:00}:{1:00}", minutes, remainingSeconds);
    }
}
