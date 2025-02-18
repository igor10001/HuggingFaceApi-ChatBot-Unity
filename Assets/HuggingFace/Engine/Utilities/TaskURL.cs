namespace HuggingFace.API {
    [System.Serializable]
    public class TaskURL {
        public string taskName;
        public string endpoint;

        public TaskURL(string taskName, string endpoint) {
            this.taskName = taskName;
            this.endpoint = endpoint;
        }
    }
}
