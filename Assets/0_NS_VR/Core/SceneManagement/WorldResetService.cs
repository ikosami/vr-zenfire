using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldResetService : MonoBehaviour
{
    [SerializeField] private VRSceneTransitionManager sceneTransitionManager;
    public void Reset() {
        Invoke("ResetScene", 0.2f);
    }

    void ResetScene() {
        sceneTransitionManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
