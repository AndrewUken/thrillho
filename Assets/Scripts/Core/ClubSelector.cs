using UnityEngine;
using System.Collections;

public class ClubSelector : MonoBehaviour {

  [SerializeField]
  private GameObject dialogue;

  [SerializeField]
  private GameObject clubSelectUi;

  [SerializeField]
  private GameObject[] clubSprites;

  bool acceptingInput = false;
  private int clubIndex = 300;

  public float clubSelectTimeout = 0.24f;

	// Use this for initialization
	void Start () {
    Game.Initialize();
    acceptingInput = false;

    clubSelectUi.SetActive(false);


    dialogue.SetActive(true);


    UkenTimer.SetTimeout(3f, () => {
      AudioManager.Instance.PlaySound("EpisodeVox/IAmCarvallo");
      UkenTimer.SetTimeout(2f, () => {
        AudioManager.Instance.PlaySound("EpisodeVox/ChooseAClub");
        UkenTimer.SetTimeout(2f, () => {
          acceptingInput = true;
          clubSelectUi.SetActive(true);
        });
      });
    });
	}
	
	// Update is called once per frame
	void Update () {
    for(int i = 0; i < clubSprites.Length; i++) {
      clubSprites[i].SetActive(ValidIndex == i);
    }
    if(acceptingInput) {
      if(Input.GetAxis(InputAxes.HORIZONTAL) < 0f) {
        Left();
        acceptingInput = false;
        UkenTimer.SetTimeout(clubSelectTimeout, () => {
          acceptingInput = true;
        });
      } else if(Input.GetAxis(InputAxes.HORIZONTAL) > 0f) {
        Right();
        acceptingInput = false;
        UkenTimer.SetTimeout(clubSelectTimeout, () => {
          acceptingInput = true;
        });
      } else if(Input.anyKeyDown) {
        SelectClub();
        acceptingInput = false;
      }
    }
	}

  private int ValidIndex {
    get {
      return clubIndex % clubSprites.Length;
    }
  }

  public void Left() {
    clubIndex -= 1;
    AudioManager.Instance.PlaySound("UI/Scroll");
  }

  public void Right() {
    clubIndex += 1;
    AudioManager.Instance.PlaySound("UI/Scroll");
  }
  
  private void SelectClub() {
    AudioManager.Instance.PlayClip(GameConfig.Instance.clubSelectSounds[ValidIndex]);

    Game.ClubPrefab = GameConfig.Instance.clubPrefabs[ValidIndex];

    UkenTimer.SetTimeout(4f, () => {
      Game.LoadScene(Game.Scenes.Game);
    });
  }
}
