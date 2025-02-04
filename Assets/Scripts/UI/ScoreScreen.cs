using TMPro;
using UnityEngine;

public class ScoreScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreLabel;
    [SerializeField] private TMP_Text totalLabel;

    private float _score;
    private float _mult;
    private float _total;

    public void AddScore(float score, float mult)
    {
        _score += score;
        _mult += mult;

        scoreLabel.text = $"{_score} x {_mult} = {_score * _mult}";
    }

    public void Finalize()
    {
        float finalScore = _score * _mult;
        _total += finalScore;
        totalLabel.text = _total.ToString();

        _score = 0;
        _mult = 0;
        scoreLabel.text = $"0 x 0";
    }
}
