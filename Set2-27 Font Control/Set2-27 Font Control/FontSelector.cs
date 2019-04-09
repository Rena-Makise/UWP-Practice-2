using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace FontControl
{
    public class FontSelector : ComboBox
    {
        // 생성자
        // ComboBox에 기반한 Control의 초기설정을 진행하고 해당 DataTemplate을 제공한다.
        // 
        public FontSelector()
        {
            ItemTemplate = (DataTemplate)XamlReader.Load(
            "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
            "<TextBlock Text=\"{Binding}\" FontFamily=\"{Binding}\"/></DataTemplate>");
            // 이전에 설치한 NuGet패키지와 함께 추가된 
            // Microsoft.Graphics.Canvas.Text.CanvasTextFormat 네임 스페이스의 
            // GetSystemFontFamilies라는 Win2D.uwp 메서드를 사용하여 
            // 설치된 글꼴로 컨트롤의 ItemsSource를 설정한다.
            ItemsSource = Microsoft.Graphics.Canvas.Text.CanvasTextFormat.GetSystemFontFamilies();
        }

        // Font를 get, set하는 프로퍼티이다.
        public FontFamily Selected
        {
            get { return new FontFamily((string)SelectedItem); }
            set { SelectedValue = value.Source; }
        }
    }
}