using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] Transform _startPosition;
    [SerializeField] TMP_Text _scoreText;

    [SerializeField] Transform _currentTarget;

    float _score = 0;

    public float Score => _score;

    Coroutine _measureScoreCoroutine;

    const float _endJudgeTime = 1f;
    float _currentJudgeTime = 0f;
    Vector3 _prevPosition;

    Vector3 _startPos;

    public System.Action<float> OnFixed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitScore()
    {
        _scoreText.text = "0";
        _currentJudgeTime = 0;
    }

    public void StartMeasuring(Transform target)
    {
        _currentTarget = target;
        _scoreText.text = "0";
        _startPos = _startPosition.position;
        _measureScoreCoroutine = StartCoroutine(MeasureScore());
    }

    IEnumerator MeasureScore()
    {
        _score = 0;
        _currentJudgeTime = 0;
        _prevPosition = _currentTarget.position;
        while (true)
        {
            if (_currentTarget == null)
            {
                break;
            }
            _score = (_currentTarget.position.z - _startPos.z);
            _scoreText.text = _score.ToString("F1") + "m";
            if(_currentTarget.position.y < -2f) {
                OnFixed?.Invoke(_score);
                break;
            }
            if((_currentTarget.position - _prevPosition).z < 0.01f)
            {
                _currentJudgeTime += Time.deltaTime;
                if(_currentJudgeTime >= _endJudgeTime)
                {
                    OnFixed?.Invoke(_score);
                    break;
                }
            }
            else
            {
                _currentJudgeTime = 0;
                _prevPosition = _currentTarget.position;
            }
            
            yield return null;
        }
    }

    public void EndMeasure()
    {
        _currentTarget = null;
        if (_measureScoreCoroutine != null)
        {
            StopCoroutine(_measureScoreCoroutine);
        }
    }
}
