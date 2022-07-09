using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{

	[SerializeField] GameObject exp;
	[SerializeField] float expForce, radius;

	private void OnCollisionEnter(Collision other)
	{
		GameObject _exp = Instantiate(exp, transform.position, transform.rotation);
		Destroy(_exp, 3);
		KnockBack();
		Destroy(gameObject);
	}

	void KnockBack()
	{
		Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

		foreach (Collider nearby in colliders)
		{
			Rigidbody rigg = nearby.GetComponent<Rigidbody>();
			if(rigg != null)
			{
				rigg.AddExplosionForce(expForce, transform.position, radius);
			}
		}
	}
}
