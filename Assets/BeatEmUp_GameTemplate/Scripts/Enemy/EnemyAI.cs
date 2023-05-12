using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : EnemyActions {

	public Score score;
	public float XDistance = 0;
	public float YDistance = 0;
	public bool enableAI;
	private List<ENEMYSTATE> ActiveAIStates = new List<ENEMYSTATE> { ENEMYSTATE.IDLE, ENEMYSTATE.RUN, ENEMYSTATE.WALK ,ENEMYSTATE.ATTACK}; //a list of states where the AI is executed
	private List<ENEMYSTATE> HitStates = new List<ENEMYSTATE> { ENEMYSTATE.DEATH, ENEMYSTATE.KNOCKDOWN, ENEMYSTATE.KNOCKDOWNGROUNDED }; //a list of states where the enemy is hit

	void Start(){
		score = GameObject.Find("score_value").GetComponent<Score>();
		animator = GFX.GetComponent<EnemyAnimator>();
		rb = GetComponent<Rigidbody2D>();
		EnemyManager.enemyList.Add(gameObject);
		RandomizeValues();
		//if(enemyType == ENEMYTYPE.Fighter)
		//GFX.AddComponent<ChangeColor>();
	}

	void OnEnable(){
		SetTarget2Player();
	}

	void Update(){
		if(!isDead && enableAI){
			if(ActiveAIStates.Contains(enemyState) && targetSpotted){
				AI();

			} else {

				//look for a target
			 	Look4Target();
			}
		}
		UpdateSpriteSorting();
	}

	void AirAttack()
	{
		if (target == null) target = GameObject.Find("Player1"); //return;
		if ((transform.position.y < target.transform.position.y + 0.3f && transform.position.y > target.transform.position.y - 0.3f)) ATTACK_AIR();
		else
		{
			FlykickCompleted = true;
		}
	}
	void AI(){
		LookAtTarget();
		range = GetRangeToTarget();

		//attack range
		if (range == RANGE.ATTACKRANGE){
			if (enemyTactic == ENEMYTACTIC.ENGAGE)
			{
				if (enemyType == ENEMYTYPE.Fighter || enemyType == ENEMYTYPE.Ninja)
				{
					if (FlykickCompleted == true)
					{
						var randnum = Random.Range(1, 3);
						if (randnum == 2)
							AirAttack();
						else ATTACK();
					}
					else AirAttack();
				}
				else ATTACK();
			}

			if(enemyTactic == ENEMYTACTIC.KEEPSHORTDISTANCE) MoveTo(closeRangeDistance, walkSpeed);
			if(enemyTactic == ENEMYTACTIC.KEEPMEDIUMDISTANCE) MoveTo(midRangeDistance, walkSpeed);
			if(enemyTactic == ENEMYTACTIC.KEEPFARDISTANCE) MoveTo(farRangeDistance, walkSpeed);
   		    if(enemyTactic == ENEMYTACTIC.STANDSTILL) IDLE();
		}

		//close range
		if(range == RANGE.CLOSERANGE){
				if (enemyTactic == ENEMYTACTIC.ENGAGE)
				{
					if (enemyType == ENEMYTYPE.Fighter || enemyType == ENEMYTYPE.Ninja)
					{ 
						if (FlykickCompleted == true)
						{
							var randnum = Random.Range(1, 3);
							if (randnum == 2)
								AirAttack();
							else MoveTo(attackRange-.2f, walkSpeed);
						}
						else AirAttack();		
					} else MoveTo(attackRange - .2f, walkSpeed);
				}
			if(enemyTactic == ENEMYTACTIC.KEEPSHORTDISTANCE) MoveTo(closeRangeDistance, walkSpeed);
			if(enemyTactic == ENEMYTACTIC.KEEPMEDIUMDISTANCE) MoveTo(midRangeDistance, walkSpeed);
			if(enemyTactic == ENEMYTACTIC.KEEPFARDISTANCE) MoveTo(farRangeDistance, walkSpeed);
			if(enemyTactic == ENEMYTACTIC.STANDSTILL) IDLE();
		}

		//mid range
		if(range == RANGE.MIDRANGE){
			if (enemyTactic == ENEMYTACTIC.ENGAGE)
			{
				if (enemyTactic == ENEMYTACTIC.ENGAGE)
				{
					if (enemyType == ENEMYTYPE.Fighter || enemyType == ENEMYTYPE.Ninja)
					{
						if (FlykickCompleted == true)
						{
							var randnum = Random.Range(1, 3);
							if (randnum == 2)
								AirAttack();
							else MoveTo(attackRange - .2f, walkSpeed);
						}
						else AirAttack();
					}
					else MoveTo(attackRange - .2f, walkSpeed);
				}
			}
			if(enemyTactic == ENEMYTACTIC.KEEPSHORTDISTANCE) MoveTo(closeRangeDistance, walkSpeed);
			if(enemyTactic == ENEMYTACTIC.KEEPMEDIUMDISTANCE) MoveTo(midRangeDistance, walkSpeed);
			if(enemyTactic == ENEMYTACTIC.KEEPFARDISTANCE) MoveTo(farRangeDistance, walkSpeed);
			if(enemyTactic == ENEMYTACTIC.STANDSTILL) IDLE();
		}

		//far range
		if(range == RANGE.FARRANGE){
			if(enemyTactic == ENEMYTACTIC.ENGAGE) MoveTo(attackRange -.2f, walkSpeed);
			if (enemyTactic == ENEMYTACTIC.KEEPSHORTDISTANCE) MoveTo(closeRangeDistance, walkSpeed);
			if(enemyTactic == ENEMYTACTIC.KEEPMEDIUMDISTANCE) MoveTo(midRangeDistance, walkSpeed);
			if(enemyTactic == ENEMYTACTIC.KEEPFARDISTANCE) MoveTo(farRangeDistance, walkSpeed);
			if(enemyTactic == ENEMYTACTIC.STANDSTILL) IDLE();
		}
	}

	public void Hit(DamageObject d){

		//stop moving
		Move(Vector3.zero, 0);

		//look towards inflictor
		if(target != null) target = d.inflictor;
		LookAtTarget();

		//show hit effect
		if(!isDead){
			ShowHitEffectAtPosition(transform.position + Vector3.up * Random.Range(1.0f, 2.0f));
			GlobalAudioPlayer.PlaySFX ("PunchHit");
		}

		//enemy can be hit
		if(!HitStates.Contains(enemyState) && !isDead){

			//showHitEffectAtPosition
			ShowHitEffectAtPosition(transform.position + Vector3.up * Random.Range(1.0f, 2.0f));

			//sfx
			GlobalAudioPlayer.PlaySFX ("PunchHit");

			//knockdown
			if(d.attackType == AttackType.KnockDown){

				enemyState = ENEMYSTATE.KNOCKDOWN;
				StartCoroutine(KnockDown(DirectionToPos(d.inflictor.transform.position.x)));

			} else {

				//normal hit
				animator.Hit();
				enemyState = ENEMYSTATE.HIT;
			}             
		}

		//unit is dead
		if(GetComponent<HealthSystem>() != null && GetComponent<HealthSystem>().CurrentHp == 0 && !isDead){
			Move(Vector3.zero, 0);
			UnitIsDead();
		}
	}

	//Unit has died
	void UnitIsDead(){
		isDead = true;
		enableAI = false;
		Move(Vector3.zero, 0);
		enemyState = ENEMYSTATE.DEATH;
		animator.Death();
		StartCoroutine(RemoveEnemy());
		EnemyManager.RemoveEnemyFromList(gameObject);

		//Add score....
		score.addscore(10);
	}

	//sets the current range
	public RANGE GetRangeToTarget(){
		XDistance = DistanceToTargetX();
		YDistance = DistanceToTargetY();

		//AttackRange
		if(XDistance <= attackRange && YDistance <= .2f) return RANGE.ATTACKRANGE;

		//Close Range
		if(XDistance > attackRange && XDistance < midRangeDistance) return RANGE.CLOSERANGE;

		//Mid range
		if(XDistance > closeRangeDistance && XDistance < farRangeDistance) return RANGE.MIDRANGE;

		//Far range
		if(XDistance > farRangeDistance) return RANGE.FARRANGE;

		return RANGE.FARRANGE;
	}

	//set an enemy tactic
	public void SetEnemyTactic(ENEMYTACTIC tactic){
		enemyTactic = tactic;
	}

	//checks if the target is in sight
	void Look4Target(){
		targetSpotted = DistanceToTargetX() < sightDistance;
	}

	void createSmoke()
	{
		Instantiate(SmokeEffect);
	}
}

public enum ENEMYSTATE {
	IDLE,
	ATTACK,
	WALK,
	RUN,
	HIT,
	KNOCKDOWN,
	KNOCKDOWNGROUNDED,
	DEATH,
}

public enum ENEMYTACTIC {
	ENGAGE = 0,
	KEEPSHORTDISTANCE = 1,
	KEEPMEDIUMDISTANCE = 2,
	KEEPFARDISTANCE = 3,
	STANDSTILL = 4,
	AIRATTACK = 5
}

public enum RANGE {
	ATTACKRANGE,
	CLOSERANGE,
	MIDRANGE,
	FARRANGE,
}
