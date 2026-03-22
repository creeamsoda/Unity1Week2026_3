using TMPro;
using UnityEngine;

public class DebugScore : MonoBehaviour
{
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] TextMeshProUGUI scoreText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "Score: " + scoreManager.GetScore();
    }
}
