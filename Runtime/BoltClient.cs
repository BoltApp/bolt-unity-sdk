using UnityEngine.Networking;
using System.Collections;
using System.Text;
using UnityEngine;

public class BoltClient
{
    private readonly string baseUrl;

    public BoltClient(string baseUrl)
    {
        this.baseUrl = baseUrl.TrimEnd('/');
    }

    private UnityWebRequest CreateRequest(string url, string method, string jsonBody = null)
    {
        var request = new UnityWebRequest(url, method);
        if (!string.IsNullOrEmpty(jsonBody))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        return request;
    }

    public IEnumerator Get(string endpoint, System.Action<string> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/{endpoint}";
        var request = CreateRequest(url, UnityWebRequest.kHttpVerbGET);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            onSuccess?.Invoke(request.downloadHandler.text);
        else
            onError?.Invoke(request.error);
    }

    public IEnumerator Delete(string endpoint, System.Action<string> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/{endpoint}";
        var request = CreateRequest(url, UnityWebRequest.kHttpVerbDELETE);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            onSuccess?.Invoke(request.downloadHandler.text);
        else
            onError?.Invoke(request.error);
    }
}
