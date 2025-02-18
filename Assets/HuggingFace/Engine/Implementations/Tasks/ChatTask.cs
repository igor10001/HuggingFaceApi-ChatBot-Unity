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
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace HuggingFace.API
{
    public class ChatTask : TaskBase<string, string, Conversation>
    {
        public override string taskName => "Conversation";
        public override string defaultEndpoint => "https://api-inference.huggingface.co/models/facebook/blenderbot-3B";

        protected override string[] LoadBackupEndpoints()
        {
            return new string[]
            {
                "https://api-inference.huggingface.co/models/microsoft/DialoGPT-medium",
                "https://api-inference.huggingface.co/models/facebook/blenderbot-400M-distill"
            };
        }

        protected override bool VerifyContext(object context, out Conversation conversation)
        {
            conversation = null;
            if (context == null)
            {
                conversation = new Conversation();
                return true;
            }
            else if (context is Conversation)
            {
                conversation = (Conversation)context;
                return true;
            }

            return false;
        }

        protected override IPayload GetPayload(string input, Conversation conversation)
        {
            var payload = new JObjectPayload(new JObject
            {
                ["inputs"] = input
            });

            string jsonPayload = payload.ToString();
            Debug.Log($"Payload being sent: {jsonPayload}");

            return payload;
        }

        private async Task<string> GetResponseFromApi(IPayload payload)
        {
            using (var client = new HttpClient())
            {
                string apiUrl = defaultEndpoint;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "YOUR_API_KEY");
                // Remove unnecessary headers
                // client.DefaultRequestHeaders.Add("X-API-Task", "conversational");

                string jsonPayload = payload.ToString();
                Debug.Log($"Payload being sent: {jsonPayload}");

                var response = await client.PostAsync(apiUrl,
                    new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Debug.LogError($"Error: {response.StatusCode}, {response.ReasonPhrase}, {responseContent}");
                    throw new HttpRequestException(
                        $"Error: {response.StatusCode}, {response.ReasonPhrase}, {responseContent}");
                }
            }
        }

        protected override bool PostProcess(object raw, string input, Conversation conversation, out string response,
            out string error)
        {
            response = null;
            error = "";

            try
            {
                JToken jsonResponse = JToken.Parse((string)raw);
                Debug.Log($"Raw response: {jsonResponse}");

                if (jsonResponse.Type == JTokenType.Array)
                {
                    // The response is a JSON array
                    JArray responseArray = (JArray)jsonResponse;
                    if (responseArray.Count > 0)
                    {
                        // Handle array of strings or objects
                        var firstItem = responseArray[0];

                        if (firstItem.Type == JTokenType.String)
                        {
                            // If the array contains strings
                            response = firstItem.ToString();
                        }
                        else if (firstItem.Type == JTokenType.Object)
                        {
                            // If the array contains objects with 'generated_text'
                            if (firstItem["generated_text"] != null)
                            {
                                response = firstItem["generated_text"].ToString();
                            }
                            else
                            {
                                error = "Array item does not contain 'generated_text'.";
                                return false;
                            }
                        }
                        else
                        {
                            error = "Unexpected array item type.";
                            return false;
                        }
                    }
                    else
                    {
                        error = "Response array is empty.";
                        return false;
                    }
                }
                else if (jsonResponse.Type == JTokenType.Object)
                {
                    // The response is a JSON object
                    JObject jsonObject = (JObject)jsonResponse;
                    if (jsonObject.TryGetValue("generated_text", out JToken generatedTextToken))
                    {
                        response = generatedTextToken.ToString();
                    }
                    else
                    {
                        error = "Response does not contain 'generated_text'.";
                        return false;
                    }
                }
                else if (jsonResponse.Type == JTokenType.String)
                {
                    // The response is a simple string
                    response = jsonResponse.ToString();
                }
                else
                {
                    error = "Unexpected response format.";
                    return false;
                }

                // Update the conversation history
                conversation.AddUserInput(input);
                conversation.AddGeneratedResponse(response);

                return true;
            }
            catch (Exception ex)
            {
                error = $"Exception during PostProcess: {ex.Message}";
                return false;
            }


        }
    }
}
