namespace BirthdayBot.IO
{
    public interface IFileWriter
    {
        void SerializeToFile<T>(string file, T value);
    }
}