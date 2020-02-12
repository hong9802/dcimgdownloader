using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace DCimgDownloader
{

    class RequestHandler
    {
        private string filepath = System.IO.Directory.GetCurrentDirectory();
        HttpWebRequest request;
        string filename;
        private string foldername;

        public string gallRequest(string url) //디씨 게시판 request
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; //SSL/TLS문제땜에 추가된 코드줄
            string responseText;
            request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.87 Safari/537.36";
            using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
            {
                HttpStatusCode satus = resp.StatusCode;
                Stream respStream = resp.GetResponseStream();
                using (StreamReader sr = new StreamReader(respStream))
                {
                    responseText = sr.ReadToEnd();
                }
            }
            foldernameRequest(responseText);
            return responseText;
        }

        public void foldernameRequest(string responseText) //게시글 제목을 폴더명으로 씀
        {
            foldername = responseText.Substring(responseText.IndexOf("<title") + 7);
            foldername = foldername.Substring(0, foldername.IndexOf("</title>"));
            if (Regex.IsMatch(foldername, @"[^a-zA-Z0-9가-힣- --]")) //특수문자 제거(폴더명에서 오류남)
                foldername = Regex.Replace(foldername, @"[^a-zA-Z0-9가-힣- --]", "", RegexOptions.Singleline);
            folderCreate();
        }

        public void folderCreate()
        {
            filepath = filepath + "\\" + foldername;
            DirectoryInfo di = new DirectoryInfo(filepath);
            if (di.Exists == false)
                di.Create();
        }
        public void filenameMaker(int i)
        {
            filename = filename.Substring(filename.IndexOf("filename=") + 9);
            filename = filename.Substring(filename.IndexOf("."));
            filename = filename.Insert(0, filepath + "\\" + i.ToString());
        }
        public void imgRequest(string imgurl, int i)
        {
            request = (HttpWebRequest)WebRequest.Create(imgurl);
            request.Referer = imgurl;
            using (WebResponse resp = request.GetResponse())
            {
                filename = resp.Headers["Content-Disposition"];
                filenameMaker(i);
                var buff = new byte[1024];
                int pos = 0;
                int count;
                using (Stream stream = resp.GetResponseStream())
                {
                    using (var fs = new FileStream(filename, FileMode.Create))
                    {
                        do
                        {
                            count = stream.Read(buff, pos, buff.Length);
                            fs.Write(buff, 0, count);
                        } while (count > 0);
                    }
                }
            }
        }
    }
}
