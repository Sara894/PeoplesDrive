using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ExitIntro : MonoBehaviour
{
    private void OnEnable()
    {
        //Only specify the sceneName or sceneBuildIntex will load the scene with the single mode
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
