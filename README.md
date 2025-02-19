# AI Chat in Unity  

## 📌 Project Description  
This project is a simple  integrated AI chat using **Hugging Face Inference API**. The player can enter messages, send them to the chat, and receive conversational AI-generated responses in real-time.  

## 🚀 Features  
- **Basic UI:**  
  - Input field for user messages  
  - "Send" button  
  - Chat window (ScrollView with Text)  
- **Hugging Face API Integration:**  
  - Sending user messages via API requests  
  - Receiving and displaying AI responses  
  - Error handling (timeouts, connection issues)  


- **C#**  
- **UnityWebRequest** 
- **Hugging Face API**   

## ⚙️ How to Run the Project  
1. **Clone the repository** or download the project archive  
2. **Open in Unity** (compatible with version 2022.3.51f1 and above)  
3. **Insert your API key** in `AIChatManager.cs`
   - Click on Window → Hugging Face API Configuration.
   - In the API Key field, paste your Hugging Face API key
   - In the API Model field, enter the name of the model you want to use.
       - Example: "facebook/blenderbot-3B" for a chatbot.
   - Adjust Additional Settings
       - Timeout: Set a timeout value (e.g., 10 seconds) to prevent long wait times.
   Error Handling: Enable error notifications for a smoother experience.


5. **Run the scene** (`HuggingFace/Resources/Scenes/ChatBot`)  

## 🖥️ API Used  
This project utilizes **Hugging Face API** with the `facebook/blenderbot-3B` model for AI-generated responses.  

Example request:  
```csharp
UnityWebRequest request = new UnityWebRequest("https://huggingface.co/api/whoami-v2", "GET");
request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
