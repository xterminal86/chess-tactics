using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour 
{
  void Start()
  {
    GameOverseer.Instance.Initialize();

    SceneManager.LoadScene("main");
  }
}
