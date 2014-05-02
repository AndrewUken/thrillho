using UnityEngine;
using System.Collections;

public class CarvalloController : MonoBehaviour {

  public event System.Action<float,float> MeleeFired = (float horizontal,float vertical)=>{};
  public event System.Action<float,float> RangedFired = (float horizontal, float vertical)=>{};

	[SerializeField]
	private float runAcceleration;
  [SerializeField]
  private float horizontalJumpAcceleration;
	[SerializeField]
	private float walkTopSpeed;
	[SerializeField]
	private float runTopSpeed;

	[SerializeField]
	[Range(0,1)]
	private float kineticFriction;
	private float staticFriction = 0.002f;

	[SerializeField]
	private float jumpSpeed;
	[SerializeField]
	[Range(0,1)]
	private float jumpHoldTime;
	[SerializeField]
	[Range(1,2)]
	private float jumpFallGravityAccel;
	private float jumpStartTime = 0;
	private bool headingDown = false;

  [SerializeField]
  private BoxCollider2D footCollider;

  private bool wasJumpingDown = false;
  private bool grounded = false;
  private bool jumpLetGo = true;

	private float initialGravity;

  private int footLayer;
  private int platformLayer;

  public CameraController CameraController { get; set; }

  public CarvalloAnimator animator;

  void Start () {
		initialGravity = rigidbody2D.gravityScale;
    footLayer = LayerMask.NameToLayer("foot");
    platformLayer = LayerMask.NameToLayer("platform");
    animator = this.GetComponent<CarvalloAnimator>();
    MeleeFired += animator.Attack1;
	}

  void Update() {
    if(CameraController) {
      CameraController.FollowCharacterPosition(this.transform.position);
    }
  }
	
  void FixedUpdate () {
    float horizontal = Input.GetAxis(InputAxes.HORIZONTAL);
    float vertical = Input.GetAxis(InputAxes.VERTICAL);
    FixedJump(horizontal,vertical);
    FixedRun(horizontal,vertical);
    FixedAttack(horizontal,vertical);
    FixedCustomPhysics(horizontal,vertical);

    if(animator) {
      animator.SetVelocity(rigidbody2D.velocity);
    }
	}
  
  void FixedCustomPhysics(float horizontal, float vertical){
    Physics2D.IgnoreLayerCollision(footLayer,platformLayer, !headingDown && !grounded);
  }

  void FixedRun(float horizontal, float vertical){
		bool running = Input.GetButton(InputAxes.RUN) || GameConfig.Instance.autoRun;
    bool locked = Input.GetButton(InputAxes.LOCK);
		float topSpeed = running ? runTopSpeed : walkTopSpeed;
		if(horizontal != 0 && !locked){
      if(grounded){
  			rigidbody2D.AddForce(runAcceleration * horizontal * Time.fixedDeltaTime * transform.right);
      } else {
        rigidbody2D.AddForce(horizontalJumpAcceleration * horizontal * Time.fixedDeltaTime * transform.right);
      }
			if(Mathf.Abs(rigidbody2D.velocity.x) > topSpeed){
				rigidbody2D.velocity = new Vector2(topSpeed*horizontal,rigidbody2D.velocity.y);
			}
		} else {
			rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x*kineticFriction,rigidbody2D.velocity.y);
			if(Mathf.Abs(rigidbody2D.velocity.x) < staticFriction){
				rigidbody2D.velocity = new Vector2(0,rigidbody2D.velocity.y);
			}
		}
	}

  void FixedJump(float horizontal, float vertical){
    bool jumping = (Input.GetButton(InputAxes.JUMP) && vertical <= 0.7f);
		if(jumping){
			if(grounded && jumpLetGo){
				jumpStartTime = Time.time;
        AudioManager.Instance.PlaySound("Gameplay/Jump");
        animator.Jump();
        jumpLetGo = false;
        grounded = false;
			}
			if(Time.time - jumpStartTime < jumpHoldTime){
				rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x,jumpSpeed);
			}
    } else {
      jumpStartTime = 0;
      jumpLetGo = true;
    }
		if(!grounded){
			if(rigidbody2D.velocity.y < 0){
				headingDown = true;
			}
		}
		if(headingDown){
			rigidbody2D.gravityScale *= jumpFallGravityAccel;
			if(rigidbody2D.velocity.y >= 0){
				rigidbody2D.gravityScale = initialGravity;
				headingDown = false;
        grounded = true;
        animator.Grounded();
			}
    }
    bool jumpingDown = (Input.GetButton(InputAxes.JUMP) && vertical > 0.7f);
    if(jumpingDown){
      if(!wasJumpingDown){
        StartCoroutine(HopDown());
        grounded = false;
        headingDown = true;
      }
      wasJumpingDown = true;
    } else {
      wasJumpingDown = false;
    }
	}

  void FixedAttack(float horizontal, float vertical){
    if(Input.GetButtonDown(InputAxes.ATTACK_MELEE)){
      MeleeFired(horizontal,vertical);
    }
    if(Input.GetButton(InputAxes.ATTACK_RANGED)){
//      RangedFired(horizontal,vertical);
    }
  }

  private IEnumerator HopDown(){
    footCollider.enabled = false;
    yield return new WaitForSeconds(0.05f);
    footCollider.enabled = true;
  }
}
