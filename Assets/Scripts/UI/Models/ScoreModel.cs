namespace Scripts.UI.Models
{
    public class ScoreModel
    {
        private float _score;
        private float _mult;
        private float _total;

        public float GetScore() => _score;
        public float GetMultiplier() => _mult;
        public float GetScoreXMult() => _score * _mult;
        public float GetTotal() => _total;

        public void AddScore(float score, float mult)
        {
            _score += score;
            _mult += mult;
        }

        public void UpdateTotal()
        {
            _total += _score * _mult;
        }

        public void SoftReset()
        {
            _score = 0;
            _mult = 0;
        }

        public void HardReset()
        {
            _score = 0;
            _mult = 0;
            _total = 0;
        }
    }
}

