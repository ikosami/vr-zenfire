using UnityEngine;

[CreateAssetMenu(fileName = "OculusStoreAppData", menuName = "NS VR/Store/Oculus Store App Data")]
public class OculusStoreAppData : ScriptableObject
{
    [System.Serializable]
    public class StoreAppInfo
    {
        public string key;
        public string appId;
        public string shortLinkURL;
        public Sprite thumbnail;
        public bool hasShortLink => !string.IsNullOrEmpty(shortLinkURL);
    }

    [SerializeField]
    private StoreAppInfo[] storeApps;

    [SerializeField] string _discordLinkURL;

    public string DiscordLinkURL => _discordLinkURL;

    /// <summary>
    /// キーからアプリ情報を取得
    /// </summary>
    public StoreAppInfo GetAppInfo(string key)
    {
        if (storeApps == null) return null;
        
        foreach (var app in storeApps)
        {
            if (app.key == key)
            {
                return app;
            }
        }
        
        Debug.LogWarning($"App with key '{key}' not found in OculusStoreAppData");
        return null;
    }
} 
