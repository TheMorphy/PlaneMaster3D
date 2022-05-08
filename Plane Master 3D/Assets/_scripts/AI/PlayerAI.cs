using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerAI : MonoBehaviour
{
	[SerializeField] Animator visual3D;
	public void ActivatePlayerAI(Vector3 destination, bool destroyWhenReached)
	{
		print("ActivatePlayerAI");
		StartCoroutine(BringPlayerToDestination(destination, destroyWhenReached));
	}

	public void PausePlayerAI()
	{
		NavMeshAgent agent = GetComponent<NavMeshAgent>();
		agent.speed = 0;
		visual3D.SetFloat("Speed", 0);

	}

	public void ResumePlayerAI()
	{
		NavMeshAgent agent = GetComponent<NavMeshAgent>();
		agent.speed = 4;
		visual3D.SetFloat("Speed", 4);

	}

	IEnumerator BringPlayerToDestination(Vector3 destination, bool destroyWhenReached)
	{
		//Assign variables
		Player player = GetComponent<Player>();
		NavMeshAgent agent = GetComponent<NavMeshAgent>();
		CharacterController controller = GetComponent<CharacterController>();

		//enable/disable components
		player.enabled = false;
		controller.enabled = false;
		agent.enabled = true;

		//set the destination for the player
		agent.SetDestination(destination);

		//Make sure the animation plays at the right speed
		visual3D.SetFloat("Speed", 4);

		yield return new WaitForSeconds(0.1f);
		//Wait
		yield return new WaitUntil(() => agent.remainingDistance < agent.stoppingDistance);
		if(destroyWhenReached)
		{
			Destroy(this.gameObject);
		}
		else
		{
			
			player.enabled = true;
			controller.enabled = true;
			agent.enabled = false;
			
		}
		
	}
}
