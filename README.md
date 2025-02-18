# AI Chat in Unity  

## 📌 Project Description  
This project is a simple 2D game in Unity with an integrated AI chat using **Hugging Face Inference API**. The player can enter messages, send them to the chat, and receive AI-generated responses in real-time.  

## 🚀 Features  
- **Basic UI:**  
  - Input field for user messages  
  - "Send" button  
  - Chat window (ScrollView with Text)  
- **Hugging Face API Integration:**  
  - Sending user messages via API requests  
  - Receiving and displaying AI responses  
- **Additional Enhancements:**  
  - Different styles for player and AI messages  
  - Asynchronous request handling with `async/await`  
  - Error handling (timeouts, connection issues)  

## 🔧 Technologies Used  
- **Unity** (version X.X.X)  
- **C#**  
- **UnityWebRequest** for API communication  
- **Hugging Face Inference API**  

## ⚙️ How to Run the Project  
1. **Clone the repository** or download the project archive  
2. **Open in Unity** (compatible with version X.X.X and above)  
3. **Insert your API key** in `AIChatManager.cs` (or the relevant script handling API requests)  
4. **Run the scene** (`Scenes/MainScene`)  

## 🖥️ API Used  
This project utilizes **Hugging Face Inference API** with the `facebook/blenderbot-3B` model for AI-generated responses.  

Example request:  
```csharp
UnityWebRequest request = UnityWebRequest.Post("https://api-inference.huggingface.co/models/facebook/blenderbot-3B", json);
request.SetRequestHeader("Authorization", "Bearer YOUR_API_KEY");
