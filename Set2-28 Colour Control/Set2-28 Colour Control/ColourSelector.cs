using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace ColourControl
{
    public class ColourSelector : ComboBox
    {
        // 단일 항목을 나타내는 클래스
        public class Colour
        {
            public string Name { get; set; }
            public Color Value { get; set; }
        }

        // 색상의 이름을 정규화하여 대문자로 분리하여 표시하는 메소드
        // ex. CornflowerBlue를 Cornflower Blue로 표시한다.
        private static IEnumerable<string> SplitCaptial(string text)
        {
            Regex regex = new Regex(@"\p{Lu}\p{Ll}*");
            foreach (Match match in regex.Matches(text))
            {
                // 현재 메소드, 즉 GetEnumerable()의 실행을 일시 정지시켜놓고 호출자에게 결과를 반환한다.
                // 메소드가 다시 호출되면, 일시 정지된 실행을 복구하여 yield return문을 만날 때까지 나머지 작업을 실행한다.
                yield return match.Value;
            }
        }

        // 모든 색상 멤버들을 나타내는 Colour List가 있고, 
        // 위의 SplitCaptial를 이용하여 색상에 사용되는 레이블을 만든다.
        private List<Colour> _colours = typeof(Colors).GetRuntimeProperties().Select(c => new Colour
        {
            Value = (Color)c.GetValue(null),
            Name = string.Join(" ", SplitCaptial(c.Name))
        }).ToList();

        // ColourSelector 생성자는 ComboBox를 기반으로 하고 
        // ItemsSource를 Colors 세트로 설정하는 Control의 ItemTemplate을 설정한다.
        public ColourSelector()
        {
            ItemTemplate = (DataTemplate)XamlReader.Load(
            "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
            "<StackPanel Orientation=\"Horizontal\">" +
            "<Rectangle Width=\"20\" Height=\"15\" Margin=\"5,0\"><Rectangle.Fill>" +
            "<SolidColorBrush Color=\"{Binding Value}\"/></Rectangle.Fill></Rectangle>" +
            "<TextBlock VerticalAlignment=\"Center\" Text=\"{Binding Name}\"/>" +
            "</StackPanel></DataTemplate>");
            SelectedValuePath = "Value";
            ItemsSource = _colours;
        }

        // 색을 가져오거나 설정하는 프로퍼티이다.
        public Color Selected
        {
            get { return ((Colour)SelectedItem).Value; }
            set { SelectedItem = (_colours.Single(w => w.Value == value)); }
        }
    }
}