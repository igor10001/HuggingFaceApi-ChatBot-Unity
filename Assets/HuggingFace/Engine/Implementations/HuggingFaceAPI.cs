using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace HuggingFace.API {
    public static class HuggingFaceAPI {
        private static IAPIConfig _config;
        private static IAPIConfig config {
            get {
                if (_config == null) {
                    _config = Resources.Load<APIConfig>("HuggingFaceAPIConfig");
                    if (_config == null) {
                        Debug.LogError("HuggingFaceAPIConfig asset not found.");
                    }
                }
                return _config;
            }
        }

        private static IAPIClient _apiClient;
        private static IAPIClient apiClient {
            get {
                if (_apiClient == null) {
                    _apiClient = new APIClient();
                }
                return _apiClient;
            }
        }

        private static Dictionary<string, ITask> tasks;

        static HuggingFaceAPI() {
            LoadTasks();
        }

        private static void LoadTasks() {
            tasks = new Dictionary<string, ITask>();

            var taskTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(ITask)) && !t.IsInterface && !t.IsAbstract);

            foreach (var taskType in taskTypes) {
                var task = (ITask)Activator.CreateInstance(taskType);
                tasks.Add(task.taskName, task);
            }
        }

       
        public static void Query(string taskName, object input, Action<object> onSuccess, Action<string> onError, object context = null) {
            if (tasks.TryGetValue(taskName, out var task)) {
                task.Query(input, apiClient, config, onSuccess, onError, context);
            } else {
                onError($"Task {taskName} not found.");
            }
        }

     
        public static void Conversation(string input, Action<string> onSuccess, Action<string> onError, Conversation context = null) {
            Query<string, string, Conversation>("Conversation", input, onSuccess, onError, context);
        }

      
       

        


        
        public static void TestAPIKey(string apiKey, Action<string> onSuccess, Action<string> onError) {
            apiClient.TestAPIKey(apiKey, onSuccess, onError).RunCoroutine();
        }

        private static void Query<TInput, TResponse>(string taskName, TInput input, Action<TResponse> onSuccess, Action<string> onError) {
            if (!(input is TInput)) {
                onError?.Invoke($"Input is not of type {typeof(TInput)}");
                return;
            }
            Query(taskName, input, (response) => {
                if (!(response is TResponse)) {
                    onError?.Invoke($"Response is not of type {typeof(TResponse)}");
                    return;
                }
                onSuccess?.Invoke((TResponse)response);
            }, onError);
        }

        private static void Query<TInput, TResponse, TContext>(string taskName, TInput input, Action<TResponse> onSuccess, Action<string> onError, TContext context = default(TContext)) {
            if (!(input is TInput)) {
                onError?.Invoke($"Input is not of type {typeof(TInput)}");
                return;
            }
            if (!(context == null || context is TContext)) {
                onError?.Invoke($"Context is not of type {typeof(TContext)}");
                return;
            }
            Query(taskName, input, (response) => {
                if (!(response is TResponse)) {
                    onError?.Invoke($"Response is not of type {typeof(TResponse)}");
                    return;
                }
                onSuccess?.Invoke((TResponse)response);
            }, onError, context);
        }
    }
}
