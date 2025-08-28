using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NS
{
    /// <summary>
    /// UniTaskを使用した遅延処理の実行を管理するクラス
    /// </summary>
    public class UniTaskCoroutineRunner : MonoBehaviour
    {
        private static UniTaskCoroutineRunner _instance;
        private static readonly object Lock = new object();
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// インスタンスの取得
        /// </summary>
        public static UniTaskCoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Lock)
                    {
                        if (_instance == null)
                        {
                            var go = new GameObject("UniTaskCoroutineRunner");
                            DontDestroyOnLoad(go);
                            _instance = go.AddComponent<UniTaskCoroutineRunner>();
                        }
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                _cancellationTokenSource = new CancellationTokenSource();
            }
        }

        private void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        /// <summary>
        /// 指定した時間後に処理を実行します
        /// </summary>
        /// <param name="delaySeconds">遅延時間（秒）</param>
        /// <param name="action">実行する処理</param>
        public async UniTask DelayActionAsync(float delaySeconds, Action action)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken: _cancellationTokenSource.Token);
                action?.Invoke();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Delayed action was cancelled");
            }
        }

        /// <summary>
        /// 指定したフレーム数後に処理を実行します
        /// </summary>
        /// <param name="frameCount">待機するフレーム数</param>
        /// <param name="action">実行する処理</param>
        public async UniTask DelayFrameActionAsync(int frameCount, Action action)
        {
            try
            {
                await UniTask.DelayFrame(frameCount, cancellationToken: _cancellationTokenSource.Token);
                action?.Invoke();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Delayed frame action was cancelled");
            }
        }

        /// <summary>
        /// 条件が満たされるまで待機して処理を実行します
        /// </summary>
        /// <param name="predicate">待機条件</param>
        /// <param name="action">実行する処理</param>
        public async UniTask WaitUntilActionAsync(Func<bool> predicate, Action action)
        {
            try
            {
                await UniTask.WaitUntil(predicate, cancellationToken: _cancellationTokenSource.Token);
                action?.Invoke();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Wait until action was cancelled");
            }
        }

        /// <summary>
        /// 実行中の全ての遅延処理をキャンセルします
        /// </summary>
        public void CancelAllTasks()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
        }
    }
} 