namespace CardReading.Core
{
    public static class Settings
    {
        public static string CardReaderComPort
        {
            get => IniHelper.Read(nameof(CardReaderComPort));
            set => IniHelper.Write(nameof(CardReaderComPort), value);
        }

        public static string CardReaderType
        {
            get => IniHelper.Read(nameof(CardReaderType));
            set => IniHelper.Write(nameof(CardReaderType), value);
        }
    }
}