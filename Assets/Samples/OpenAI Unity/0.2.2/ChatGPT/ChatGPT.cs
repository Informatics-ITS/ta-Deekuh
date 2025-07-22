using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace OpenAI
{
    public class ChatGPT : MonoBehaviour
    {
        // [SerializeField] private InputField inputField;
        // [SerializeField] private Button button;
        // [SerializeField] private ScrollRect scroll;
        
        [SerializeField] private RectTransform sent;
        [SerializeField] private RectTransform received;

        private float height;
        private OpenAIApi openai = new OpenAIApi();

        private List<ChatMessage> messages = new List<ChatMessage>();
        // private string prompt = "You are an expert in Japanese ninja culture and Naruto lore.";

        private void Start()
        {
            // button.onClick.AddListener(SendReply);
        }

        private void AppendMessage(ChatMessage message)
        {
            // scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

            // var item = Instantiate(message.Role == "user" ? sent : received, scroll.content);
            // item.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Content;
            // item.anchoredPosition = new Vector2(0, -height);
            // LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            // height += item.sizeDelta.y;
            // scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            // scroll.verticalNormalizedPosition = 0;
        }

        public async void SendReply()
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content =
                "Anda akan membuat daftar kata yang cocok dan familiar dalam tema jutsu/ninja, terutama berasal dari series Naruto. Semua kata harus dalam huruf KAPITAL, tanpa angka, tanpa simbol, dan bukan merupakan nama tokoh atau karakter dalam series Naruto. " +
                "Format hasil HARUS berupa satu array, TANPA teks tambahan apapun, persis seperti contoh berikut:\n\n" +
                "[\"CHI\", \"ANBU\", \"KYUBI\", \"JUTSU\", \"KUNAI\", \"CHAKRA\", \"HOKAGE\", \"TENSEI\", \"SHURIKEN\", \"RASENGAN\"]\n\n" +
                "Aturan:\n" +
                "- 2–3 kata pertama harus terdiri dari 3–4 huruf\n" +
                "- 5–6 kata berikutnya harus terdiri dari 5–6 huruf\n" +
                "- 2–3 kata terakhir harus terdiri dari 7–8 huruf\n" +
                "- Semua kata dalam array harus berurutan sesuai aturan di atas. Pastikan total kata dalam array adalah 10, tidak lebih atau kurang.\n" +
                "- Pastikan setiap kata memiliki panjang yang semakin meningkat, dari kata pertama sampai kata ke sepuluh.\n" +
                "- Jangan menambahkan teks apapun, berikan respon dalam bentuk JSON array saja.\n" +
                "- Kata terpanjang yang diizinkan adalah 8 huruf."
            };
            
            // AppendMessage(newMessage);

            // if (messages.Count == 0) newMessage.Content = prompt + "\n" + inputField.text; 
            
            messages.Add(newMessage);
            Debug.Log("Sending message to GPT: " + newMessage.Content);
            
            // button.enabled = false;
            // inputField.text = "";
            // inputField.enabled = false;

            // Complete the instruction
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-4o",
                Messages = messages
            });
            
            Debug.Log("GPT raw response: " + completionResponse);

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();

                // SAVE the content into PlayerPrefs
                // PlayerPrefs.SetString("LastWordListJson", message.Content);
                // PlayerPrefs.Save();
                // Debug.Log("Saved GPT response to PlayerPrefs: " + message.Content);.

                var levelManager = FindObjectOfType<LevelManager>();
                if (levelManager != null)
                {
                    Debug.Log("Updating levels from GPT response: " + message.Content);
                    levelManager.UpdateLevelsFromGPT(message.Content);
                }
                else
                {
                    Debug.LogWarning("LevelManager not found!");
                }

                // messages.Add(message);
                // AppendMessage(message);
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }

            // button.enabled = true;
            // inputField.enabled = true;
        }
    }
}
