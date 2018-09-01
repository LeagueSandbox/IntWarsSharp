﻿using CommandLine;
using LeagueSandbox.GameServerConsole;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeagueSandbox.GameServerConsoleTests
{
    [TestClass]
    public class TestArgsOptions
    {
        [TestMethod]
        public void TestConfigAndPort()
        {
            var port = 5839;
            var config = "/some/config/path.json";
            var args = new[]
            {
                "--port", port.ToString(),
                "--config", config
            };
            ArgsOptions.Parse(args).WithParsed(options =>
            {
                Assert.AreEqual(port, options.ServerPort);
                Assert.AreEqual(config, options.GameInfoJsonPath);
            });
        }

        [TestMethod]
        public void TestDefaults()
        {
            ArgsOptions.Parse(new string[0]).WithParsed(options =>
            {
                Assert.IsTrue(options.ServerPort > 0);
                Assert.IsFalse(string.IsNullOrEmpty(options.GameInfoJsonPath));
            });
        }
    }
}
