using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;

public class MobileNotifications : MonoBehaviour
{
    void Start()
    {
		AndroidNotificationCenter.CancelAllDisplayedNotifications();

		var channel = new AndroidNotificationChannel()
		{
			Id = "channel_id",
			Name = "Default Channel",
			Importance = Importance.Default,
			Description = "Generic notifications",
		};
		AndroidNotificationCenter.RegisterNotificationChannel(channel);

		var notification = new AndroidNotification();
		notification.Title = "👨‍✈️ Your pilots are waiting for you!";
		notification.Text = "Log in to the game and help them out now! 🔧";
		notification.LargeIcon = "app_icon_large";
		notification.FireTime = System.DateTime.Now.AddDays(1);

		var id = AndroidNotificationCenter.SendNotification(notification, "channel_id");

		if (AndroidNotificationCenter.CheckScheduledNotificationStatus(id) == NotificationStatus.Scheduled)
		{
			AndroidNotificationCenter.CancelAllNotifications();
			AndroidNotificationCenter.SendNotification(notification, "channel_id");
		}
	}
}
