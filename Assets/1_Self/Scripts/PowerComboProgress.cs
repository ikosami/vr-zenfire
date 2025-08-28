using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerComboProgress : MonoBehaviour
{
    public static PowerComboProgress Instance { get; private set; }
    [SerializeField] TMP_Text _text;
    [SerializeField] Image _image;
    float _maxCombo = 100;
    
    public float Combo { get; private set; } = 0;

    public float MaxCombo => _maxCombo;
    public float Progress => Combo / _maxCombo;
    // Start is called before the first frame update
    void Start()
    {
        if(Instance == null) {
            Instance = this;
        }else {
            Destroy(gameObject);
        }
    }
    public void Init(RagdollOnCollision ragdollOnCollision) {
        Combo = 0;
        _image.fillAmount = 0;
        _text.text = Progress.ToString("P0");
        ragdollOnCollision.OnHit += AddCombo;
    }

    public void AddCombo(Collision collision) {
        Combo += Mathf.Min(collision.relativeVelocity.magnitude * 0.5f, 2f);
        _image.fillAmount = Progress;
        _text.text = Progress.ToString("P0");

    }
}
