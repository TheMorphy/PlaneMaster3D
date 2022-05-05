using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerTest : MonoBehaviour
{
    //[SerializeField] List<Transform> realNormalPos = new List<Transform>();

    //[SerializeField] List<Transform> truckNormalPos = new List<Transform>();

    [SerializeField] PlaneMain ps;

    GameObject addObj;

    public void DoScript(List<Transform> groundPositions)
    {
        //ps.enabled = true;
        StartCoroutine(MoveBreakablesToGroundDestinations(groundPositions));
    }

    IEnumerator MoveBreakablesToGroundDestinations(List<Transform> groundPositions)
    {
        int firstInt = ps.randomNumbers[0];

        while(Vector3.Distance(ps.breakables[firstInt].transform.position, groundPositions[firstInt].position) > 1f)
        {
			print("MoveBreakablesToGroundDestinations");
            /*for (int i = 0; i < ps.breakables.Count; i++)
            {
                ps.breakables[i].transform.position = Vector3.Lerp(ps.breakables[i].transform.position, groundPositions[i].position, 0.1f);
                ps.breakables[i].transform.rotation = Quaternion.Lerp(ps.breakables[i].transform.rotation, groundPositions[i].rotation, 0.1f);
            }*/

            for (int i = 0; i < ps.randomNumbers.Count; i++)
            {
                int rand = ps.randomNumbers[i];
                //print(rand);
                ps.breakables[rand].transform.position = Vector3.Lerp(ps.breakables[rand].transform.position, groundPositions[i].position, 0.1f);
                ps.breakables[rand].transform.rotation = Quaternion.Lerp(ps.breakables[rand].transform.rotation, groundPositions[i].rotation, 0.1f);
				if(Vector3.Distance(ps.breakables[rand].transform.position, groundPositions[i].position) <= 0.1f)
				{
					yield break;
				}
            }
            yield return null;
        }
        //Parts moved to the ground
        yield break;
    }
}
