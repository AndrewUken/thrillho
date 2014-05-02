using UnityEngine;
using System.Collections;

public class GameplayDirector : MonoBehaviour {

  [SerializeField]
  private Transform[] levels;
  public int curLevelIndex = 0;

  [SerializeField]
  private Transform levelRoot;

  [SerializeField]
  private Transform heroPrefab;

  [SerializeField]
  private Transform heroSpawnPoint;

  [SerializeField]
  public GameObject mainCamera;


  #region UI
  [SerializeField]
  private GameObject gameOverUi;

  [SerializeField]
  ScoreCounter scoreUi;
  #endregion

  private int lives = 0;
  public int Lives {
    get {
      return lives;
    }
    set {
      lives = value;
      if(scoreUi) {
        scoreUi.SetScore(lives);
      }
    }
  }

  private Hero curHero = null;

  // Use this for initialization
  void Start() {
    Initialize();
    LoadLevel(0);
  }

  private void Initialize() {
    curLevelIndex = 0;
    this.gameOverUi.SetActive(false);

    //Just in case the game was launched directly into this scene
    Game.Initialize();

    Lives = GameConfig.Instance.maxLives;

    AudioManager.Instance.PlayMusic();
  }
  
  void Update() {

    if(Input.GetKeyDown("escape")) {
      Game.LoadScene(Game.Scenes.Startup);
    }

    DebugControllsUpdate();
  }

  private void DebugControllsUpdate() {
    if(Feature.enabled(Feature.DEBUG_LEVEL_CONTROLS)) {
      if(Input.GetKeyDown(KeyCode.L)) {
        NextLevel();
      }
    }
  }

  public void NextLevel() {
    curLevelIndex += 1;
    LoadLevel(curLevelIndex);
  }

  public void LoadLevel(int index) {
    Lives = GameConfig.Instance.maxLives;
    if(index > levels.Length - 1) {
      //Last level beaten game over with win
      GameOver(true);
    } else if(index < 0) {
      //Goto start scene
      Game.LoadScene(Game.Scenes.Startup);
    } else {
      //Cleanup levels
      foreach(Transform child in levelRoot) {
        Destroy(child.gameObject); 
      }

      mainCamera.transform.position = Vector3.zero;


      Transform newLevelGo = GameObject.Instantiate(levels[index], levelRoot.transform.position, Quaternion.identity) as Transform;
      newLevelGo.parent = levelRoot;
      newLevelGo.gameObject.name = levels[index].gameObject.name;

      Level newLevel = newLevelGo.GetComponent<Level>();
     
      SpawnHero();
      mainCamera.GetComponent<CameraController>().FollowWaypoints(newLevel);
    }

  }

  private void SpawnHero() {
    Debug.Log("Current Lives: " + Lives);

    Transform newHero = GameObject.Instantiate(heroPrefab, heroSpawnPoint.position + Vector3.back*10, Quaternion.identity) as Transform;
    Hero hero = newHero.GetComponent<Hero>();
    hero.GameplayDirector = this;
    newHero.transform.parent = levelRoot.transform;
    curHero = newHero.GetComponent<Hero>();
    newHero.GetComponent<CarvalloController>().CameraController = this.mainCamera.GetComponent<CameraController>();
    hero.ActivateRespawnInvincibility();
  }

  public void Kill(Hero hero) {
    int curLives = Lives - 1;
    if(curLives < 0) {
      GameOver(false);
    } else {
      //Respawn
      Lives = curLives;

      SpawnHero();
      mainCamera.GetComponent<CameraController>().HaltThenGotoWaypoint();
      AudioManager.Instance.PlaySound("Gameplay/RespawnC");
    }
  }

  /// <summary>
  /// Games the over.
  /// </summary>
  /// <param name="win">If set to <c>true</c> window.</param>
  public void GameOver(bool win) {
    AudioManager.Instance.StopMusic();
    if(win) {
      Debug.Log("YOU A WINNA");
    } else {
      Debug.Log("HAHA");
    }
    this.gameOverUi.SetActive(true);

    if(curHero != null) {
      GameObject.Destroy(curHero.gameObject);
    }
  }


}
