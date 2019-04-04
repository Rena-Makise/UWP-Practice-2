using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

public class Library
{
    private const string app_title = "Lucky Roulette";

    private int _spins = 0;
    private int _spinValue = 0;
    private int _pickValue = 0;
    private Grid _pocket;
    private Random _random = new Random((int)DateTime.Now.Ticks);

    // MessageDialog를 표시하는 메소드이다.
    public void Show(string content, string title)
    {
        IAsyncOperation<IUICommand> command = new MessageDialog(content, title).ShowAsync();
    }

    // 숫자가 홀수인지를 판단한다.
    private bool IsOdd(int value)
    {
        return (value % 2 != 0);
    }

    // 룰렛 휠 포켓 번호와 그 번호에 적합한 색상을 TextBlock이 있는 Grid에 표시하는 메소드이다
    private void Pick()
    {
        _spins++;
        _pocket.Children.Clear();
        _spinValue = _random.Next(0, 36);
        Color fill = Colors.Transparent;
        if (_spinValue >= 1 && _spinValue <= 10 || _spinValue >= 19 && _spinValue <= 28)
        {
            fill = IsOdd(_spinValue) ? Colors.Black : Colors.DarkRed;
        }
        else if (_spinValue >= 11 && _spinValue <= 18 || _spinValue >= 29 && _spinValue <= 36)
        {
            fill = IsOdd(_spinValue) ? Colors.DarkRed : Colors.Black;
        }
        else if (_spinValue == 0)
        {
            fill = Colors.DarkGreen;
        }
        Grid container = new Grid()
        {
            Height = 200,
            Width = 180,
            BorderBrush = new SolidColorBrush(Colors.Black),
            BorderThickness = new Thickness(5),
            Background = new SolidColorBrush(fill)
        };
        TextBlock text = new TextBlock()
        {
            Foreground = new SolidColorBrush(Colors.White),
            FontSize = 120,
            Text = _spinValue.ToString(),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        container.Children.Add(text);
        _pocket.Children.Add(container);
        if (_spinValue == _pickValue) // Check Win
        {
            Show($"Spin {_spins} matched {_spinValue}", app_title);
            _spins = 0;
        }
    }

    // 포켓에 메인 레이아웃이 될 그리드를 생성하고 StackPanel에 콤보박스를 생성한다.
    private void Layout(ref Grid grid)
    {
        grid.Children.Clear();
        _spins = 0;
        List<int> values = Enumerable.Range(0, 36).ToList();
        _pocket = new Grid()
        {
            Height = 200,
            Width = 180,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        ComboBox combobox = new ComboBox()
        {
            Margin = new Thickness(10),
            HorizontalAlignment = HorizontalAlignment.Center,
            ItemsSource = values,
            SelectedIndex = 0,
        };
        combobox.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
        {
            _pickValue = (int)combobox.SelectedValue;
        };
        StackPanel panel = new StackPanel();
        panel.Children.Add(_pocket);
        panel.Children.Add(combobox);
        grid.Children.Add(panel);
    }

    // Layout을 호출하여 생성
    public void New(Grid grid)
    {
        Layout(ref grid);
    }

    // 게임이 시작되지 않았으면 레이아웃을 생성해주고
    // Pick을 호출
    public void Pick(Grid grid)
    {
        if (_pocket == null) Layout(ref grid);
        Pick();
    }
}