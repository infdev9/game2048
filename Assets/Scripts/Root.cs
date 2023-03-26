using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Root : MonoBehaviour
{
    public static Root Instance;

    [SerializeField] private Image _panel;
    [SerializeField] private TextMeshProUGUI _gameResult;
    [SerializeField] private TextMeshProUGUI _pointsText;

    public static uint Points { get; private set; }

    public static bool GameStarted { get; private set; }

    private void Awake()
    {
        if (Instance is null)
            Instance = this;
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        _panel.enabled = false;
        _gameResult.text = string.Empty;
        SetPoints(0);
        GameStarted = true;
    
        Field.Instance.GenerateField();
    }

    public void Win()
    {
        GameStarted = false;
        _panel.enabled = true;
        _gameResult.text = "Победа";
    }

    public void Lose()
    {
        GameStarted = false;
        _panel.enabled = true;
        _gameResult.text = "Поражение";
    }

    public void AddPoints(uint points)
    {
        SetPoints(Points + points);
    }

    private void SetPoints(uint points)
    {
          Points = points;
          _pointsText.text = Points.ToString();
    }

}
