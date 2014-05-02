using UnityEngine;

public class GameConfig : ScriptableObject {
  public GameConfig() {
  }

  public int maxLives = 8;
  public bool autoRun = true;

  [Range(0, 4f)]
  public float cameraDelayOnDeath = 2f;

  [SerializeField]
  public GameObject[] clubPrefabs;

  [SerializeField]
  public AudioClip[] clubSelectSounds;

  public bool followCameraY = true;

  private static GameConfig _instance = null;
  public static GameConfig Instance {
    get {
      if(_instance == null) {
        _instance = (GameConfig)Resources.Load("ScriptableObjects/GameConfig");
      }
      return _instance;
    }
  }
}

