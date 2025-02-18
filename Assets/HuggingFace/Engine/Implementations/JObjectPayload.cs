using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System;
using System.Net.Http;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace HuggingFace.API {
    public class JObjectPayload : IPayload
    {
        public Newtonsoft.Json.Linq.JObject  JsonObject { get; }

        public JObjectPayload(Newtonsoft.Json.Linq.JObject jsonObject)
        {
            JsonObject = jsonObject;
        }

        public void Prepare(UnityWebRequest request)
        {
            string jsonString = JsonObject.ToString();
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonString);

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
        }

        public override string ToString()
        {
            return JsonObject.ToString();
        }
    }
}
