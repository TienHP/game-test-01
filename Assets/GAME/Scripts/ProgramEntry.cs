using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Zenject;

public class ProgramEntry : MonoBehaviour
{
    [Inject] private IWebRequestService _webRequestService;
    [SerializeField] private RawImage _img;

    // Start is called before the first frame update
    async Task Start()
    {
        var text = await _webRequestService.GetTextAsync(
            UnityWebRequest.Get(UrlUtils.GetCharacterJsonUrl("3999")));
        Debug.Log($"-->[ProgramEntry] received player character: {text}");
        var atlas = await _webRequestService.GetTextAsync(
            UnityWebRequest.Get(UrlUtils.GetCharacterAtlasUrl("3999")));
        Debug.Log($"-->[ProgramEntry] received player atlas: {atlas}");
        var characterTextureUrl = UrlUtils.GetCharacterTextureUrl("3999");
        Debug.Log($"-->[ProgramEntry] downloading character texture url: {characterTextureUrl}");
        var texture2D =
            await _webRequestService.GetTextureAsync(
                UnityWebRequestTexture.GetTexture(characterTextureUrl));
        Debug.Log($"-->[ProgramEntry] character texture url: {texture2D.width} - {texture2D.height}");
        _img.texture = texture2D;

        text = await _webRequestService.GetTextAsync(
            UnityWebRequest.Get(UrlUtils.GetCharacterJsonUrl("4000")));
        Debug.Log($"-->[ProgramEntry] received enemy character: {text}");
        atlas = await _webRequestService.GetTextAsync(
            UnityWebRequest.Get(UrlUtils.GetCharacterAtlasUrl("4000")));
        Debug.Log($"-->[ProgramEntry] received enemy atlas: {atlas}");
    }

    // Update is called once per frame
    void Update()
    {
    }
}