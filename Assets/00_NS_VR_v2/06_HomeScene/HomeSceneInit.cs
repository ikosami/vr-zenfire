using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeSceneInit : MonoBehaviour
{
    [SerializeField] HomeInitData _homeInitData;
    [SerializeField] SpriteRenderer _appLogoRenderer;
    [SerializeField] Image _appThumbnail;
    [SerializeField] SpriteRenderer _ratingGiftImageRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _appLogoRenderer.sprite = _homeInitData.AppLogo;
        _appThumbnail.sprite = _homeInitData.Thumbnail;
        _ratingGiftImageRenderer.sprite = _homeInitData.RatingGiftImage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
