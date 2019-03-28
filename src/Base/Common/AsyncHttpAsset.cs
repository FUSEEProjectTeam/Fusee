using JSIL.Meta;
using System;
using System.Net.Http;

namespace Fusee.Base.Common
{
    public class AsyncHttpAsset<T>
    {
        public string Id { get; private set; }
        public Type Type { get; private set; }
        public object Content { get; private set; }
        public byte[] Contentraw { get; private set; }
        public AsyncAssetState State { get; private set; }

        public event EventHandler onDone;
        public event EventHandler onFail;

        public AsyncHttpAsset(string id, bool startLoad = true)
        {
            this.Id = id;
            Type = typeof(T);

            State = AsyncAssetState.None;

            if (startLoad)
                StartLoad();
        }

        public AsyncHttpAsset(string id, EventHandler onDone, EventHandler onFail, bool startLoad = true) : this(id, startLoad)
        {
            this.onDone = onDone;
            this.onFail = onFail;
        }

        public void StartLoad()
        {
            if (State == AsyncAssetState.None)
            {
                State = AsyncAssetState.Processing;
                StartGet();
            }
        }

        [JSExternal]
        private async void StartGet()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    Contentraw = await client.GetByteArrayAsync(Id);
                    State = AsyncAssetState.Done;
                    onDone?.Invoke(this, EventArgs.Empty);
                }
                catch (HttpRequestException e)
                {
                    State = AsyncAssetState.Failed;
                    onFail?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// JsDoneCallback is used by the JavaScript implementation of this class to return data after loading.
        /// </summary>
        /// <param name="content"></param>
        private void JsDoneCallback(byte[] content)
        {
            this.Contentraw = content;
            this.State = AsyncAssetState.Done;
            onDone?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// JsFailCallback is used by the JavaScript implementation of this class to return data after loading.
        /// </summary>
        private void JsFailCallback()
        {
            this.State = AsyncAssetState.Failed;
            onFail?.Invoke(this, EventArgs.Empty);
        }
    }

    public enum AsyncAssetState
    {
        None,
        Processing,
        Done,
        Failed
    }
}