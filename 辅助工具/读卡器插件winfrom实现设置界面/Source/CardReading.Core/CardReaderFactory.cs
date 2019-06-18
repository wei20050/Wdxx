using System;
using System.Linq;
using System.IO;
using System.IO.Ports;
using System.Reflection;

namespace CardReading.Core
{
    public static class CardReaderFactory
    {
        public static IReadCard GetCardReader()
        {
            var type = GetBusinessEntityType(Settings.CardReaderType);
            if (!(Activator.CreateInstance(type) is IReadCard cardReader)) return null;
            var paths = Environment.GetEnvironmentVariable("PATH")?.Split(';');
            var dir = Directory.GetParent(type.Assembly.Location).FullName;
            var exist = (paths ?? throw new InvalidOperationException()).Any(i =>
                0 == string.Compare(i, dir, StringComparison.CurrentCultureIgnoreCase));
            if (!exist)
            {
                Environment.SetEnvironmentVariable("PATH",
                    Environment.GetEnvironmentVariable("PATH") + ";" + dir);
            }
            if (SerialPort.GetPortNames().Length == 1)
            {
                Settings.CardReaderComPort = SerialPort.GetPortNames()[0];
            }
            else
            {
                if (string.IsNullOrEmpty(Settings.CardReaderComPort) && SerialPort.GetPortNames().Length > 0)
                {
                    Settings.CardReaderComPort = SerialPort.GetPortNames()[0];
                }
            }
            cardReader.ComPort = Settings.CardReaderComPort;
            cardReader.Ini();
            return cardReader;
        }

        private static Type GetBusinessEntityType(string typeName)
        {
            var location = Directory.GetParent(typeof(CardReaderFactory).Assembly.Location).FullName;
            var dllDirectory = Path.Combine(location, @"IdCardReader");
            foreach (var dir in Directory.GetDirectories(dllDirectory))
            {
                foreach (var file in Directory.GetFiles(dir, "CardReading*.dll"))
                {
                    var fullPath = Path.GetFullPath(file);
                    Assembly.LoadFrom(fullPath);
                }
            }
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith("CardReading")).ToList();
            if (assemblies.Count < 2)
            {
                throw new ArgumentException("没有读卡插件!");
            }
            if (string.IsNullOrEmpty(Settings.CardReaderType))
            {
                Settings.CardReaderType = typeName = "CardReading.Common.CardReadingCommon";
            }
            foreach (var assembly in assemblies)
            {
                var t = assembly.GetType(typeName, false);
                if (t != null)
                    return t;
            }
            Settings.CardReaderType = typeName = "CardReading.Common.CardReadingCommon";
            foreach (var assembly in assemblies)
            {
                var t = assembly.GetType(typeName, false);
                if (t != null)
                    return t;
            }
            throw new ArgumentException("类型 " + typeName + " 找不到");
        }
    }
}