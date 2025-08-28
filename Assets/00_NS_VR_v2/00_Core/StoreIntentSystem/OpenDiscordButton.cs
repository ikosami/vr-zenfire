using UnityEngine;

public class OpenDiscordButton : MonoBehaviour
{
    public void OpenDiscord()
    {
        var link = References.Instance.OculusStoreAppData.DiscordLinkURL;
        Application.OpenURL(link);
    }
}
