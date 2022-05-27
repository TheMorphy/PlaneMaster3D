using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class PlayerUpgradeUI
{
	[SerializeField]
	public TextMeshProUGUI levelTxt;
	[SerializeField]
	public TextMeshProUGUI prizeText;

}
public class Player : MonoBehaviour
{
	#region Speed
	[Header("Speed")]
	[Header("Upgrades")]
	
	
	[SerializeField]
	float speedStandard = 3f;
	int speedLevel = 1;
	[SerializeField]
	float speedIncrement;
	[SerializeField]
	bool upgradeSpeedPercentage;
	[Space(5)]
	//Prize
	
	int speedPrize;
	[SerializeField]
	int speedPrizeStandard;
	[SerializeField]
	float speedPrizeIncrement;
	[SerializeField]
	PlayerUpgradeUI speedUpgradeUI;

	#endregion

	#region backpack
	[Header("Backpack")]
	[SerializeField]
	int backpackStandard = 10;
	int backpackLevel = 1;
	[SerializeField]
	float backpackIncrement;
	[SerializeField]
	bool upgradeBackpackPercentage;
	[Space(5)]
	//Prize
	
	int backpackPrize;
	[SerializeField]
	int backpackPrizeStandard;
	[SerializeField]
	float backpackPrizeIncrement;
	[SerializeField]
	PlayerUpgradeUI backpackUpgradeUI;
	#endregion


	#region movement
	[Header("Attributes")]
    [SerializeField]
    float moveSpeed = 3f;
    [SerializeField]
    float moveSmooth = 0.13f, turnSmooth = 0.1f;
    [SerializeField]
    AnimationCurve joistickReplyCurve;

    [Header("Animation")]
    [SerializeField]
    float animationPlaybackspeed = 1f;

    [Header("Serializables")]
    [SerializeField]
    bool touchControls = true;
    [SerializeField]
    FloatingJoystick joistick;
    [SerializeField]
    Animator visual3D;
    [SerializeField]
    Transform cam;
	[SerializeField]
    Transform stableForward;
    Vector3 velocity, lookRotation;
    CharacterController controller;
    float inputX, inputY;
    float currentSpeed;
    [HideInInspector]
    public bool isMoving;
    bool isGrounded;
    float velY;
	public Backpack backpack;
	#endregion


	#region Player Upgrade
	void LoadLevel()
	{
		speedLevel = PlayerPrefs.GetInt("playerSpeedLevel");
		backpackLevel = PlayerPrefs.GetInt("playerBackpackLevel");
		if (speedLevel < 1)
			speedLevel = 1;
		if (backpackLevel < 1)
			backpackLevel = 1;

		SetSpeedByLevel();
		SetSpeedPrice();
		SetStorageByLevel();
		SetBackpackPrice();


	}
	void SaveLevel()
	{
		PlayerPrefs.SetInt("playerSpeedLevel", speedLevel);
		PlayerPrefs.SetInt("playerBackpackLevel", backpackLevel);
	}
	#region Speed

	public void TryUpgradePlayerSpeed()
	{
		
		if(backpack.TryPay(speedPrize, LevelSystem.instance.workersHouse.position))
		{
			//Upgrade speed
			speedLevel++;
			SaveLevel();
			SetSpeedPrice();
			SetSpeedByLevel();
		}
	}

	void SetSpeedPrice()
	{
		speedPrize = (int)(speedPrizeStandard * Mathf.Pow(1 + speedPrizeIncrement, speedLevel - 1));
		speedUpgradeUI.prizeText.text = speedPrize.ToString();
	}
	float GetSpeedByLevel(int level)
	{
		return upgradeSpeedPercentage ? speedStandard * Mathf.Pow(1 + speedIncrement, level - 1) : speedStandard + speedIncrement * (level -1);
	}
	void SetSpeedByLevel()
	{
		moveSpeed = upgradeSpeedPercentage ? speedStandard * Mathf.Pow(1 + speedIncrement, speedLevel - 1) : speedStandard + speedIncrement * (speedLevel -1);
		speedUpgradeUI.levelTxt.text = "LVL " + speedLevel;
	}






	#endregion
	#region Backpack
	public void TryUpgradeBackpack()
	{
		if (backpack.TryPay(backpackPrize, LevelSystem.instance.workersHouse.position))
		{
			//Upgrade speed
			backpackLevel++;
			SaveLevel();
			SetBackpackPrice();
			SetStorageByLevel();
		}
	}

	void SetBackpackPrice()
	{
		backpackPrize = (int)(backpackPrizeStandard * Mathf.Pow(1 + backpackPrizeIncrement, backpackLevel - 1));
		backpackUpgradeUI.prizeText.text = backpackPrize.ToString();
	}
	float GetStorageByLevel(int level)
	{
		return upgradeBackpackPercentage ? backpackStandard * Mathf.Pow(1 + backpackIncrement, level - 1) : backpackStandard + backpackIncrement * (level - 1);
	}
	void SetStorageByLevel()
	{
		backpack.stackSize = upgradeBackpackPercentage ? (int)(backpackStandard * Mathf.Pow(1 + backpackIncrement, backpackLevel - 1)) : (int)(backpackStandard + backpackIncrement * (backpackLevel - 1));
		backpackUpgradeUI.levelTxt.text = "LVL " + backpackLevel;
	}
	#endregion
	#endregion






	private void OnEnable()
    {
        controller = GetComponent<CharacterController>();
		LoadLevel();
        //stableForward = cam.GetChild(0);
    }
    void Update()
	{
		
			
		
			
		

		//touch controls
		inputY = joistick.Vertical;
        inputX = joistick.Horizontal;


        if (!touchControls)
        {
            //pc controls for debugging
            inputY = Input.GetAxisRaw("Vertical");
            inputX = Input.GetAxisRaw("Horizontal");
            currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed, moveSmooth);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed * joistickReplyCurve.Evaluate(joistick.Direction.magnitude) , moveSmooth);
        }
            if (Mathf.Max(Mathf.Abs(inputY), Mathf.Abs(inputX)) > 0)
            {
                
                Vector3 tempLookRotation = stableForward.forward * inputY + stableForward.right * inputX;
                tempLookRotation *= 180;
                lookRotation = Vector3.Lerp(lookRotation, tempLookRotation, turnSmooth);
                isMoving = true;

            }
            else
            {
                currentSpeed = Mathf.Lerp(currentSpeed, 0, moveSmooth);
                if (isMoving)
                {

                    isMoving = false;
                }
            }



		//print(stableForward.forward.magnitude + "r: " + stableForward.right.magnitude);
            velocity = stableForward.forward * inputY * Time.deltaTime * currentSpeed + stableForward.right * inputX * Time.deltaTime * currentSpeed +stableForward.up * velY;
            if(lookRotation != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(lookRotation);
            controller.Move(velocity);
            visual3D.SetFloat("Speed", currentSpeed);
        
        

    }

    private void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(transform.position, 0.3f, 3);
        if(!isGrounded)
        {
            velY += -9.81f * Time.fixedDeltaTime; 
        }
        else
        {
            velY = -2f;
        }
    }
}
