#if UNITY_ANDROID
using UnityEngine;

namespace NS_VR.Core.Utils
{
    public class OculusStoreOpener
    {

        public static void OpenOculusStore(string appID)
        {
            if (string.IsNullOrEmpty(appID)) {
                Debug.LogError("アプリケーションIDが見つかりません。");
                return;
            }
#if UNITY_EDITOR
            Debug.Log("エディタではストアを開けません。実機でお試しください。");
#else
            try
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");
                AndroidJavaObject i = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", "com.oculus.vrshell");
                i.Call<AndroidJavaObject>("setClassName", "com.oculus.vrshell", "com.oculus.vrshell.MainActivity");
                i.Call<AndroidJavaObject>("setAction", "android.intent.action.VIEW");
                i.Call<AndroidJavaObject>("putExtra", "uri", "/item/" + appID);
                i.Call<AndroidJavaObject>("putExtra", "intent_data", "systemux://store");
                currentActivity.Call("startActivity", i);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Oculusストアを開く際にエラーが発生しました: " + e.Message);
            }
#endif
        }

        public static void OpenOculusStoreByURL(string appURL)
        {
            if (string.IsNullOrEmpty(appURL)) {
                Debug.LogError("アプリケーションURLが見つかりません。");
                return;
            }

            Application.OpenURL(appURL);
        }
    }
}
#endif 