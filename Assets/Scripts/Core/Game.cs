using UnityEngine;
using System.Collections;

public static class Game {

  public enum Scenes {
    Startup,
    Game,
    Welcome
  }


  private static bool initialized = false;
  public static void Initialize() {
    //Initialize app level stuff here
    if(initialized == false) {
      initialized = true;

      //Bake AudioManager so there's an audio listener in the scene
      #pragma warning disable 219
      AudioManager bakedAudioManager = AudioManager.Instance;
      if(GameConfig.Instance.clubPrefabs != null && GameConfig.Instance.clubPrefabs.Length > 0) {
        ClubPrefab = GameConfig.Instance.clubPrefabs[0];
      }
    }
  }

  public static GameObject ClubPrefab { set; get; }

  public static void LoadScene(Game.Scenes scene) {
    Application.LoadLevel(scene.ToString().ToLower());
  }

}
