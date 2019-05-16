using JSIL.Meta;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Fusee.Base.Core
{
    /// <summary>
    /// Class to load data over HTTP asynchronously and process it according to AsyncAssetHandlers.
    /// </summary>
    public class AsyncHttpAsset
    {
        private static readonly Dictionary<Type, AsyncAssetHandler> _assetHandlers = new Dictionary<Type, AsyncAssetHandler>()
        {
            {
                typeof(byte[]),
                new AsyncAssetHandler
                {
                    ReturnedType = typeof(byte[]),
                    Decoder = (id, data, callback) => {
                        callback(data);
                    },
                }
            }
        };

        /// <summary>
        /// The async asset handlers.
        /// </summary>
        public static Dictionary<Type, AsyncAssetHandler> AssetHandlers { get => _assetHandlers; }

        /// <summary>
        /// Register an AsyncAssetHandler for a certain Type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="assetHandler"></param>
        public static void RegisterTypeHandler(Type type, AsyncAssetHandler assetHandler)
        {
            _assetHandlers.Add(type, assetHandler);
        }

        /// <summary>
        /// Id/URL of the asset.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Type of the asset.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Content of the asset.
        /// </summary>
        public object Content { get; private set; }

        /// <summary>
        /// State of the asset.
        /// </summary>
        public AsyncAssetState State { get; set; }

        /// <summary>
        /// EventHandler for when downloading and processing is done.
        /// </summary>
        public event EventHandler onDone;

        /// <summary>
        /// EventHandler for when either downloading or processing fails.
        /// </summary>
        public event EventHandler onFail;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="startLoad">If true, starts loading immediately, otherwise call StartGet() to start loading the asset.</param>
        public AsyncHttpAsset(string id, Type type = null, bool startLoad = true)
        {
            if (type == null)
                type = typeof(byte[]);

            Id = id;
            Type = type;

            onDone += AsyncHttpAsset_onDone;
            onFail += AsyncHttpAsset_onFail;

            if (!_assetHandlers.ContainsKey(Type))
            {
                Diagnostics.Log(this.GetType() + " does not contain an AssetHandler for type " + Type + " returning data as " + typeof(byte[]));
                Type = typeof(byte[]);
            }

            State = AsyncAssetState.None;

            if (startLoad)
                StartLoad();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="onDone"></param>
        /// <param name="onFail"></param>
        /// <param name="startLoad">If true, starts loading immediately, otherwise call StartGet() to start loading the asset.</param>
        public AsyncHttpAsset(string id, Type type, EventHandler onDone, EventHandler onFail, bool startLoad = true) : this(id, type, startLoad)
        {
            this.onDone += onDone;
            this.onFail += onFail;
        }

        private void AsyncHttpAsset_onDone(object sender, EventArgs e)
        {
            Diagnostics.Log("Download " + ((AsyncHttpAsset)sender).Id + " Done");
        }

        private void AsyncHttpAsset_onFail(object sender, EventArgs e)
        {
            Diagnostics.Log("Download " + ((AsyncHttpAsset)sender).Id + " Failed");
        }

        public void StartLoad()
        {
            if (State == AsyncAssetState.None)
            {
                GetAsset();
            }
        }

        private void GetAsset()
        {
            State = AsyncAssetState.Downloading;

            DoGetAsset();
        }

        [JSExternal]
        private async void DoGetAsset()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    byte[] bytes = await client.GetByteArrayAsync(Id);
                    //onDownloaded
                    ProcessAsset(bytes);
                }
                catch (HttpRequestException e)
                {
                    FailCallback();
                }
            }
        }

        private void ProcessAsset(object data)
        {
            State = AsyncAssetState.Processing;

            if (_assetHandlers.ContainsKey(Type))
            {
                _assetHandlers[Type].Decoder(Id, data, this.DoneCallback);
            }
        }

        private void DoneCallback(object content)
        {
            Content = content;
            this.State = AsyncAssetState.Done;
            onDone?.Invoke(this, EventArgs.Empty);
        }

        private void FailCallback()
        {
            this.State = AsyncAssetState.Failed;
            onFail?.Invoke(this, EventArgs.Empty);
        }
    }

    public enum AsyncAssetState
    {
        /// <summary>
        /// No available state.
        /// </summary>
        None,
        /// <summary>
        /// Asset is being downloaded.
        /// </summary>
        Downloading,
        /// <summary>
        /// Asset is being processed.
        /// </summary>
        Processing,
        /// <summary>
        /// State: done.
        /// </summary>
        Done,
        /// <summary>
        /// State: failed.
        /// </summary>
        Failed,
        UserState1,
        UserState2,
        UserState3
    }

    public struct AsyncAssetHandler
    {
        public Type ReturnedType;
        public AsyncAssetDecoder Decoder;
    }

    public delegate void AsyncAssetDecoder(string id, object storage, Action<object> returndata);
}