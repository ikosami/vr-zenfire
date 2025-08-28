using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NS_VR.StoreIntentSystem;

public class RatingGroupPresenter : MonoBehaviour
{
    [SerializeField] RateStarButton _rateStarButton;
    [SerializeField] GameObject _unlockedCheck;
    // Start is called before the first frame update
    void Start()
    {
        var isAlreadyRated = Rating.Value > 0;
        _rateStarButton.ChangeView(isAlreadyRated);
        _unlockedCheck.SetActive(isAlreadyRated);
        _rateStarButton.OnRate += () => {
            var isAlreadyRated = Rating.Value > 0;
            _rateStarButton.ChangeView(isAlreadyRated);
            _unlockedCheck.SetActive(isAlreadyRated);
        };
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
