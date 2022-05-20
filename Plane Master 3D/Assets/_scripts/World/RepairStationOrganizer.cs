using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairStationOrganizer : MonoBehaviour
{


	[SerializeField]
	int level;

	[SerializeField]
	List<BuildForMoney> stations = new List<BuildForMoney>();
    // Start is called before the first frame update
    void Start()
    {
		level = Mathf.Max(PlayerPrefs.GetInt("RepairStationOrganizerLevel"), level);
		for(int i = 0; i < level; i++)
		{
			stations[i].SetAsBought();
		}
		stations[level].ActivateBuyZone();
    }




	void OnBoughtStation()
	{
		level++;
		PlayerPrefs.SetInt("RepairStationOrganizerLevel", level);
		stations[level].ActivateBuyZone();
	}

    
}
