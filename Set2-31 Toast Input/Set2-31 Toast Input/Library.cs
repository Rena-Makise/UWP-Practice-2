using System;
using System.Text;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

public class Library
{
    private Random _random = new Random((int)DateTime.Now.Ticks);
    private ToastNotifier _notifier = ToastNotificationManager.CreateToastNotifier();

    // 지정된 시간에 커스터마이즈된 템플릿을 기반으로 ScheduledToastNotification을 만드는 메소드
    // AddToSchedule을 사용하여 일부 텍스트를 입력으로 허용한다.
    public void Add(TimeSpan occurs)
    {
        DateTime when = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
            occurs.Hours, occurs.Minutes, occurs.Seconds);
        if (when > DateTime.Now)
        {
            StringBuilder template = new StringBuilder();
            template.Append("<toast><visual version='2'><binding template='ToastText02'><text id='2'>Enter Message:</text></binding></visual>");
            template.Append("<actions><input id='message' type='text'/><action activationType='foreground' content='Ok' arguments='ok'/></actions></toast>");
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(template.ToString());
            ScheduledToastNotification toast = new ScheduledToastNotification(xml, when)
            {
                Id = _random.Next(1, 100000000).ToString()
            };
            _notifier.AddToSchedule(toast);
        }
    }
}