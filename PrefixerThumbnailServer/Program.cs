using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Text;
using System.Xml.Linq;

namespace PrefixerThumbnailServer
{
    class Program
    {
        private static readonly Random random = new Random();

        private static readonly HttpClient client = new HttpClient();

        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder result = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString();
        }

        public static string Postimage(int uid, string rendtype, string base64image)
        {
            WebClient client = new WebClient();
            Console.WriteLine("Trying to send the render to the servers");
            string uploadurl = "http://www.prefixr.me/api/internal/uplimg.php?apikey=verysecretapikeyreal&uid=" + uid + "&typeofasset=" + rendtype;
            client.Headers.Add("Content-Type", "application/octet-stream");
            using (Stream fileStream = File.OpenRead(uid + ".txt"))
            using (Stream requestStream = client.OpenWrite(new Uri(uploadurl), "POST"))
            {
                fileStream.CopyTo(requestStream);
            }

            client.DownloadString("http://www.prefixr.me/api/internal/doFinish.php?renderid=" + uid + "&type=" + rendtype + "&apiKey=apikeyfrfrfrfrfr&version=1");
            File.Delete(uid + ".txt");
            Console.WriteLine($"DONE! Uploaded {rendtype} of user/asset id {uid}");
            return "posted";
        }

        public static async Task<string> dorender(int uid, string rendtype)
        {
            Console.WriteLine($"RENDER REQUEST USER/ASSET {uid}, TYPE {rendtype}");
            string randstring = GenerateRandomString(10);
            string soapXml = "";
            if (rendtype == "Avatar")
            {
                soapXml = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:SOAP-ENC=""http://schemas.xmlsoap.org/soap/encoding/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:ns2=""http://roblox.com/RCCServiceSoap"" xmlns:ns1=""http://roblox.com/"" xmlns:ns3=""http://roblox.com/RCCServiceSoap12"">
            <SOAP-ENV:Body>
                <ns1:OpenJob>
                    <ns1:job>
                        <ns1:id>{randstring}</ns1:id>
                        <ns1:expirationInSeconds>15</ns1:expirationInSeconds>
                        <ns1:category>1</ns1:category>
                        <ns1:cores>321</ns1:cores>
                    </ns1:job>
                    <ns1:script>
                        <ns1:name>Script</ns1:name>
                        <ns1:script>
{{
  ""Mode"": ""Thumbnail"",
  ""Settings"": {{
    ""Type"": ""Avatar_R15_Action"",
    ""PlaceId"": 1,
    ""UserId"": {uid},
    ""BaseUrl"": ""prefixr.me"",
    ""MatchmakingContextId"": 1,
    ""Arguments"": [
      ""https://prefixr.me"",
      ""https://prefixr.me/v1.1/avatar-fetch?userId={uid}"",
      ""PNG"",
      768,
      768
    ]
  }},
  ""Arguments"": {{
    ""MachineAddress"": ""127.0.0.1""
  }}
        }}
                        </ns1:script>
                    </ns1:script>
                </ns1:OpenJob>
            </SOAP-ENV:Body>
        </SOAP-ENV:Envelope>";
            }

            if(rendtype == "Headshot")
            {
                soapXml = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:SOAP-ENC=""http://schemas.xmlsoap.org/soap/encoding/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:ns2=""http://roblox.com/RCCServiceSoap"" xmlns:ns1=""http://roblox.com/"" xmlns:ns3=""http://roblox.com/RCCServiceSoap12"">
            <SOAP-ENV:Body>
                <ns1:OpenJob>
                    <ns1:job>
                        <ns1:id>{randstring}</ns1:id>
                        <ns1:expirationInSeconds>15</ns1:expirationInSeconds>
                        <ns1:category>1</ns1:category>
                        <ns1:cores>321</ns1:cores>
                    </ns1:job>
                    <ns1:script>
                        <ns1:name>Script</ns1:name>
                        <ns1:script>
{{
  ""Mode"": ""Thumbnail"",
  ""Settings"": {{
    ""Type"": ""Closeup"",
    ""PlaceId"": 1,
    ""UserId"": {uid},
    ""BaseUrl"": ""prefixr.me"",
    ""MatchmakingContextId"": 1,
    ""Arguments"": [
      ""https://prefixr.me"",
      ""https://prefixr.me/v1.1/avatar-fetch?userId={uid}"",
      ""PNG"",
      768,
      768,
      true,
      40,
      100,
      0,
      0
    ]
  }},
  ""Arguments"": {{
    ""MachineAddress"": ""127.0.0.1""
  }}
}}
                        </ns1:script>
                    </ns1:script>
                </ns1:OpenJob>
            </SOAP-ENV:Body>
        </SOAP-ENV:Envelope>";
            }

            if (rendtype == "Hat")
            {
                soapXml = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:SOAP-ENC=""http://schemas.xmlsoap.org/soap/encoding/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:ns2=""http://roblox.com/RCCServiceSoap"" xmlns:ns1=""http://roblox.com/"" xmlns:ns3=""http://roblox.com/RCCServiceSoap12"">
            <SOAP-ENV:Body>
                <ns1:OpenJob>
                    <ns1:job>
                        <ns1:id>{randstring}</ns1:id>
                        <ns1:expirationInSeconds>15</ns1:expirationInSeconds>
                        <ns1:category>1</ns1:category>
                        <ns1:cores>321</ns1:cores>
                    </ns1:job>
                    <ns1:script>
                        <ns1:name>Script</ns1:name>
                        <ns1:script>
{{
""Mode"": ""Thumbnail"",
""Settings"": {{
""Type"": ""Hat"",
""PlaceId"": 1,
""UserId"": 1,
""BaseUrl"": ""prefixr.me"",
""MatchmakingContextId"": 1,
""Arguments"": [""https://prefixr.me/asset/?id={uid}"",""PNG"", 720, 720, ""https://prefixr.me/""]
}},
""Arguments"": {{
""MachineAddress"": ""127.0.0.1""
}}
}}
                        </ns1:script>
                    </ns1:script>
                </ns1:OpenJob>
            </SOAP-ENV:Body>
        </SOAP-ENV:Envelope>";
            }

            if (rendtype == "gameicon")
            {
                soapXml = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:SOAP-ENC=""http://schemas.xmlsoap.org/soap/encoding/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:ns2=""http://roblox.com/RCCServiceSoap"" xmlns:ns1=""http://roblox.com/"" xmlns:ns3=""http://roblox.com/RCCServiceSoap12"">
            <SOAP-ENV:Body>
                <ns1:OpenJob>
                    <ns1:job>
                        <ns1:id>{randstring}</ns1:id>
                        <ns1:expirationInSeconds>15</ns1:expirationInSeconds>
                        <ns1:category>1</ns1:category>
                        <ns1:cores>321</ns1:cores>
                    </ns1:job>
                    <ns1:script>
                        <ns1:name>Script</ns1:name>
                        <ns1:script>
{{
""Mode"": ""Thumbnail"",
""Settings"": {{
""Type"": ""Place"",
""PlaceId"": {uid},
""UserId"": 1,
""BaseUrl"": ""prefixr.me"",
""MatchmakingContextId"": 1,
""Arguments"": [""https://prefixr.me/asset/?id={uid}&amp;gamefetch=true"", ""PNG"", 720, 720, ""https://prefixr.me"", {uid}]
}},
""Arguments"": {{
""MachineAddress"": ""127.0.0.1""
}}
}}
                        </ns1:script>
                    </ns1:script>
                </ns1:OpenJob>
            </SOAP-ENV:Body>
        </SOAP-ENV:Envelope>";
            }

            if (rendtype == "game")
            {
                soapXml = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:SOAP-ENC=""http://schemas.xmlsoap.org/soap/encoding/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:ns2=""http://roblox.com/RCCServiceSoap"" xmlns:ns1=""http://roblox.com/"" xmlns:ns3=""http://roblox.com/RCCServiceSoap12"">
            <SOAP-ENV:Body>
                <ns1:OpenJob>
                    <ns1:job>
                        <ns1:id>{randstring}</ns1:id>
                        <ns1:expirationInSeconds>15</ns1:expirationInSeconds>
                        <ns1:category>1</ns1:category>
                        <ns1:cores>321</ns1:cores>
                    </ns1:job>
                    <ns1:script>
                        <ns1:name>Script</ns1:name>
                        <ns1:script>
{{
""Mode"": ""Thumbnail"",
""Settings"": {{
""Type"": ""Place"",
""PlaceId"": {uid},
""UserId"": 1,
""BaseUrl"": ""prefixr.me"",
""MatchmakingContextId"": 1,
""Arguments"": [""https://prefixr.me/asset/?id={uid}&amp;gamefetch=true"", ""PNG"", 1280, 720, ""https://prefixr.me"", {uid}]
}},
""Arguments"": {{
""MachineAddress"": ""127.0.0.1""
}}
}}
                        </ns1:script>
                    </ns1:script>
                </ns1:OpenJob>
            </SOAP-ENV:Body>
        </SOAP-ENV:Envelope>";
            }
            string url = "http://localhost:64989"; // Replace with your SOAP service URL
                Console.WriteLine("Trying to send soap request");
                try
                {
                    // Create HTTP content based on the XML request
                    var content = new StringContent(soapXml, Encoding.UTF8, "text/xml");

                    // Set the appropriate headers
                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "text/xml; charset=utf-8");
                    content.Headers.Add("SOAPAction", "http://tempuri.org/YourSoapMethod"); // Adjust SOAPAction as required

                    // Send the request
                    HttpResponseMessage response = await client.PostAsync(url, content);

                    // Check for success status code
                    response.EnsureSuccessStatusCode();
                    Console.WriteLine("Yea i guess it worked");
                    // Read and parse the response content
                    string result = await response.Content.ReadAsStringAsync();
                    XDocument doc = XDocument.Parse(result);

                    // Extract the value of ns1:table
                    XNamespace ns1 = "http://prefixr.me/";
                    string tableValue = doc.Descendants(ns1 + "value").FirstOrDefault()?.Value;
                    File.WriteAllText(uid + ".txt", tableValue);
                    Console.WriteLine("Rendered");
                    Postimage(uid, rendtype, tableValue);
                    return "yay";
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ok something went wrong");
                    Console.WriteLine(ex.Message);
                    return "yay";
                }
            return "yay";
        }

        static void Main(string[] args)
        {
            try
            {
                WebClient wClient = new WebClient();
                while (true)
                {
                    if (wClient.DownloadString("http://www.prefixr.me/api/internal/getList.php") != "no-render")
                    {
                        dynamic quejson = JsonConvert.DeserializeObject(wClient.DownloadString("http://www.prefixr.me/api/internal/getList.php"));
                        dorender(Convert.ToInt32(quejson.userid), Convert.ToString(quejson.type));
                        Thread.Sleep(10000);
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }

            }
            catch (Exception)
            {
                Console.ReadKey();
                MessageBox.Show("some shit went wrong");
            }
        }
    }
}
