using System;
using System.Text;
using System.Windows;
using RTEA_Library;

namespace Interface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Encode_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(EncodedTextBox.Text))
                return;
            string encodedText = EncodedTextBox.Text;
            string key = KeyTextBox.Text;
            Encoding encoding = Encoding.Default;

            EncodedBytesTextBox.Text = String.Empty;
            
            byte[] encodingBytes = encoding.GetBytes(encodedText);
            for (int i = 0; i < encodingBytes.Length; i++)
            {
                if (i != encodingBytes.Length - 1)
                {
                    EncodedBytesTextBox.Text += encodingBytes[i].ToString() + "-";
                }

                else
                {
                    EncodedBytesTextBox.Text += encodingBytes[i].ToString();
                }
            }

            RTEA rtea = new RTEA();
            byte[] result;

            DecodedBytesTextBox.Text = String.Empty;
            DecodedTextBox.Text = String.Empty;

            try
            {
                result = rtea.Encode(encodedText, key);
            }
            catch (ArgumentException exception)
            {
                DecodedTextBox.Text = exception.Message;
                DecodedBytesTextBox.Text = String.Empty;
                Status.Content = "Ошибка!";
                return;
            }

            DecodedTextBox.Text = encoding.GetString(result);
            for (int i = 0; i<result.Length; i++)
            {
                if (i != result.Length - 1)
                {
                    DecodedBytesTextBox.Text += result[i].ToString() + "-";
                }
                    
                else
                {
                    DecodedBytesTextBox.Text += result[i].ToString();
                }
            }

            Status.Content = "Зашифровано!";
        }

        private void Decode_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(DecodedBytesTextBox.Text))
                return;
            string [] decodedBytesString = DecodedBytesTextBox.Text.Split('-');
            string key = KeyTextBox.Text;
            Encoding encoding = Encoding.Default;

            byte[] decodedBytes = new byte[decodedBytesString.Length];
            for (int i = 0; i<decodedBytes.Length;i++)
            {
                decodedBytes[i] = Convert.ToByte(decodedBytesString[i]);
            }

            DecodedTextBox.Text = encoding.GetString(decodedBytes);


            EncodedBytesTextBox.Text = String.Empty;
            EncodedTextBox.Text = String.Empty;
            RTEA rtea = new RTEA();

            string result;
            try
            {
                result = rtea.Decode(decodedBytes, key);
            }
            catch (ArgumentException exception)
            {
                EncodedTextBox.Text = exception.Message;
                EncodedBytesTextBox.Text = String.Empty;
                Status.Content = "Ошибка!";
                return;
            }

            EncodedTextBox.Text = result;

            EncodedBytesTextBox.Text = String.Empty;
            byte[] resultBytes = encoding.GetBytes(result);
            for (int i = 0; i < result.Length; i++)
            {
                if (i != result.Length - 1)
                {
                    EncodedBytesTextBox.Text += resultBytes[i].ToString() + "-";
                }

                else
                {
                    EncodedBytesTextBox.Text += resultBytes[i].ToString();
                }
            }

            Status.Content = "Расшифровано!";
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            Info info = new Info();
            info.ShowDialog();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            About about=new About();
            about.ShowDialog();
        }
    }
}
