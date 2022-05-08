using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour
{
    [SerializeField]
    public int backpackSize = 50;
    [SerializeField]
    int stackSize = 10;
    [SerializeField]
    float stackOffset, pickupRadius, itemDropInterval;
    [SerializeField]
    public List<Item> items = new List<Item>();
    [SerializeField]
    Transform itemParent;
    [SerializeField]
    Player player;
    [SerializeField]
    WorkerAI worker;
	[SerializeField]
	AudioSource itemSoundSource;
	[SerializeField]
	List<AudioClip> pickUpSounds = new List<AudioClip>(), dropSounds = new List<AudioClip>();
	[SerializeField]
	[Range(0.01f, 1)]
	float itemLerpTime = 0.15f;
	[SerializeField]
	AnimationCurve itemLerpCurve;
    

    //private variables
    float dropTime;
    // Start is called before the first frame update
    void Start()
    {
		//StartCoroutine(ItemHandler());
		SoundSystem.instance.sounds.Find(sound => sound.name == "ItemSounds").sources.Add(itemSoundSource);
	}

    private void OnEnable()
    {
        //StartCoroutine(ItemHandler());	
    }

    private void OnDisable()
    {
        //StopCoroutine(ItemHandler());
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        
        dropTime -= Time.deltaTime;
        //Check For Item to pick up
        foreach (Collider c in Physics.OverlapSphere(transform.position, pickupRadius))
        {
            if(c.GetComponent<Item>() != null && items.Count < backpackSize)
            {
                Item i = c.GetComponent<Item>();
                if(worker != null)
                {
                    if(i.itemName == worker.itemToCarry)
                    {
                        if (!i.pickedUp)
                        {
                            i.pickedUp = true;
                            items.Add(i);
                            c.isTrigger = true;
                        }
                    }
                }
                else if(!i.pickedUp)
                {
                    i.pickedUp = true;
                    items.Add(i);
					i.transform.parent = itemParent;
					UpdateItemDestinations(items.Count - 1);
					c.enabled = false;
					//AddProgress
                    if(i.itemName == "Iron")
                    {
                        QuestSystem.instance.AddProgress("Collect Iron", 1);
                    }
                    //Play pickup sound
					if(itemSoundSource != null)
					{
						itemSoundSource.PlayOneShot(pickUpSounds[Random.Range(0, pickUpSounds.Count)]);
					}
                }
            }

            if(player != null)
            {
                if (c.gameObject.layer == 7 && c.GetComponent<DroppingZone>().enabled && !player.isMoving && dropTime <= 0)
                {
                    DroppingZone droppingZone = c.GetComponent<DroppingZone>();
                    foreach (UpgradeCondition u in droppingZone.conditions)
                    {
                        if (!u.completed && dropTime <= 0)
                        {
                            if (CheckItems(u.name) >= 0)
                            {
                                int itemToDrop = CheckItems(u.name);
                                Transform itemTransform = items[itemToDrop].transform;
                                u.count++;
                                if (!droppingZone.showDroppedItems)
                                {
                                    itemTransform.parent = u.itemDestination;
                                    StartCoroutine(LerpItemToDestination(itemTransform));
                                    droppingZone.AddItem(items[itemToDrop], false);
                                }
                                else
                                {
                                    droppingZone.AddItem(items[itemToDrop]);
                                    itemTransform.parent = u.itemDestination;
                                }
                                items.RemoveAt(itemToDrop);

								//Update The destination for all the other items
								UpdateItemDestinations(itemToDrop);

								//Play Drop sound
								itemSoundSource.PlayOneShot(dropSounds[Random.Range(0, dropSounds.Count)]);

								if (items[items.Count - 1] == null)
                                {
                                    items.RemoveAt(items.Count - 1);
                                }

                                dropTime = itemDropInterval;
                            }
                        }
                    }
                }
            }
            else if(worker != null)
            {
                if (c.gameObject.layer == 7 && !worker.isMoving && dropTime <= 0)
                {
                    DroppingZone droppingZone = c.GetComponent<DroppingZone>();
                    foreach (UpgradeCondition u in droppingZone.conditions)
                    {
                        if (!u.completed && dropTime <= 0)
                        {
                            if (CheckItems(u.name) >= 0)
                            {
                                int itemToDrop = CheckItems(u.name);
                                print("item to drop: " + itemToDrop);
                                Transform itemTransform = items[itemToDrop].transform;
                                u.count++;
                                if (!droppingZone.showDroppedItems)
                                {
                                    itemTransform.parent = u.itemDestination;
                                    StartCoroutine(LerpItemToDestination(itemTransform));
                                    droppingZone.AddItem(items[itemToDrop], false);
                                }
                                else
                                {
                                    droppingZone.AddItem(items[itemToDrop]);
                                    itemTransform.parent = u.itemDestination;
                                }

                                items.RemoveAt(itemToDrop);

                                //if (items[items.Count - 1] == null)
                               // {
                               //     items.RemoveAt(items.Count - 1);
                              //  }

                                dropTime = itemDropInterval;
                            }
                        }
                    }
                }
            }
            
        }
    }

    IEnumerator LerpItemToDestination(Transform item, bool destroy = true)
    {
        while(item.transform.localPosition != Vector3.zero)
        {
            item.transform.localPosition = Vector3.Lerp(item.transform.localPosition, Vector3.zero, 0.1f);
            yield return null;
        }
        //if(destroy)
        //Destroy(item.transform.GetChild(0).gameObject);
        
    } 
    int CheckItems(string itemName)
    {
        int output = -1;
        for(int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                items.RemoveAt(i);
                return -1;
            }
                
            if(items[i].itemName == itemName)
            {
                output = i;
            }
        }
        return output;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }

	void UpdateItemDestinations(int start = 0)
	{
		//print(start);
		Vector3 pos = Vector3.zero;
		if(start > 0 && start % stackSize > 0)
		{
			
			pos = items[start - 1].destination + Vector3.up * items[start - 1].height;
			//print(start + "|" + System.Convert.ToInt32(start / stackSize));
		}

		float iterations = (float)items.Count / stackSize;

		for (int i = (int)start / stackSize; i < Mathf.CeilToInt(iterations); i++)
		{
			pos.z = -i * stackOffset;
			for (int x = start; x < Mathf.Min(items.Count, stackSize * (i + 1)); x++)
			{
				print("posy: " + pos.y);
				//print(start + " i: " + i + "\n" + pos);
				items[x].destination = pos;

				if (items[x].lerpCoroutine != null)
					StopCoroutine(items[x].lerpCoroutine);
				items[x].lerpCoroutine = StartCoroutine(BringItemToDesPosition(items[x]));

				pos.y += items[x].height;
			}
			pos.y = 0;
			
			//pos.z += stackOffset;
		}
	}

	IEnumerator BringItemToDesPosition(Item i)
	{
		Vector3 startPos = i.transform.localPosition;
		Quaternion startRot = i.transform.localRotation;
		float t = 0;
		while(t < 1)
		{
			t += Time.deltaTime / itemLerpTime;
			Vector3 curPos = Vector3.Lerp(startPos, i.destination, itemLerpCurve.Evaluate(t));
			Quaternion curRot = Quaternion.Lerp(startRot, Quaternion.Euler(Vector3.zero), t);
			i.transform.localPosition = curPos;
			i.transform.localRotation = curRot;
			
			yield return null;
		}
		yield break;
	}


	//Trying to not use it anymore
    IEnumerator ItemHandler()
    {
        while(false == false)
        {
            float nextyOffset = 0;
            float iterations = (float)items.Count / stackSize;
            yield return new WaitUntil(() => items.Count > 0);
            for (int i = 0; i < Mathf.CeilToInt(iterations); i++)
            {
                float nextItemOffset = 0;
                for (int a = i * stackSize; a < Mathf.Min(items.Count, stackSize * (i + 1)); a++)
                {
                    Vector3 destinatedPos = Vector3.up * nextItemOffset - Vector3.forward * nextyOffset;
                    if (items[a].transform.localPosition != destinatedPos)
                    {
                        items[a].transform.parent = itemParent;
                        items[a].transform.localPosition = Vector3.Lerp(items[a].transform.localPosition, destinatedPos, 0.1f);
                        items[a].transform.localRotation = Quaternion.Lerp(items[a].transform.localRotation, Quaternion.Euler(Vector3.zero), 0.1f);
                    }
                    nextItemOffset += items[a].height;
                }
                nextyOffset += stackOffset;
            }
            yield return null;
        }
    }
}
