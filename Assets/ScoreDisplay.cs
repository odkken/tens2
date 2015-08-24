using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    public void UpdateScores(int team1, int team2, int localPlayerTeam)
    {
        if (localPlayerTeam == 1)
        {
            AddUsScore(team1);
            AddThemScore(team2);
        }
        else
        {
            AddUsScore(team2);
            AddThemScore(team1);
        }
    }

    void AddUsScore(int score)
    {
        transform.FindChild("Us").GetComponent<Text>().text += "\n" + score;
    }

    void AddThemScore(int score)
    {
        transform.FindChild("Them").GetComponent<Text>().text += "\n" + score;
    }
}
