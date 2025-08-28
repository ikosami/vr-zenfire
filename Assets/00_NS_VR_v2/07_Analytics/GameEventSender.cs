using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;

public class GameEventSender : MonoBehaviour
{
    public static GameEventSender Instance;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LevelComplete(int level)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Level " + level.ToString("000"), "Level_Progress");
    }

    public void RateGame()
    {
        GameAnalytics.NewDesignEvent("RateGame");
    }
    
}
