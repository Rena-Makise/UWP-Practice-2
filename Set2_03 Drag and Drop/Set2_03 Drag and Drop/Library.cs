using System;
using System.Collections.ObjectModel;
using System.Linq;

// ObservableCollection의 아이템을 표현하기 위한 클래스이다
// Library 클래스 내에서 ObservableCollection의 아이템으로써 프로퍼티로 표현된다.
public class Item
{
    public Guid Id { get; set; }
    public string Value { get; set; }
}

public class Library
{
    public ObservableCollection<Item> Items { get; set; } = new ObservableCollection<Item>();

    public void Add(string value)
    {
        Items.Add(new Item
        {
            Id = Guid.NewGuid(),
            Value = value,
        });
    }

    public void Remove(Guid id)
    {
        Item result = Items.FirstOrDefault(item => item.Id == id);
        if (result != null)
        {
            Items.Remove(result);
        }
    }
}