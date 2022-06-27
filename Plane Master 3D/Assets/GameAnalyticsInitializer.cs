using GameAnalyticsSDK;
using UnityEngine;

public class GameAnalyticsInitializer : MonoBehaviour
{
	private void Start()
	{
		GameAnalytics.Initialize();
	}
}
