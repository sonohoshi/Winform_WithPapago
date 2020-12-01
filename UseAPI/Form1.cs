using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Use_API_Tutorial
{
    public struct JSONMessage
    {
        [JsonInclude] public Message message;
    }
    public class Message
    {
        public class Result
        {
            [JsonInclude] public string srcLangType{ get; set; }
            [JsonInclude] public string tarLangType{ get; set; }
            [JsonInclude] public string translatedText{ get; set; }
            [JsonInclude] public string engineType{ get; set; }
            [JsonInclude] public string pivot{ get; set; }
        }
        
        [JsonInclude] public string @type{ get; set; }
        [JsonInclude] public string @service{ get; set; }
        [JsonInclude] public string @version{ get; set; }
        [JsonInclude] public Result result{ get; set; }
    }
    
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            StreamReader sr = new StreamReader(new FileStream("IDresource.txt", FileMode.Open));

            string file = sr.ReadToEnd();
            
            clientID = file.Split('\n')[0];
            secretID = file.Split('\n')[1];
            
            sr.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Text = Request(textBox1.Text.Replace('\n', ' '));
        }

        private string Request(string kor)
        {
            string url = "https://openapi.naver.com/v1/papago/n2mt";
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            
            request.Headers.Add("X-Naver-Client-Id", clientID);
            request.Headers.Add("X-Naver-Client-Secret", secretID);
            request.Method = "POST";

            byte[] byteDataParams = Encoding.UTF8.GetBytes("source=ko&target=en&text=" + kor);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteDataParams.Length;
            Stream st = request.GetRequestStream();
            st.Write(byteDataParams, 0, byteDataParams.Length);
            st.Close();
            
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            string text = reader.ReadToEnd();
            stream.Close();
            response.Close();
            reader.Close();

            var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true,};

            JSONMessage msg = JsonSerializer.Deserialize<JSONMessage>(text, options);
            
            return msg.message.result.translatedText;
        }
    }
}