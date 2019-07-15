using UniRx.Async;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class ResourceLoader
{
    [Inject] private IWebRequestService _webRequestService;

    public async UniTask<SpineData[]> LoadCharacterData(string[] idList)
    {
        var spineDataArr = new SpineData[idList.Length];
        for (var i = 0;
            i < idList.Length;
            i++)
        {
            var id = idList[i];
            var txtModel = await _webRequestService.GetTextAsync(
                UnityWebRequest.Get(UrlUtils.GetCharacterJsonUrl(id)));
            Debug.Log($"-->[ProgramEntry] received player character: {txtModel}");
            var txtAtlas = await _webRequestService.GetTextAsync(
                UnityWebRequest.Get(UrlUtils.GetCharacterAtlasUrl(id)));
            Debug.Log($"-->[ProgramEntry] received player atlas: {txtAtlas}");
            var characterTextureUrl = UrlUtils.GetCharacterTextureUrl(id);
            Debug.Log($"-->[ProgramEntry] downloading character texture url: {characterTextureUrl}");
            var texture2D =
                await _webRequestService.GetTextureAsync(
                    UnityWebRequestTexture.GetTexture(characterTextureUrl));
            Debug.Log($"-->[ProgramEntry] character texture url: {texture2D.width} - {texture2D.height}");
            var data = new SpineData
            {
                Id = idList[i],
                TxtModel = txtModel,
                TxtAtlas = txtAtlas,
                CharTexture = texture2D,
                SkeletonDataAsset = null
            };
            spineDataArr[i] = data;
        }

        return spineDataArr;
    }
}