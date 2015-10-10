﻿using System.Collections.Generic;
using NLog;
using Qplex.Layers;

namespace Qplex
{
    /// <summary>
    /// Container contains the layers, initiates them, connects them by channels and start them.
    /// </summary>
    public abstract class Container
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Layers list
        /// </summary>
        private readonly IList<ILayer> _layersList;

        /// <summary>
        /// Ctor
        /// </summary>
        protected Container()
        {
            _layersList = new List<ILayer>();
        }

        /// <summary>
        /// Start the container
        /// </summary>
        public void Start()
        {
            Logger.Log(LogLevel.Debug, "Initiating container...");
            InitContainer();
            Logger.Log(LogLevel.Debug, "Initiating layers...");
            InitLayers();
            Logger.Log(LogLevel.Debug, "Starting layers...");
            StartLayers();
        }

        /// <summary>
        /// Add layer to container
        /// </summary>
        /// <param name="layer">Layer</param>
        protected void AddLayer(ILayer layer)
        {
            Logger.Log(LogLevel.Debug, $"Added layer {layer.GetType().Name}");
            _layersList.Add(layer);
        }

        /// <summary>
        /// Initiate the layers
        /// </summary>
        protected void InitLayers()
        {
            foreach (var layer in _layersList)
            {
                if (!layer.Init())
                {
                    Qplex.Instance.CloseApplication($"The layer {layer.GetType().FullName} failed to initialize!");
                }
            }
        }

        /// <summary>
        /// Start the layers
        /// </summary>
        protected void StartLayers()
        {
            foreach (var layer in _layersList)
            {
                if (!layer.Start())
                {
                    Qplex.Instance.CloseApplication($"The layer {layer.GetType().FullName} failed to start!");
                }
            }
        }

        /// <summary>
        /// Initiate the container:
        ///     -Create layers instances
        ///     -Create channels
        ///     -Connect layers by channels
        /// </summary>
        protected abstract void InitContainer();
    }
}
