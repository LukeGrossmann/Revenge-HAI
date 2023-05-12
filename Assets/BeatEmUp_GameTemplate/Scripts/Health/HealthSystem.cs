using UnityEngine;
using System.Collections;

public class HealthSystem : MonoBehaviour {

	public int MaxHp = 500;
	public float CurrentHp = 500;
	public bool invulnerable;
	public delegate void OnHealthChange(float percentage, GameObject GO);
	public static event OnHealthChange onHealthChange;

	//substract health
	public void SubstractHealth(float damage){
		if(!invulnerable){
			
			//reduce hp
			CurrentHp = Mathf.Clamp(CurrentHp -= damage, 0, MaxHp);

			//sendupdate Health Event
			SendUpdateEvent();
		}
	}

	//add health
	public void AddHealth(int amount){
		CurrentHp = Mathf.Clamp(CurrentHp += amount, 0, MaxHp);
		SendUpdateEvent();
	}


	//health update event
	void SendUpdateEvent(){
		float CurrentHealthPercentage = 1f/MaxHp * CurrentHp;
		if(onHealthChange != null) onHealthChange(CurrentHealthPercentage, gameObject);
	}
}
