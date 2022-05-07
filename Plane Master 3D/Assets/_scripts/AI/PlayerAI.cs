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

	IEnumerator BringPlayerToDestination(Vector3 destination, bool destroyWhenReached)
	{
		//Assign variables
		Player player = GetComponent<Player>();
		NavMeshAgent agent = GetComponent<NavMeshAgent>();
		CharacterController controller = GetComponent<CharacterController>();

		//change properties of the variables
		player.enabled = false;
		controller.enabled = false;
		agent.enabled = true;
		agent.SetDestination(destination);

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
