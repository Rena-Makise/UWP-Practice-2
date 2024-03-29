﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

public class Library
{
    private const string app_title = "Recognition App";
    private const string text_file_extension = ".txt";
    private const string image_file_extension = ".jpg";

    // 두 개의 버튼이 있는 dialog를 생성하는 메소드이다.
    public async Task<bool> ConfirmAsync(string content, string title, string ok, string cancel)
    {
        bool result = false;
        MessageDialog dialog = new MessageDialog(content, title);
        dialog.Commands.Add(new UICommand(ok, new UICommandInvokedHandler((cmd) => result = true)));
        dialog.Commands.Add(new UICommand(cancel, new UICommandInvokedHandler((cmd) => result = false)));
        await dialog.ShowAsync();
        return result;
    }

    // ConfirmAsync를 이용하여 TextBox의 컨텐츠를 비울것인지를 확인하는 메소드이다.
    public async void NewAsync(Image source, TextBox target)
    {
        if (await ConfirmAsync("Create New?", app_title, "Yes", "No"))
        {
            source.Source = null;
            target.Text = string.Empty;
        }
    }

    // 파일의 내용을 TextBox의 컨텐츠로 세팅해주는 메소드이다.
    // 텍스트파일이면 FileOpenPicker로 파일을 열어 ReadTextAsync로 읽는다.
    // 또한 이미지파일이면 OcrEngine을 이용하여 안에 있는 텍스트를 인식한다.
    public async void OpenAsync(Image source, TextBox target)
    {
        try
        {
            FileOpenPicker picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeFilter.Add(text_file_extension);
            picker.FileTypeFilter.Add(image_file_extension);
            StorageFile file = await picker.PickSingleFileAsync();
            switch (file.FileType)
            {
                case text_file_extension:
                    target.Text = await FileIO.ReadTextAsync(file);
                    break;
                case image_file_extension:
                    using (IRandomAccessStream stream = await file.OpenReadAsync())
                    {
                        BitmapDecoder bitmapDecoder = await BitmapDecoder.CreateAsync(stream);
                        SoftwareBitmap softwareBitmap = await bitmapDecoder.GetSoftwareBitmapAsync(
                            BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                        OcrEngine engine = OcrEngine.TryCreateFromLanguage(new Language("en-us"));
                        OcrResult ocrResult = await engine.RecognizeAsync(softwareBitmap);
                        target.Text = ocrResult.Text;
                        stream.Seek(0);
                        BitmapImage image = new BitmapImage();
                        image.SetSource(stream);
                        source.Source = image;
                    }
                    break;
                default:
                    break;
            }
        }
        catch
        {

        }
    }

    // TextBox의 컨텐츠를 파일로 저장하는 메소드이다.
    // FileSavePicker와 WriteTextAsync로 텍스트 파일로 저장할 수 있다.
    // 또한 TextBox의 컨텐츠를 이미지로 저장할 수도 있다.
    public async void SaveAsync(Image source, TextBox target)
    {
        try
        {
            FileSavePicker picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeChoices.Add("Image File", new List<string>() { image_file_extension });
            picker.FileTypeChoices.Add("Text File", new List<string>() { text_file_extension });
            picker.DefaultFileExtension = text_file_extension;
            StorageFile file = await picker.PickSaveFileAsync();
            switch (file.FileType)
            {
                case text_file_extension:
                    await FileIO.WriteTextAsync(file, target.Text);
                    break;
                case image_file_extension:
                    using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                        RenderTargetBitmap render = new RenderTargetBitmap();
                        await render.RenderAsync(target);
                        IBuffer buffer = await render.GetPixelsAsync();
                        encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                        (uint)render.PixelWidth, (uint)render.PixelHeight, 96.0, 96.0, buffer.ToArray());
                        await encoder.FlushAsync();
                        buffer = null;
                        encoder = null;
                    }
                    break;
                default:
                    break;
            }
        }
        catch
        {

        }
    }

    // TextBox에 샘플 텍스트를 세팅해주는 메소드이다.
    public void Sample(ref TextBox target)
    {
        target.Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris non massa diam. " +
            "Nunc luctus non lorem id imperdiet. Nunc quis mi nec enim malesuada commodo mollis eget nisl. " +
            "Sed vulputate in purus eu vulputate. Quisque commodo eu odio et malesuada. Duis porttitor, " +
            "lectus ut egestas placerat, purus nisi elementum diam, congue lacinia erat lectus sit amet felis. " +
            "Proin suscipit lobortis bibendum. Aliquam erat volutpat. Nunc vitae nulla nunc.";
    }
}