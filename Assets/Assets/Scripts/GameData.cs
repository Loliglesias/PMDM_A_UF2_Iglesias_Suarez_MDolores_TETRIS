using System.Collections.Generic;

/*
Clase que nos permite gardar os datos do xogador unha vez perde ou gaña a partida de Tetris Classic ou Block
*/

public class GameData
{
    private const string DATA_FILE = "scoreData.json";

    [System.Serializable]
    public class ScoreData
    {
        public string playerName;
        public int score;
        public float time;
    }
[System.Serializable]
public class ScoreDataList
{
    public List<GameData.ScoreData> scores;
}

}
