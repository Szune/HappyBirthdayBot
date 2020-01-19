
namespace BirthdayBot.IO
{
    public class FileWriter : IFileWriter
    {
        public void SerializeToFile<T>(string file, T value) => JsonHelper.SerializeToFile(file, value);
    }
}