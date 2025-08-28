using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandbagManager : MonoBehaviour
{
    [SerializeField] RagdollOnCollision[] _sandbags;
    [SerializeField] Transform _spawnPoint;

    RagdollOnCollision _current;

    public RagdollOnCollision Current => _current;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spawn(System.Action<Transform> onRagdoll) {
        if(_current != null) {
            Destroy(_current.gameObject);
        }
        _current = Instantiate(_sandbags[Random.Range(0, _sandbags.Length)], _spawnPoint.position, _spawnPoint.rotation);
        _current.OnRagdoll += (_) => onRagdoll?.Invoke(_);
    }
}
