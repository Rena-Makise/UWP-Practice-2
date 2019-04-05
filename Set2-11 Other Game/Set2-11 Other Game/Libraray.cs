using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

public class Library
{
    private const string app_title = "Order Game";
    private const int size = 6;
    private const int total = size * size;

    // 게임을 클리어하는데 얼마나 걸린지를 측정하기 위한 DateTime멤버
    private DateTime _timer;
    // int 항목 자체의 리스트
    private ObservableCollection<int> _list = new ObservableCollection<int>();
    private Random _random = new Random((int)DateTime.Now.Ticks);

    // 플레이어에게 MessageDialog를 띄워주기 위한 메소드
    public void Show(string content, string title)
    {
        IAsyncOperation<IUICommand> command = new MessageDialog(content, title).ShowAsync();
    }

    // Random함수를 이용하여 숫자들을 뽑는 메소드이다.
    private List<int> Select(int start, int finish, int total)
    {
        int number;
        List<int> numbers = new List<int>();
        while (numbers.Count < total)
        {
            // Start와 Finish 사이의 Random Number를 뽑는다.
            number = _random.Next(start, finish + 1);
            // 만약 숫자가 포함되어 있지 않거나, 아직 숫자가 하나도 선택되지 않았을 시에
            if ((!numbers.Contains(number)) || (numbers.Count < 1))
            {
                // 추가한다.
                numbers.Add(number);
            }
        }
        return numbers;
    }

    // 게임에서 승리하였는지를 판별하는 메소드이다.
    private bool Winner()
    {
        return _list.OrderBy(o => o).ToList().SequenceEqual(_list.ToList());
    }

    // 게임을 위해 레이아웃을 세팅하고 초기화하는 메소드이다.
    private void Layout(ref GridView grid)
    {
        _timer = DateTime.UtcNow;
        grid.IsEnabled = true;
        grid.ItemsSource = null;
        _list = new ObservableCollection<int>();
        List<int> numbers = Select(1, total, total);
        int index = 0;
        while (index < numbers.Count)
        {
            _list.Add(numbers[index]);
            index++;
        }
        grid.ItemsSource = _list;
    }

    // 게임을 시작하기 위해 레이아웃 메소드를 호출한다.
    public void New(ref GridView grid)
    {
        Layout(ref grid);
    }

    // 게임에서 승리하면 Grid를 Ordering하지 못하게 false시키는 메소드이다.
    public void Order(ref GridView grid)
    {
        if (Winner())
        {
            TimeSpan duration = (DateTime.UtcNow - _timer).Duration();
            Show($"Well Done! Completed in {duration.Hours} Hours, {duration.Minutes} Minutes and {duration.Seconds} Seconds!", app_title);
            grid.IsEnabled = false;
        }
    }
}