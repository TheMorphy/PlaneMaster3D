using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Aircraft
{
	[SerializeField]
	GameObject model;

	[SerializeField]
	int profit;

	[SerializeField]
	List<Breakable> breakables = new List<Breakable>();

	public GameObject Model { get => model; set => model = value; }
	public List<Breakable> Breakables { get => breakables; set => breakables = value; }
	public int Profit { get => profit; set => profit = value; }
}
