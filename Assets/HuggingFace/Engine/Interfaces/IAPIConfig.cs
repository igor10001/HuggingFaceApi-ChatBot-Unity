using System.Collections.Generic;

namespace HuggingFace.API {
    public interface IAPIConfig {
        string apiKey { get; }
        bool useBackupEndpoints { get; }
        bool waitForModel { get; }
        float maxTimeout { get; }
        List<TaskURL> taskEndpoints { get; }
        bool GetTaskEndpoint(string taskName, out TaskURL taskURL);
    }
}