﻿using LeagueSandbox.GameServer.Logging;
using log4net;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace LeagueSandbox.GameServer
{
    internal class Server : IDisposable
    {
        private string _blowfishKey;
        private string _serverVersion = "0.2.0";
        private readonly ILog _logger;
        private Game _game;
        private Config _config;
        private ushort _serverPort { get; }

        public Server(Game game, ushort port, string configJson, string blowfishKey)
        {
            _logger = LoggerProvider.GetLogger();
            _game = game;
            _serverPort = port;
            _blowfishKey = blowfishKey;
            _config = Config.LoadFromJson(game, configJson);

            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        public void Start()
        {
            var build = $"League Sandbox Build {ServerContext.BuildDateString}";
            Console.Title = build;
            _logger.Debug(build);
            _logger.Debug($"Yorick {_serverVersion}");
            _logger.Info($"Game started on port: {_serverPort}");
            _game.Initialize(_serverPort, _blowfishKey, _config);
        }

        public void StartNetworkLoop()
        {
            _game.GameLoop();
        }

        public void Dispose()
        {
            // PathNode.DestroyTable();
        }

        public void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            if (e.Exception is InvalidCastException || e.Exception is KeyNotFoundException)
                return;

            _logger.Error($"A first chance exception was thrown {e.Exception}");
        }

        public void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs x)
        {
            var ex = (Exception)x.ExceptionObject;
            _logger.Error("An unhandled exception was thrown", ex);
        }
    }
}
