using FarmEase_230508.Maui.ViewModels;
using Plugin.LocalNotification;

namespace FarmEase_230508.Maui.Views;

public partial class PushNotifyDemoPage : ContentPage
{
	public PushNotifyDemoPage()
	{
		InitializeComponent();
		BindingContext = new PushNotifyDemoViewModel();
	}

    private void OnButtonClicked(object sender, EventArgs e) {
        var request = new NotificationRequest {
            NotificationId = 1337,
            Title = "Subscribe to my channel",
            Subtitle = "Hello",
            Description = "It's me",
            BadgeNumber = 42,
            Schedule = new NotificationRequestSchedule {
                NotifyTime = DateTime.Now.AddSeconds(5),
                NotifyRepeatInterval = TimeSpan.FromDays(1),
            }
        };

        LocalNotificationCenter.Current.Show(request);
    }
}