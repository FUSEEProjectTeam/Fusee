﻿using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Fusee.Base.Core
{
    /// <summary>
    /// Class to load data over HTTP asynchronously and process it according to AsyncAssetHandlers.
    /// </summary>
    public class AsyncHttpAsset
    {
        private static readonly Dictionary<Type, AsyncAssetDecoder> _assetHandlers = new()
        {
            // Default callback for byte[]
            {
                typeof(byte[]),
                (id, data, callback) => { callback(data); }
            },
            // Default callback for string
            {
                typeof(string),
                (id, data, callback) => { callback(System.Text.Encoding.Default.GetString((byte[])data)); }
            }
        };

        /// <summary>
        /// The async asset handlers.
        /// </summary>
        public static Dictionary<Type, AsyncAssetDecoder> AssetHandlers { get => _assetHandlers; }

        /// <summary>
        /// Register an AsyncAssetHandler.
        /// </summary>
        /// <param name="assetHandler"></param>
        public static void RegisterTypeHandler(AsyncAssetHandler assetHandler)
        {
            _assetHandlers.Add(assetHandler.ReturnedType, assetHandler.Decoder);
        }

        /// <summary>
        /// Register an AsyncAssetDecoder for a certain Type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="asyncAssetDecoder"></param>
        public static void RegisterTypeHandler(Type type, AsyncAssetDecoder asyncAssetDecoder)
        {
            _assetHandlers.Add(type, asyncAssetDecoder);
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
        public event EventHandler OnDone;

        /// <summary>
        /// EventHandler for when either downloading or processing fails.
        /// </summary>
        public event EventHandler OnFail;

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

            OnDone += AsyncHttpAsset_onDone;
            OnFail += AsyncHttpAsset_onFail;

            if (!_assetHandlers.ContainsKey(Type))
            {
                Diagnostics.Warn(this.GetType() + " does not contain an AssetHandler for type " + Type + " returning data as " + typeof(byte[]));
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
            this.OnDone += onDone;
            this.OnFail += onFail;
        }

        private void AsyncHttpAsset_onDone(object sender, EventArgs e)
        {
            Diagnostics.Debug("Download " + ((AsyncHttpAsset)sender).Id + " Done");
        }

        private void AsyncHttpAsset_onFail(object sender, EventArgs e)
        {
            Diagnostics.Debug("Download " + ((AsyncHttpAsset)sender).Id + " Failed");
        }

        /// <summary>
        /// Starts to load the assets.
        /// </summary>
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

        private async void DoGetAsset()
        {
            using HttpClient client = new();
            try
            {
                byte[] bytes = await client.GetByteArrayAsync(Id);
                //onDownloaded
                ProcessAsset(bytes);
            }
            catch
            {
#pragma warning disable ERP022 // Unobserved exception in generic exception handler
                FailCallback();
            }
#pragma warning restore ERP022 // Unobserved exception in generic exception handler

        }

        private void ProcessAsset(object data)
        {
            State = AsyncAssetState.Processing;

            if (_assetHandlers.ContainsKey(Type))
            {
                _assetHandlers[Type](Id, data, this.DoneCallback);
            }
        }

        private void DoneCallback(object content)
        {
            Content = content;
            State = AsyncAssetState.Done;
            OnDone?.Invoke(this, EventArgs.Empty);
        }

        private void FailCallback()
        {
            State = AsyncAssetState.Failed;
            OnFail?.Invoke(this, EventArgs.Empty);
        }
    }
    /// <summary>
    /// The states an asset can inhibit.
    /// </summary>
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
        /// <summary>
        /// State: defined by User.
        /// </summary>
        UserState1,
        /// <summary>
        /// State: defined by User.
        /// </summary>
        UserState2,
        /// <summary>
        /// State: defined by User.
        /// </summary>
        UserState3
    }

    /// <summary>
    /// The asset handler.
    /// </summary>
    public struct AsyncAssetHandler
    {
        /// <summary>
        /// The asset load type.
        /// </summary>
        public Type ReturnedType;
        /// <summary>
        /// The asset decoder.
        /// </summary>
        public AsyncAssetDecoder Decoder;
    }

    /// <summary>
    /// The asset decoder
    /// </summary>
    /// <param name="id">The asset id.</param>
    /// <param name="data">The asset data.</param>
    /// <param name="callback">The decoded asset data.</param>
    public delegate void AsyncAssetDecoder(string id, object data, Action<object> callback);
}