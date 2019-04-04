using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x412에 나와 있습니다.

namespace Set2_01_Custom_Dialog
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            // Dialog에 보여질 내용과 시스템 스타일을 사용한 TextBlock을 설정한다.
            ContentDialog dialog = new ContentDialog()
            {
                Title = "Custom Dialog",
                MaxWidth = this.ActualWidth,
                PrimaryButtonText = "Close",
                Content = new TextBlock
                {
                    Style = (Style)App.Current.Resources["SubheaderTextBlockStyle"],
                    Text = "Hello World",
                }
            };
            await dialog.ShowAsync();
        }
    }
}
