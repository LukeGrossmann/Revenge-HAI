﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class EnemyActions : Enemy {

	public GameObject target; //the enemy target
	public RANGE range; //the range to target
	public ENEMYTACTIC enemyTactic;//the current tactic
	public ENEMYSTATE enemyState;
	public EnemyAnimator animator;
	public GameObject GFX;
	public SpriteRenderer Shadow;
	public Rigidbody2D rb;
	public bool isDead;
	public float YHitDistance = 0.3f; //the Y distance from which the enemy is able to hit the player
    private float randomOffset = 0;
	private bool CR_running_resetOffset = false;
	private float MoveThreshold = 0.1f; //margin threshold before the enemy starts moving
    private float MoveThresholdY = 0.01f; //margin threshold before the enemy starts moving
    public float lastAttackTime; //the time of the last attack
	private bool attackWhilePlayerIsgrounded = true; //only attack when the player is on the ground, and not while he's jumping
	private int currentDirection = 0;
	public bool IsFlykick = true;
	public bool FlykickCompleted = true;
	private GameObject smokeEffectInstance = null;
	private bool CR_RUNNING_StopKickTimer = false;
	public void ZeroVelocity()
	{
		rb.velocity = Vector2.zero;
	}

	IEnumerator resetTimer(float time)
	{
		yield return new WaitForSeconds(time);
		IsFlykick = false;
		lastAttackTime = Time.time;
		animator.IdleForce();
	}

	IEnumerator CoolDownTimer(float time)
	{
		yield return new WaitForSeconds(time);
		
		StartCoroutine(RecoverTime(3f));
	}

	IEnumerator RecoverTime(float time)
	{
		yield return new WaitForSeconds(time);
		stophit();
		IsFlykick = true;
	}
	IEnumerator FlyKickTimer(float time)
	{
		yield return new WaitForSeconds(time);
		IsFlykick = true;
	}

	IEnumerator StopKickTimer(float time)
	{
		yield return new WaitForSeconds(time);

		stophit();
		StartCoroutine(FlyKickTimer(3f));
		CR_RUNNING_StopKickTimer = false;
	}

	public void stophit()
	{
		IsFlykick = false;
		FlykickCompleted = true;
		animator.Idle();
		//animator.Idle();
		rb.velocity = Vector2.zero;
		enemyState = ENEMYSTATE.IDLE;
	}
    public void ATTACK_AIR()
	{
		FlykickCompleted = false;

		//if (IsFlykick == false && Time.time - lastAttackTime > attackInterval)
		//{
		//	IsFlykick = true;
		//	StartCoroutine(resetTimer(0.5f));
		//}
		if (IsFlykick == true)
		{
			//StartCoroutine(CoolDownTimer(2f));
			
			enemyState = ENEMYSTATE.ATTACK;

			Debug.Log("AIR KICK");

			LookAtTarget();
			animator.Attack2();

			if (CheckForHit2())
			{
				Debug.Log("Kick landed");
				Debug.Log("Ending Kick");
				if (CR_RUNNING_StopKickTimer == false) {
					StartCoroutine(StopKickTimer(1f));
					CR_RUNNING_StopKickTimer = true;
				}
			
				//stophit();
				//StartCoroutine(FlyKickTimer(3f));
				return;
			}

			Debug.Log("DistanceToTargetX() " + DistanceToTargetX());
			int dir = (int)DirectionToPos(target.transform.position.x);
			if (DistanceToTargetX() < 1.0f)
			{
				Debug.Log("Kick missed! distance was" + DistanceToTargetX());
				Debug.Log("Ending Kick");
				currentDirection = dir;
				stophit();
				StartCoroutine(FlyKickTimer(3f));
				return;
			}
			//rb.AddForce(new Vector2(100 * dir, 0));
			//rb.velocity = new Vector2(10 * dir, 0);
			if (enemyType == ENEMYTYPE.Ninja)
			{
				if(smokeEffectInstance == null)
				smokeEffectInstance = Instantiate(SmokeEffect,self.transform.position,Quaternion.identity);
				Move(new Vector2(40 * dir, 0), 5); /// teleport ninjas
			}

			//if (enemyType == ENEMYTYPE.Fighter)
			MoveNoAnim(new Vector2(1 * dir, 0), 7.5f); /// Fighters
			//lastAttackTime = Time.time;
		}
			
	}

	public void ATTACK() {
		if(target == null) SetTarget2Player();

		if(Time.time - lastAttackTime > attackInterval){

			//checks if we should only attack when the player is grounded
			if(attackWhilePlayerIsgrounded){

				if(target.GetComponent<PlayerMovement>().playerIsGrounded()){
					enemyState = ENEMYSTATE.ATTACK;
					Move(Vector3.zero, 0); //when commented the character runs to the other side
					LookAtTarget();
					animator.Attack1();
					lastAttackTime = Time.time;
				}

			} else {

					enemyState = ENEMYSTATE.ATTACK;
					Move(Vector3.zero, 0); //when commented the character runs to the other side
					LookAtTarget();
					animator.Attack1();
					lastAttackTime = Time.time;
			}
		}
	}

	//move to the target
	public void MoveTo(float distance, float speed){
		LookAtTarget();

		//horizontal movement
		//Debug.Log("DistanceToTargetX" + (DistanceToTargetX() - distance));
		//Debug.Log("X position" + transform.position.x);
		//var totalDist = DistanceToTargetX() - distance;
		if (CR_running_resetOffset == false) { 
		ResetOffset(2.0f);
			CR_running_resetOffset = true;		
		}

		//move closer on horizontal line
		if (DistanceToTargetX() - distance > MoveThreshold + randomOffset)
		{
			Move(Vector3.right * Dir2DistPoint(distance, 1f), speed);
		}

		else if (Mathf.Abs(DistanceToTargetY()) > MoveThresholdY + randomOffset) // *Removed - distance from DistanceToTargetY.
		{

			//move closer on vertical line
			Move(Vector3.up * DirToVerticalLine(), speed / 1.5f);
		} 
	}

	IEnumerator ResetOffset(float time)
	{
		yield return new WaitForSeconds(time);
		randomOffset = Random.Range(-8.0f, 8.0f);
		CR_running_resetOffset = false;
	}

	//returns the direction to the distance point
	float Dir2DistPoint(float distance, float amountToMove){
		if(target == null) SetTarget2Player();

		//go Left
		if(transform.position.x < target.transform.position.x){
			float distancepointX = target.transform.position.x - distance;
			if(transform.position.x < distancepointX) 
				return amountToMove;
			else 
				return -amountToMove;
		} else {

		//go Right
			float distancepointX = target.transform.position.x + distance;
			if(transform.position.x > distancepointX) 
					return -amountToMove;
				else 
					return amountToMove;
		}
	}

	//set the layer order so this enemy appears behind or in front of other objects
	public void UpdateSpriteSorting(){
		GFX.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y*-10f); //100 is multiplied to spraid out the values
		Shadow.sortingOrder = GFX.GetComponent<SpriteRenderer>().sortingOrder-1;
	}

	//flips the sprite towards the current target
	public void LookAtTarget()	{
		if(target != null){
			if(transform.position.x >= target.transform.position.x){
				GFX.GetComponent<SpriteRenderer>().flipX = true;
			} else {
				GFX.GetComponent<SpriteRenderer>().flipX = false;
			}
		}
	}

	//returns the current direction
	public DIRECTION getCurrentDirection(){
		if(GFX.GetComponent<SpriteRenderer>().flipX){
			return DIRECTION.Left;
		} else 
			return DIRECTION.Right;
	}

	//return the direction to the wanted position
	public DIRECTION DirectionToPos(float xPos){
		if (transform.position.x >= xPos) return DIRECTION.Left;
		return DIRECTION.Right;
	}

	//return the direction to the target on a vertical ine
	int DirToVerticalLine(){
		if (target == null) return 0;
		if(transform.position.y > target.transform.position.y) 
			return -1;
		else 
			return 1;
	}

	//idle
	public void IDLE()	{
		Move(Vector3.zero, 0);
		enemyState = ENEMYSTATE.IDLE;
		animator.Idle();
	}

	//move towards a vector
	public void MovePhysics(Vector3 vector, float speed)
	{
		rb.velocity = vector * speed;

		if (speed > 0)
			animator.Walk();
		else
			animator.Idle();
	}
	public void Move(Vector3 vector, float speed){
		transform.position = new Vector2(transform.position.x + vector.x * speed * Time.deltaTime, transform.position.y + vector.y * speed * Time.deltaTime);
		//rb.velocity = vector * speed; !WARNING this produces a bug.

		if(speed>0) 
			animator.Walk();
		else
			animator.Idle();
	}

	public void MoveNoAnim(Vector3 vector, float speed)
	{
		//transform.position = new Vector2(transform.position.x + vector.x * speed * Time.deltaTime, transform.position.y + vector.y * speed * Time.deltaTime);
		transform.position = new Vector2(transform.position.x + vector.x * speed * Time.smoothDeltaTime, transform.position.y + vector.y * speed * Time.smoothDeltaTime);
	}

	//wait for an x number of seconds
	public IEnumerator Wait(float delay)	{
		yield return new WaitForSeconds (delay);
		enemyState = ENEMYSTATE.IDLE;
	}

	//x distance to the target
	public float DistanceToTargetX(){
		if(target != null)
			return Mathf.Abs(transform.position.x - target.transform.position.x);
		else
			return -1;
	}

	//y distance to the target
	public float DistanceToTargetY(){
		if(target != null)
			return Mathf.Abs(transform.position.y - target.transform.position.y);
		else
			return -1;
	}

	//check if the attack hits and deal damage to the target
	public void CheckForHit(){
		if(DistanceToTargetX() < attackRange && DistanceToTargetY()<YHitDistance){ 
			DealDamageToTarget();
		}
	}

	public bool CheckForHit2()
	{
		if (DistanceToTargetX() < attackRange && DistanceToTargetY() < YHitDistance)
		{
			DealDamageToTarget(kickDamage);
			return true;
		}
		else return false;
	}

	//deal damage to the current target
	void DealDamageToTarget(){
		if(target != null){
			DamageObject d = new DamageObject(attackDamage, gameObject);
			d.attackType = AttackType.Default;
			target.SendMessage("Hit", d, SendMessageOptions.DontRequireReceiver);
		}
	}

	void DealDamageToTarget(int damage)
	{
		if (target != null)
		{
			DamageObject d = new DamageObject(damage, gameObject);
			d.attackType = AttackType.Default;
			target.SendMessage("Hit", d, SendMessageOptions.DontRequireReceiver);
		}
	}

	//enemy knockDown
	public IEnumerator KnockDown(DIRECTION dir){
		float t=0;
		float travelSpeed = 2f;

		//play knockdown animation
		animator.KnockDown();

		//knock down air
		while(t<1 && !isDead){
			rb.velocity = Vector2.left * (int)dir * travelSpeed;
			t += Time.deltaTime;
			yield return 0;
		}

		//knock down grounded
		rb.velocity = Vector2.zero;
		enemyState = ENEMYSTATE.KNOCKDOWNGROUNDED;
	}

	//remove enemy from the playing field with a flicker effect
	public IEnumerator RemoveEnemy(){

		float osc;
		float speed = 3f;
		float startTime = Time.time;

		//pause before flickering start
		yield return new WaitForSeconds(1.5f);

		while(true){
			osc = Mathf.Sin((Time.time - startTime) * speed);
			speed += Time.deltaTime * 10;

			//Switch between sprites
			GFX.GetComponent<SpriteRenderer>().enabled = (osc>0);

			if(speed > 35) DestroyUnit();
			yield return null;
		}
	}

	//show hit effect
	public void ShowHitEffectAtPosition(Vector3 pos) {
		GameObject.Instantiate (Resources.Load ("HitEffect"), pos, Quaternion.identity);
	}

	//set the player as target
	public void SetTarget2Player(){
		target = GameObject.FindGameObjectWithTag("Player");
	}

	//randomizes values for variation
	public void RandomizeValues(){
		walkSpeed *= Random.Range(.8f, 1.2f);
		attackInterval *= Random.Range(.8f, 1.2f);
	}
}