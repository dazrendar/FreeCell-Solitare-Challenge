using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class PlayerScores : MonoBehaviour
{
    private string path;

    private string jsonString;
    
    // Start is called before the first frame update
    void Start()
    {
        path = Application.streamingAssetsPath + "/player_scores.json";
        jsonString = File.ReadAllText(path);
        HighScores hs = JsonUtility.FromJson<HighScores>(jsonString);
        Debug.Log(hs.PlayerName);
        transform.GetComponent<Text>().text = "**High Score**\nName: " + hs.PlayerName + "\nScore: " + hs.Score;
    }
}
[System.Serializable]
public class HighScores
{
    public string PlayerName;
    public string Score;
}
