using System;
using System.Threading;
using UniRx.Async;
using UnityEngine;

public class TestOuterCancel : MonoBehaviour
{
    private CancellationTokenSource _cancellationTokenSource;

    void Start()
    {
        // 1. CancellationTokenSourceのインスタンスを生成
        _cancellationTokenSource = new CancellationTokenSource();

        // 2. UniTask実行時にCancellationTokenのインスタンスを渡す
        CancelableAsync(_cancellationTokenSource.Token)
            .Forget(e => { }); // 呼び出し階層のトップでOperationCanceledExceptionを握りつぶす
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            // 3. CancellationTokenSourceのCancel()を呼び出す
            _cancellationTokenSource.Cancel();
        }
    }

    async UniTask CancelableAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1));

        Debug.Log("waited 1sec");

        if (cancellationToken.IsCancellationRequested)
        {
            // a.
            // キャンセルされていたらOperationCanceledExceptionをスロー
            throw new OperationCanceledException(cancellationToken);
        }

        await UniTask.Delay(TimeSpan.FromSeconds(2));

        Debug.Log("waited 2sec");

        // b.
        // キャンセルされていたらOperationCanceledExceptionをスロー
        cancellationToken.ThrowIfCancellationRequested();

        // c.
        // キャンセルされていたらOperationCanceledExceptionをスロー
        await UniTask.Delay(TimeSpan.FromSeconds(3),
            cancellationToken: cancellationToken);

        Debug.Log("waited 3sec");

        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(5),
                cancellationToken: cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // キャンセル時に実行したい処理があればtry~catchで捕捉
            Debug.LogWarning("Canceled!");
            throw;
        }

        Debug.LogWarning("Done!");
    }
}