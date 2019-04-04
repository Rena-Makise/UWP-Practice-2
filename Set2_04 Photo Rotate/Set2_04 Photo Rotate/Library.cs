using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

public class Library
{
    // 회전 각도를 담기 위한 int
    private int _angle;
    // 사진 파일을 담거나 가져오기 위한 StorageFile
    private StorageFile _file;
    private WriteableBitmap _bitmap;

    // 지원하는 회전 각도를 담은 Dictionary
    private readonly Dictionary<int, BitmapRotation> rotation_angles =
        new Dictionary<int, BitmapRotation>()
    {
        { 0, BitmapRotation.None },
        { 90,  BitmapRotation.Clockwise90Degrees },
        { 180,  BitmapRotation.Clockwise180Degrees },
        { 270, BitmapRotation.Clockwise270Degrees },
        { 360, BitmapRotation.None }
    };
    private const string file_extension = ".jpg";

    private async Task<WriteableBitmap> ReadAsync()
    {
        // 파일로부터 IRandomAccessStream를 가져오고
        using (IRandomAccessStream stream = await _file.OpenAsync(FileAccessMode.ReadWrite))
        {
            //BitmapDecoder로 이미지를 가져온다
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(BitmapDecoder.JpegDecoderId, stream);
            uint width = decoder.PixelWidth;
            uint height = decoder.PixelHeight;
            if (_angle % 180 != 0)
            {
                width = decoder.PixelHeight;
                height = decoder.PixelWidth;
            }
            // 그런 다음 사진을 조작하여 변형을 반영한다.
            BitmapTransform transform = new BitmapTransform
            {
                Rotation = rotation_angles[_angle]
            };
            // PixelDataProvider를 이용하여 사진을 회전
            PixelDataProvider data = await decoder.GetPixelDataAsync(
            BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, transform,
            ExifOrientationMode.IgnoreExifOrientation, ColorManagementMode.DoNotColorManage);
            _bitmap = new WriteableBitmap((int)width, (int)height);
            // 이후 정보를 파일에 다시 기록하여 회전된 이미지를 생성
            byte[] buffer = data.DetachPixelData();
            using (Stream pixels = _bitmap.PixelBuffer.AsStream())
            {
                pixels.Write(buffer, 0, (int)pixels.Length);
            }
        }
        return _bitmap;
    }

    // 결과 이미지를 BitmapEncoder를 통해 사진에 최적회된 형식으로 인코딩하는데 사용
    private async void WriteAsync()
    {
        using (IRandomAccessStream stream = await _file.OpenAsync(FileAccessMode.ReadWrite))
        {
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
            (uint)_bitmap.PixelWidth, (uint)_bitmap.PixelHeight, 96.0, 96.0, _bitmap.PixelBuffer.ToArray());
            await encoder.FlushAsync();
        }
    }

    // FileOpenPicker로 사진을 가져오는데 사용
    // ReadAsync를 호출하여 파일을 가져오고 전달된 이미지를 소스로 설정
    public async void OpenAsync(Image display)
    {
        _angle = 0;
        try
        {
            FileOpenPicker picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(file_extension);
            _file = await picker.PickSingleFileAsync();
            if (_file != null)
            {
                display.Source = await ReadAsync();
            }
        }
        catch
        {

        }
    }

    // 회전된 사진 결과물을 저장하는데 사용되는 메소드이다.
    public async void SaveAsync()
    {
        try
        {
            FileSavePicker picker = new FileSavePicker
            {
                DefaultFileExtension = file_extension,
                SuggestedFileName = "Picture",
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeChoices.Add("Picture", new List<string>() { file_extension });
            _file = await picker.PickSaveFileAsync();
            if (_file != null)
            {
                WriteAsync();
            }
        }
        catch
        {

        }
    }

    // 회전시키는데 사용되는 값을 90도씩 증가시키고, 360도가 되면 리셋시킨다.
    // 이후 ReadAsync를 통해 이미지를 소스로 가져온다.

    public async void RotateAsync(Image display)
    {
        if (_angle == 360) _angle = 0;
        _angle += 90;
        display.Source = await ReadAsync();
    }
}