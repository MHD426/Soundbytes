using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void ExitBtn() {
        Application.Quit();
        Debug.Log("Sounbytes application closed");
    }

}
