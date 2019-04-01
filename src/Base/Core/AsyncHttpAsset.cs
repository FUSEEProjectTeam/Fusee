using Fusee.Base.Common;
using JSIL.Meta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace Fusee.Base.Core
{
    public class AsyncHttpAsset
    {
        private static Dictionary<Type, AssetHandler> _assetHandlers = new Dictionary<Type, AssetHandler>()
        {
            {
                typeof(string),
                new AssetHandler
                {
                    ReturnedType = typeof(string),
                    Decoder = (id, data) => {
                        string  sr = WrapString(data);
                        return sr;
                    },
                    Checker = (id) => { return true; }
                }
            }
        };

        public static Dictionary<Type, AssetHandler> AssetHandlers { get => _assetHandlers; set => _assetHandlers = value; }

        public string Id { get; private set; }
        public Type Type { get; private set; }
        public object Content { get; private set; }
        public AsyncAssetState State { get; private set; }

        public event EventHandler onDone;
        public event EventHandler onFail;

        public AsyncHttpAsset(string id, Type type, bool startLoad = true)
        {
            Id = id;
            Type = type;

            onDone += AsyncHttpAsset_onDone;
            onFail += AsyncHttpAsset_onFail;

            if (!_assetHandlers.ContainsKey(Type))
            {
                Diagnostics.Log(this.GetType() + " does not contain an AssetHandler for type " + Type + " returning data as " + typeof(byte[]));
            }

            State = AsyncAssetState.None;

            if (startLoad)
                StartLoad();
        }

        public AsyncHttpAsset(string id, Type type, EventHandler onDone, EventHandler onFail, bool startLoad = true) : this(id, type, startLoad)
        {
            this.onDone += onDone;
            this.onFail += onFail;
        }

        private void AsyncHttpAsset_onDone(object sender, EventArgs e)
        {
            Diagnostics.Log(((AsyncHttpAsset)sender).Id + " Done");
        }

        private void AsyncHttpAsset_onFail(object sender, EventArgs e)
        {
            Diagnostics.Log(((AsyncHttpAsset)sender).Id + " Failed");
        }

        public void StartLoad()
        {
            if (State == AsyncAssetState.None)
            {
                State = AsyncAssetState.Downloading;
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
                    ProcessAsset(await client.GetByteArrayAsync(Id));
                }
                catch (HttpRequestException e)
                {
                    State = AsyncAssetState.Failed;
                    onFail?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void ProcessAsset(object data)
        {
            State = AsyncAssetState.Processing;

            if (_assetHandlers.ContainsKey(Type))
            {
                Content = _assetHandlers[Type].Decoder(Id, data);
            }
            else
            {
                Content = data;
            }

            State = AsyncAssetState.Done;
            onDone?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// JsDoneCallback is used by the JavaScript implementation of this class to return data after loading.
        /// </summary>
        /// <param name="content"></param>
        private void JsDoneCallback(byte[] content)
        {
            Diagnostics.Log(content.Length);
            ProcessAsset(content);
        }

        /// <summary>
        /// JsFailCallback is used by the JavaScript implementation of this class to return data after loading.
        /// </summary>
        private void JsFailCallback()
        {
            this.State = AsyncAssetState.Failed;
            onFail?.Invoke(this, EventArgs.Empty);
        }

        [JSExternal]
        public static ImageData WrapImage(object assetOb)
        {
            throw new NotImplementedException("This method is implemented in JavaScript [JSExternal]");
        }

        [JSExternal]
        public static string WrapString(object data)
        {
            return Encoding.UTF8.GetString((byte[])data);
        }
    }

    public enum AsyncAssetState
    {
        None,
        Downloading,
        Processing,
        Done,
        Failed
    }
}