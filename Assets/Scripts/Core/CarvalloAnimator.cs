using UnityEngine;
using System.Collections;
using System.Linq;

[RequireComponent (typeof(Hero))]
public class CarvalloAnimator : MonoBehaviour {
  
  public GameObject model; //this is no longer the model

  private Hero hero;
  private Vector3 originalModelScale;

  [SerializeField]
  private GameObject playerModelTemplate;
  private GameObject rig;
  public Animator rigAnimator;

  void Start(){
    hero = GetComponent<Hero>();
    originalModelScale = model.transform.localScale;
    rig = GameObject.Instantiate(playerModelTemplate, model.transform.position, Quaternion.identity) as GameObject;
    rig.transform.parent = model.transform;
    rig.transform.localScale = Vector3.one;
    rigAnimator = rig.GetComponent<Animator>();
    foreach(SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()){
      sr.sortingLayerName = "player";
    }

    GameObject weaponMount = rig.GetComponentsInChildren<WeaponMount>(true).First(wm => wm.hand2 == false).gameObject;
    if(Game.ClubPrefab != null && weaponMount != null){
      GameObject club = GameObject.Instantiate(Game.ClubPrefab, weaponMount.transform.position, Quaternion.identity) as GameObject;
      club.transform.parent = weaponMount.transform;
      club.transform.localScale = Vector3.one;
    }
    
    //Spawn a second weapon for animation, only one is visible at a time
    GameObject weaponMount2 = rig.GetComponentsInChildren<WeaponMount>(true).First(wm => wm.hand2 == true).gameObject;
    if(Game.ClubPrefab != null && weaponMount2 != null) {
      GameObject club = GameObject.Instantiate(Game.ClubPrefab, weaponMount.transform.position, weaponMount.transform.rotation) as GameObject;
      club.transform.parent = weaponMount2.transform;
      club.transform.localScale = Vector3.one;
    }
  }
  
  void Update(){
    float facingX = (hero.direction == Unit.Directions.Right) ? 1 : -1;
    if(model) {
      model.transform.localScale = new Vector3(originalModelScale.x * facingX, originalModelScale.y, originalModelScale.z);
    }
  }

  void LateUpdate(){
    rigAnimator.transform.localPosition = Vector3.zero;
  }

  public void SetVelocity(Vector2 vel) {
    if(rigAnimator) {
      rigAnimator.SetFloat("velocityX", vel.x);
      rigAnimator.SetFloat("velocityY", vel.y);
    }
  }


  public void Attack1(float x=0f, float y=0f) {
    rigAnimator.SetTrigger("attack1");
  }

  public void Attack2(float x=0f, float y=0f) {
    rigAnimator.SetTrigger("attack2");
  }

  public void Jump() {
    rigAnimator.SetTrigger("jump");
  }

  public void Grounded() {
    rigAnimator.SetTrigger("grounded");
  }
}
