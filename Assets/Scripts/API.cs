using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class ApiData
{
    // Default params to make our API requisitions
    public int game;
    public string id;
    public string artifact;
    public int value;
}

public class API : MonoBehaviour
{
    private string apiUrl = "http://52.67.75.192/api/data/";

    public void SendDataToAPI(string artifact, int value)
    {
        //The data payload
        ApiData data = new ApiData();
        data.game = 3;
        data.id = PlayerPrefs.GetString("player");
        data.artifact = artifact;
        data.value = value;

        // To prevent send data while testing
        if (PlayerPrefs.GetInt("sendData") == 1) 
        {
            StartCoroutine(SendReq(JsonUtility.ToJson(data)));
        }
    }

    private IEnumerator SendReq(string jsonData)
    {
        Debug.Log(apiUrl);
        Debug.Log(jsonData);

        using (UnityWebRequest request = UnityWebRequest.Post(apiUrl, jsonData))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Erro na requisição: " + request.error);
            }
            else
            {
                Debug.Log("Requisição bem-sucedida!");
            }

            request.Dispose();
        }
    }

}
