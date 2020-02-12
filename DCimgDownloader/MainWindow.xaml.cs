using System.Windows;
using System.Windows.Controls;

namespace DCimgDownloader
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        const int IMGMAX = 200;
        string responseText;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            RequestHandler requestHandler = new RequestHandler();
            string url;
            string tmpText;
            string imgurl;
            url = input_url.Text;
            try
            {
                responseText = requestHandler.gallRequest(url);
                for (int i = 0; i < IMGMAX; i++)
                {
                    if (responseText.IndexOf("writing_view_box") != -1)
                    {
                        if (i == 0)
                            responseText = responseText.Substring(responseText.IndexOf("writing_view_box"));
                        tmpText = responseText.Substring(responseText.IndexOf("<img src=") + 10);
                        responseText = tmpText;
                        tmpText = tmpText.Substring(0, tmpText.IndexOf("\" style="));
                    }
                    else
                    {
                        responseText = responseText.Substring(responseText.IndexOf("Dc_App_Img_" + i.ToString()));
                        tmpText = responseText.Substring(responseText.IndexOf("<img src=") + 10);
                        tmpText = tmpText.Substring(0, tmpText.IndexOf("\" style="));
                    }
                    url = tmpText;
                    imgurl = "https://image.dcinside.com/viewimage.php?" + tmpText.Substring(tmpText.IndexOf("id="));
                    requestHandler.imgRequest(imgurl, i);
                }
            }
            catch (System.ArgumentException) { }
            catch (System.UriFormatException) { result.Text = "URI ERROR"; }
            finally
            {
                if(result.Text.IndexOf("ERROR") == -1)
                    result.Text = "finish";
            }
        }

        private void Url_TextChanged(object sender, TextChangedEventArgs e)
        {
            result.Text = "";
        }
    }
}
