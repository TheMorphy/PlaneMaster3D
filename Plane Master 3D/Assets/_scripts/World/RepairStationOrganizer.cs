using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RepairStationOrganizer : MonoBehaviour
{


	[SerializeField]
	public int level;

	[SerializeField]
	List<BuildForMoney> stations = new List<BuildForMoney>();
	public UnityEvent OnLevelUp;
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
		OnLevelUp.Invoke();
		PlayerPrefs.SetInt("RepairStationOrganizerLevel", level);
		if(stations.Count < level)
		stations[level].ActivateBuyZone();
	}

    
}
