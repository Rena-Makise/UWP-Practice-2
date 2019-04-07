using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Compression;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

public class Library
{
    private const string app_title = "Compression App";
    private const string text_file_extension = ".txt";
    private const string compressed_file_extension = ".compressed";
    private readonly CompressAlgorithm compression_algorithm = CompressAlgorithm.Lzms;

    // MessageDialog를 보여주는 메소드이다.
    public void Show(string content, string title)
    {
        IAsyncOperation<IUICommand> command = new MessageDialog(content, title).ShowAsync();
    }

    // 두 개의 버튼이 있는 dialog를 생성하는데 사용되는 메소드이다.
    public async Task<bool> ConfirmAsync(string content, string title, string ok, string cancel)
    {
        bool result = false;
        MessageDialog dialog = new MessageDialog(content, title);
        dialog.Commands.Add(new UICommand(ok, new UICommandInvokedHandler((cmd) => result = true)));
        dialog.Commands.Add(new UICommand(cancel, new UICommandInvokedHandler((cmd) => result = false)));
        await dialog.ShowAsync();
        return result;
    }

    // ConfirmAsync 메소드를 사용하여 새로운 문서를 만들 것인지를 확인하는 메소드이다.
    public async void NewAsync(TextBox display)
    {
        if (await ConfirmAsync("Create New?", app_title, "Yes", "No"))
        {
            display.Text = string.Empty;
        }
    }

    // 파일의 내용을 TextBox의 내용으로 설정하는 메소드이다.
    // FileOpenPicker를 이용하여 ReadTextAsync로 열린다
    // 또한 Decompressor을 이용하여 압축된 파일을 풀 수도 있다.
    public async void OpenAsync(TextBox display)
    {
        try
        {
            FileOpenPicker picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeFilter.Add(text_file_extension);
            picker.FileTypeFilter.Add(compressed_file_extension);
            StorageFile file = await picker.PickSingleFileAsync();
            switch (file.FileType)
            {
                case text_file_extension:
                    display.Text = await FileIO.ReadTextAsync(file);
                    break;
                case compressed_file_extension:
                    using (MemoryStream stream = new MemoryStream())
                    using (IInputStream input = await file.OpenSequentialReadAsync())
                    using (Decompressor decompressor = new Decompressor(input))
                    using (IRandomAccessStream output = stream.AsRandomAccessStream())
                    {
                        long inputSize = input.AsStreamForRead().Length;
                        ulong outputSize = await RandomAccessStream.CopyAsync(decompressor, output);
                        output.Seek(0);
                        display.Text = await new StreamReader(output.AsStream()).ReadToEndAsync();
                        Show($"Decompressed {inputSize} bytes to {outputSize} bytes", app_title);
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

    // TexBox의 내용을 파일에 저장하는 메소드이다.
    // FileSavePicker를 이용하여 WriteTextAsync를 통해 파일에 저장된다.
    // 또한 Compressor를 이용하여 파일 압축도 가능하다.
    public async void SaveAsync(TextBox display)
    {
        try
        {
            FileSavePicker picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeChoices.Add("Text File", new List<string>() { text_file_extension });
            picker.FileTypeChoices.Add("Compressed File", new List<string>() { compressed_file_extension });
            picker.DefaultFileExtension = text_file_extension;
            StorageFile file = await picker.PickSaveFileAsync();
            switch (file.FileType)
            {
                case text_file_extension:
                    await FileIO.WriteTextAsync(file, display.Text);
                    break;
                case compressed_file_extension:
                    using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(display.Text)))
                    using (IRandomAccessStream input = stream.AsRandomAccessStream())
                    using (IRandomAccessStream output = await file.OpenAsync(FileAccessMode.ReadWrite))
                    using (Compressor compressor = new Compressor(output.GetOutputStreamAt(0), compression_algorithm, 0))
                    {
                        ulong inputSize = await RandomAccessStream.CopyAsync(input, compressor);
                        bool finished = await compressor.FinishAsync();
                        ulong outputSize = output.Size;
                        Show($"Compressed {inputSize} bytes to {outputSize} bytes", app_title);
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

    // TextBox에 예제 텍스트를 세팅해주는 메소드이다.
    public void Sample(ref TextBox display)
    {
        StringBuilder text = new StringBuilder();
        for (int i = 0; i < 10; i++)
        {
            text.AppendLine("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris non massa diam. " +
            "Nunc luctus non lorem id imperdiet. Nunc quis mi nec enim malesuada commodo mollis eget nisl. " +
            "Sed vulputate in purus eu vulputate. Quisque commodo eu odio et malesuada. Duis porttitor, " +
            "lectus ut egestas placerat, purus nisi elementum diam, congue lacinia erat lectus sit amet felis. " +
            "Proin suscipit lobortis bibendum. Aliquam erat volutpat. Nunc vitae nulla nunc.\n");
        }
        display.Text = text.ToString();
    }
}