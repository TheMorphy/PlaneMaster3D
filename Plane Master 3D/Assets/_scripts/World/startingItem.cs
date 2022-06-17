using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startingItem : MonoBehaviour
{
    Item item;
    string savingKey;
    // Start is called before the first frame update
    void Start()
    {
        savingKey = transform.position.sqrMagnitude.ToString();
        item = GetComponent<Item>();
        if(PlayerPrefs.GetInt(savingKey + "startItem") == 1)
		{
            Destroy(gameObject);
		}
        else
        StartCoroutine(WaitForPickUp());
    }

    void SavePickUp()
	{
        PlayerPrefs.SetInt(savingKey + "startItem", 1);
	}


    IEnumerator WaitForPickUp()
	{
        yield return new WaitUntil(() => item.pickedUp == true);
        SavePickUp();
	}
    // Update is called once per frame
    void Update()
    {
        
    }
}
