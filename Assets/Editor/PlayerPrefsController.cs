using System;
using System.Text;
using UnityEditor;
using UnityEngine;

public class PlayerPrefsController : MonoBehaviour
{

    [MenuItem("NS/セーブデータ削除")]
    static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("NS/AddressableAsset ClearCache")]
    static void CleanPlayerContent()
    {
        Caching.ClearCache();
    }
}
