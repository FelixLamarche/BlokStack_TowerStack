using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI scoreText;

    int score;
    public int Score {
        get{return score;}
        private set {
            score = value;
            UpdateScore();
        }
    }

    public bool IsCounting { get; set; }

    void Awake()
    {
        IsCounting = false;
        Score = 0;
    }

    public void ResetScore()
    {
        Score = 0;
        UpdateScore();
    }

    public void AddScore()
    {
        if(!IsCounting)
            return;

        Score++;
        UpdateScore();
    }
    public void SetScore(int newScore)
    {
        if(!IsCounting)
            return;
            
        Score = newScore;
    }

    void UpdateScore()
    {
        scoreText.text = Score.ToString();
    }
}
