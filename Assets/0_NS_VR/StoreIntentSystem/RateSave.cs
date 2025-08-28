using UnityEngine;

namespace NS_VR.StoreIntentSystem{
    public class Rating
    {
        public static int Value {
            get {
                return PlayerPrefs.GetInt("Rating", 0);
            }

            set {
                PlayerPrefs.SetInt("Rating", value);
            }
        }
    }
}