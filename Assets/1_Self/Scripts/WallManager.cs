using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{
    [SerializeField] GameObject[] _walls;
    [SerializeField] int _sameCount;
    [SerializeField] int _totalCount;
    [SerializeField] Transform _startPosition;
    [SerializeField] float _span = 4f;
    [SerializeField] GameObject[] _symbols;

    List<GameObject> _currentWalls = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnWalls() {
        foreach(var wall in _currentWalls) {
            Destroy(wall);
        }

        int sameCount = _sameCount;
        int wallIndex = 0;
        float borderVelocity = 1f;
        for(int i = 0; i < _totalCount; i++) {
            var wall = Instantiate(_walls[wallIndex % _walls.Length]);
            wall.transform.position = _startPosition.position + Vector3.forward * i * _span;
            wall.SetActive(true);
            _currentWalls.Add(wall);
            sameCount--;
            borderVelocity += 4f;
            if(sameCount == 0) {
                sameCount = _sameCount;
                wallIndex++;
            }
        }

        for (int i = 0; i < _symbols.Length; i++) {
            var symbol = Instantiate(_symbols[i]);
            symbol.SetActive(true);
            _currentWalls.Add(symbol);
        }
    }
}
