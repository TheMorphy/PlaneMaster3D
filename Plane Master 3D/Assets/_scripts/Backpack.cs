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
    public int stackSize = 10;
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
					UIItems[u].text.transform.parent.gameObject.SetActive(true);
					
					int count = 0;
					for(int i = 0; i < stack.items.Count; i++)
					{
						count += stack.items[i].amount;
					}
					UIItems[u].text.text = count.ToString();
					

					if(stack.text != null)
					{
						stack.text.text = count.ToString();

						stack.text.transform.SetParent(stack.items[stack.items.Count - 1].transform, false);
						stack.text.enabled = true;
						stack.text.GetComponent<LookAtCamera>().enabled = true;
					}
				}
				else
				{
					UIItems[u].text.transform.parent.gameObject.SetActive(false);
				}
			}
			else
			{
				
				if (stack != null)
				{

					UIItems[u].text.transform.parent.gameObject.SetActive(true);
					UIItems[u].text.text = stack.items.Count.ToString();
					stack.text.text = stack.items.Count.ToString();
					stack.text.transform.SetParent(stack.items[stack.items.Count - 1].transform, false);
				}
				else
				{
					UIItems[u].text.transform.parent.gameObject.SetActive(false);
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
					CheckForChangableMoney();
				}
			}
		}
		if(stackExist == false)
		{
			ItemStack newItemStack = new ItemStack();
			itemStacks.Add(newItemStack);
			newItemStack.items.Add(item);
			newItemStack.itemType = item.itemType;
			//UpdateItemDestinations();
			if(stackTextPrefab != null)
			newItemStack.text = Instantiate(stackTextPrefab, item.transform, false).GetComponent<TextMeshPro>();
			
			added = true;
			
		}
		return added;
	}

	public bool TryPay(int amount, Vector3 payPos)
	{
		ItemStack moneyStack = GetItemStack(ItemType.Money);
		if (moneyStack == null)
		{
			return false;
		}
		else
		{
			if(GetAmountOfItemstack(moneyStack) >= amount)
			{
				StartCoroutine(PayMoney(amount, moneyStack, payPos));
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	IEnumerator PayMoney(int amount, ItemStack stack, Vector3 payPos)
	{
		List<Item> itemsToLerp = new List<Item>();
		for(int i = stack.items.Count - 1; i >= 0; i--)
		{
			amount -= stack.items[i].amount;
			
			itemsToLerp.Add(stack.items[i]);
			stack.items.RemoveAt(i);
			if (amount <= 0)
			{
				break;
			}
		}
		if (stack.items.Count == 0)
		{
			Destroy(stack.text.gameObject);
			itemStacks.Remove(stack);
		}
		RefreshItemUI();
		float waitTime = 1 / itemsToLerp.Count;
		//Lerp Items
		while(itemsToLerp.Count > 0)
		{
			itemsToLerp[itemsToLerp.Count - 1].transform.parent = null;
			StartCoroutine(LerpItemToPos(itemsToLerp[itemsToLerp.Count - 1], payPos));
			itemsToLerp.RemoveAt(itemsToLerp.Count - 1);

			yield return new WaitForSeconds(waitTime);
		}

		//get change
		amount = Mathf.Abs(amount);
		while (amount > 0)
		{
			Item itemToAdd = LevelSystem.SpawnMoneyAtPosition(ref amount, payPos);
			tryAddItem(itemToAdd);
			itemToAdd.pickedUp = true;
			RefreshItemUI();
			itemToAdd.transform.parent = itemParent;
			//Play item sound
			if (itemSoundSource != null)
			{
				itemSoundSource.PlayOneShot(pickUpSounds[Random.Range(0, pickUpSounds.Count)]);
			}
			UpdateItemDestinations();
			yield return new WaitForSeconds(0.05f);
		}
		CheckForChangableMoney();
	}

	void CheckForChangableMoney()
	{
		UpdateItemDestinations();
		ItemStack stack = itemStacks.Find(i => i.itemType == ItemType.Money);
		if(stack != null)
		{
			List<int> usedPowsOf10 = new List<int>();
			for(int a = 0; a < stack.items.Count; a++)
			{
				bool used = false;
				for(int b = 0; b < usedPowsOf10.Count; b++)
				{
					if(stack.items[a].amount == Mathf.Pow(10, b))
					{
						usedPowsOf10[b]++;
						used = true;
					}
					
				}
				if(!used)
				{
					int exp = (int)Mathf.Log10(stack.items[a].amount);
					print("exp " + exp);
					for (int c = usedPowsOf10.Count; c < exp; c++)
					{
						usedPowsOf10.Add(0);
					}
					usedPowsOf10.Add(1);
				}
				
			}

			for(int d = 0; d < usedPowsOf10.Count; d++)
			{
				if(LevelSystem.instance.moneyPrefabsM10.Count > d + 1)
					if(usedPowsOf10[d] >= 10)
					{
						 List<Item> listOfMarkedItems = stack.items.FindAll(s => s.amount == Mathf.Pow(10, d));
						for(int e = 0; e < 10; e++)
						{
							RemoveItemFromStack(listOfMarkedItems[e], stack);
						
						}
						print("Prefab: " + d + 1);
						Item itemToAdd = Instantiate(LevelSystem.instance.moneyPrefabsM10[d + 1], transform.position, transform.rotation).GetComponent<Item>();
						itemToAdd.transform.parent = itemParent;
						itemToAdd.pickedUp = true;
						tryAddItem(itemToAdd);
						listOfMarkedItems.RemoveRange(0, 10);
					}
			}
		}
		UpdateItemDestinations();
	}

	void RemoveItemFromStack(Item item, ItemStack stack)
	{
		for(int i = 0; i < stack.items.Count; i++)
		{
			if(item == stack.items[i])
			{
				stack.items.RemoveAt(i);
				Destroy(item.gameObject);
				
				return;
			}
		}
	}

	IEnumerator LerpItemToPos(Item item, Vector3 pos)
	{
		float t = 0;
		while(t < 1)
		{
			t += Time.deltaTime;
			item.transform.position = Vector3.Lerp(item.transform.position, pos, itemLerpCurve.Evaluate(t));
			yield return null;
		}
		Destroy(item.gameObject);
	}

	

	int GetAmountOfItemstack(ItemStack stack)
	{
		int output = 0;
		for(int i = 0; i < stack.items.Count; i++)
		{
			output += stack.items[i].amount;
		}
		return output;
	}

	ItemStack GetItemStack(ItemType itemType)
	{
		
		for(int i = 0; i < itemStacks.Count; i++)
		{
			if(itemStacks[i].itemType == itemType)
			{
				return itemStacks[i];
			}
		}
		return null;
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
                    if(i.itemType == worker.itemToCarry)
                    {
                        if (!i.pickedUp && tryAddItem(i))
                        {
                            i.pickedUp = true;
                            //items.Add(i);
                           // RefreshItemUI();
							if (i.transform.parent != null)
								if (i.transform.parent.parent != null)
								{
									i.transform.parent.parent.SendMessage("RemoveItem", SendMessageOptions.DontRequireReceiver);
								}
							i.transform.parent = itemParent;
							UpdateItemDestinations();
                            c.enabled = false;
                        }
                    }
                }
                else if(!i.pickedUp)
                {
					if (tryAddItem(i))
					{
						i.pickedUp = true;
						
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

            
                if (c.gameObject.layer == 7 && c.GetComponent<DroppingZone>().enabled && ((player != null && !player.isMoving) || (worker != null && !worker.isMoving)) && dropTime <= 0)
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
                                
                                
								//Update The destination for all the other items
								UpdateItemDestinations();

								//Play Drop sound
								if(player != null)
								{
									RefreshItemUI();
									itemSoundSource.PlayOneShot(dropSounds[Random.Range(0, dropSounds.Count)]);
								}	

							//	if (items[items.Count - 1] == null)
							//   {
							//       items.RemoveAt(items.Count - 1);
							//   }

							dropTime = itemDropInterval;
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
					if(itemStacks[s].text != null)
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
		while(t < 1 && i != null)
		{
			if (worker != null)
				print("Hallo");
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
