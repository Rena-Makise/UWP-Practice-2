using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace JsonfileApp
{
    public class Music
    {
        // 이벤트 핸들러와 해당 이벤트에 대한 트리거
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _album;
        private string _artist;
        private string _genre;

        public Music() { Id = Guid.NewGuid().ToString(); }

        public string Id { get; set; }
        public string Album { get { return _album; } set { _album = value; NotifyPropertyChanged(); } }
        public string Artist { get { return _artist; } set { _artist = value; NotifyPropertyChanged(); } }
        public string Genre { get { return _genre; } set { _genre = value; NotifyPropertyChanged(); } }
    }

    public class Library
    {
        private const string file_name = "file.json";

        private StorageFile _file;

        public static ObservableCollection<Music> Collection { get; private set; } = new ObservableCollection<Music>();

        private async void Read()
        {
            try
            {
                // GetFileAsync를 이용하여 파일에 대한 정보를 가져온다.
                _file = await ApplicationData.Current.LocalFolder.GetFileAsync(file_name);
                using (Stream stream = await _file.OpenStreamForReadAsync())
                {
                    // DataContractJsonSerializer와 ReadObject를 이용하여 저장된 데이터를 가져와서
                    // ObservableCollection를 읽어낸다.
                    Collection = (ObservableCollection<Music>)
                        new DataContractJsonSerializer(typeof(ObservableCollection<Music>))
                        .ReadObject(stream);
                }
            }
            catch
            {
            }
        }

        private async void Write()
        {
            try
            {
                // CreateFileAsync를 이용하여 파일을 생성한다.
                _file = await ApplicationData.Current.LocalFolder.CreateFileAsync(file_name,
                    CreationCollisionOption.ReplaceExisting);
                using (Stream stream = await _file.OpenStreamForWriteAsync())
                {
                    // DataContractJsonSerializer와 WriteObject를 사용하여 저장된 데이터를 쓴다.
                    new DataContractJsonSerializer(typeof(ObservableCollection<Music>))
                        .WriteObject(stream, Collection);
                }
            }
            catch
            {
            }
        }

        public Library()
        {
            Read();
        }

        // Music의 ObservableCollection에서 해당 항목을 삽입한다.
        public void Add(FlipView display)
        {

            Collection.Insert(0, new Music());
            display.SelectedIndex = 0;
        }

        public void Save()
        {
            Write();
        }

        // FlipView의 SelectedItem을 ObservableCollection에서 삭제한다.
        public void Remove(FlipView display)
        {
            if (display.SelectedItem != null)
            {
                Collection.Remove(Collection.Where(w => w.Id ==
                ((Music)display.SelectedValue).Id).Single());
                Write();
            }
        }

        // 저장소에서 해당 파일을 삭제한다.
        public async void Delete(FlipView display)
        {
            try
            {
                Collection = new ObservableCollection<Music>();
                display.ItemsSource = Collection;
                _file = await ApplicationData.Current.LocalFolder.GetFileAsync(file_name);
                await _file.DeleteAsync();
            }
            catch
            {

            }
        }
    }
}