using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace NodeCS.Helpers
{
    public static class NetworkUtilities
    {
        public static IPAddress GetLocalAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip;
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static byte[] ToBytes(this string content) =>
            Encoding.UTF8.GetBytes(content);

        public static string ToUTFString(this byte[] content) =>
            Encoding.UTF8.GetString(content);

        public static void Send(this HttpListenerResponse response, string message)
        {
            try
            {
                using (var writer = new StreamWriter(response.OutputStream))
                    writer.Write(message);
            }
            catch { }
        }

        public static Dictionary<string, string[]> GetHeaders(this HttpListenerRequest request)
        {
            Dictionary<string, string[]> keyvalue = new Dictionary<string, string[]>();
            var keys = request.Headers.AllKeys;
            for(int i = 0; i < keys.Length; i++)
                keyvalue.Add(keys[i], request.Headers.GetValues(i));
            return keyvalue;
        }


        public static string Read(this HttpListenerRequest request)
        {
            using(var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                return reader.ReadToEnd();
        }

        public static void End(this HttpListenerResponse response)
        {

            response.OutputStream.Flush();
            response.OutputStream.Close();
        }

        public static IEnumerable<(string key, string[] value)> GetQueryData(this HttpListenerRequest request)
        {
            for (int i = 0; i < request.QueryString.Keys.Count; i++)
                if (request.QueryString.Keys[i] != null)
                    yield return (request.QueryString.Keys[i], request.QueryString.GetValues(i));
        }

        public static IEnumerable<(string key, string[] value)> GetHeaderData(this HttpListenerRequest request)
        {
            for (int i = 0; i < request.Headers.Keys.Count; i++)
                if (request.Headers.Keys[i] != null)
                    yield return (request.Headers.Keys[i], request.Headers.GetValues(i));
        }

        public static T Deserialize<T>(this string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch { return default(T); }
        }

        public static string Serialize(this object obj) =>
            JsonConvert.SerializeObject(obj);

        public static string GetPath(this HttpListenerRequest req) =>
            req.RawUrl.Split('?')[0];
    }
}
