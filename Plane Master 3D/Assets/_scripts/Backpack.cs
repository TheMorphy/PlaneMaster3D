using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class UIItem
{
	public ItemType itemType;
    public int count;
    public TextMeshProUGUI text;
}
[System.Serializable]
public class ItemStack
{
	public List<Item> items = new List<Item>();
	public ItemType itemType;
	public TextMeshPro text;
}
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
	public List<ItemStack> itemStacks = new List<ItemStack>();
	[SerializeField]
	GameObject stackTextPrefab;

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
    [SerializeField]
    List<UIItem> UIItems = new List<UIItem>();

    void RefreshItemUI()
	{
		for(int u = 0; u < UIItems.Count; u++)
		{
			ItemStack stack = itemStacks.Find(stack => stack.itemType == UIItems[u].itemType);
			if (UIItems[u].itemType == ItemType.Money)
			{
				if(stack != null)
				{
					UIItems[u].text.gameObject.SetActive(true);
					
					int count = 0;
					for(int i = 0; i < stack.items.Count; i++)
					{
						count += stack.items[i].amount;
					}
					UIItems[u].text.text = count.ToString();
					stack.text.text = count.ToString();

					stack.text.transform.SetParent(stack.items[stack.items.Count - 1].transform, false);
				}
				else
				{
					UIItems[u].text.gameObject.SetActive(false);
				}
			}
			else
			{
				
				if (stack != null)
				{
					UIItems[u].text.gameObject.SetActive(true);
					UIItems[u].text.text = stack.items.Count.ToString();
					stack.text.text = stack.items.Count.ToString();
					stack.text.transform.SetParent(stack.items[stack.items.Count - 1].transform, false);
				}
				else
				{
					UIItems[u].text.gameObject.SetActive(false);
				}
			}
			
		}
        

    }
    

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


	bool tryAddItem(Item item)
	{
		bool added = false;
		bool stackExist = false;
		for(int s = 0; s < itemStacks.Count; s++)
		{
			if(itemStacks[s].itemType == item.itemType)
			{
				stackExist = true;
				if(itemStacks[s].items.Count < stackSize || item.itemType == ItemType.Money)
				{
					itemStacks[s].items.Add(item);
					added = true;
				}
			}
		}
		if(stackExist == false)
		{
			ItemStack newItemStack = new ItemStack();
			itemStacks.Add(newItemStack);
			newItemStack.items.Add(item);
			newItemStack.itemType = item.itemType;
			UpdateItemDestinations();
			newItemStack.text = Instantiate(stackTextPrefab, item.transform, false).GetComponent<TextMeshPro>();
			
			added = true;
		}
		return added;
	}



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
                            RefreshItemUI();
                            c.isTrigger = true;
                        }
                    }
                }
                else if(!i.pickedUp)
                {
					if (tryAddItem(i))
					{
						i.pickedUp = true;
						//integrade in new system



						//
						//items.Add(i);
						RefreshItemUI();
						if(i.transform.parent != null)
						if (i.transform.parent.parent != null)
						{
							i.transform.parent.parent.SendMessage("RemoveItem", SendMessageOptions.DontRequireReceiver);
						}
						i.transform.parent = itemParent;
						UpdateItemDestinations();
						c.enabled = false;
						//AddProgress
						if (i.itemName == "Iron")
						{
							QuestSystem.instance.AddProgress("Collect Iron", 1);
						}
						//Play pickup sound
						if (itemSoundSource != null)
						{
							itemSoundSource.PlayOneShot(pickUpSounds[Random.Range(0, pickUpSounds.Count)]);
						}
					}
//						print("cant add item. Stack is full");
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
                            if (SearchForItemType(u.itemType))
                            {
                                Item itemToDrop = DropItem(u.itemType);
                                Transform itemTransform = itemToDrop.transform;
								if(itemToDrop.amount > u.countNeeded - u.count)
								{
									int change = itemToDrop.amount - (u.countNeeded - u.count);
									while(change > 0)
									LevelSystem.SpawnMoneyAtPosition(ref change, player.transform.position);
								}
								u.count += itemToDrop.amount;
                                if (!droppingZone.showDroppedItems)
                                {
                                    itemTransform.parent = u.itemDestination;
                                    StartCoroutine(LerpItemToDestination(itemTransform));
                                    droppingZone.AddItem(itemToDrop, false);
                                }
                                else
                                {
                                    droppingZone.AddItem(itemToDrop);
                                    itemTransform.parent = u.itemDestination;
                                }
                                
                                RefreshItemUI();
								//Update The destination for all the other items
								UpdateItemDestinations();

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
                        if (u.completed == false && dropTime <= 0)
                        {
                            if (SearchForItemType(u.itemType))
                            {
                                Item itemToDrop = DropItem(u.itemType);
                                print("item to drop: " + itemToDrop);
                                Transform itemTransform = itemToDrop.transform;
                                u.count++;
                                if (!droppingZone.showDroppedItems)
                                {
                                    itemTransform.parent = u.itemDestination;
                                    StartCoroutine(LerpItemToDestination(itemTransform));
                                    droppingZone.AddItem(itemToDrop, false);
                                }
                                else
                                {
                                    droppingZone.AddItem(itemToDrop);
                                    itemTransform.parent = u.itemDestination;
                                }


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

	bool SearchForItemType(ItemType itemType)
	{
		for (int s = 0; s < itemStacks.Count; s++)
		{
			if (itemStacks[s].itemType == itemType && itemStacks[s].items.Count > 0)
			{
				return true;
			}
		}
		return false;
	}
    Item DropItem(ItemType itemType)
    {
		Item itemToDrop;
		for(int s = 0; s < itemStacks.Count; s++)
		{
			if(itemStacks[s].itemType == itemType)
			{
				itemToDrop = itemStacks[s].items[itemStacks[s].items.Count - 1];
				itemStacks[s].items.RemoveAt(itemStacks[s].items.Count - 1);
				if(itemStacks[s].items.Count == 0)
				{
					Destroy(itemStacks[s].text.gameObject);
					itemStacks.RemoveAt(s);
				}
				return itemToDrop;
			}
		}
		return null;


		/*
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
		*/
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }

	void UpdateItemDestinations()
	{
		Vector3 pos = Vector3.zero;





		for (int s = 0; s < itemStacks.Count; s++)
		{
			pos.z = -s * stackOffset;
			for (int i = 0; i < itemStacks[s].items.Count; i++)
			{
				Item curItem = itemStacks[s].items[i];
				curItem.destination = pos;

				if (curItem.lerpCoroutine != null)
					StopCoroutine(curItem.lerpCoroutine);
				curItem.lerpCoroutine = StartCoroutine(BringItemToDesPosition(curItem));

				pos.y += curItem.height;
			}
			pos.y = 0;
		}











	/*
		//old from here
		//print(start);
		//Vector3 pos = Vector3.zero;
		if(start > 0 && start % stackSize > 0)
		{
			
			pos = items[start - 1].destination + Vector3.up * items[start - 1].height;
			//print(start + "|" + System.Convert.ToInt32(start / stackSize));
		}

		float iterations = (float)items.Count / stackSize;

		for (int i = (int)start / stackSize; i < Mathf.CeilToInt(iterations); i++)
		{

            //set the distance betweeen the stacks 
			pos.z = -i * stackOffset;
			
			for (int x = start; x < Mathf.Min(items.Count, stackSize * (i + 1)); x++)
			{
				//Debug.Log($"Started at: {start} \n now setting pos for x")
				items[x].destination = pos;

				if (items[x].lerpCoroutine != null)
					StopCoroutine(items[x].lerpCoroutine);
				items[x].lerpCoroutine = StartCoroutine(BringItemToDesPosition(items[x]));

				pos.y += items[x].height;
                start = x;
			}
			pos.y = 0;
			
			//pos.z += stackOffset;
		}
	*/
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


	
}
