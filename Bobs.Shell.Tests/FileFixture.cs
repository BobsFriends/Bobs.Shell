using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bobs.Shell.Tests
{
    public class FileFixture : IDisposable
    {
        public FileFixture()
        {
            string tempPath = Path.GetTempPath();
            TestFolder = Path.Combine(tempPath, Path.GetRandomFileName());
            Directory.CreateDirectory(TestFolder);
        }

        public string TestFolder { get; }

        internal string GetLinkPath()
        {
            return Path.ChangeExtension(Path.Combine(TestFolder, Path.GetRandomFileName()), ".lnk");
        }

        public void Dispose()
        {
            Directory.Delete(TestFolder, true);
        }

        internal string CreateTargetFile()
        {
            string targetFilePath = Path.ChangeExtension(Path.Combine(TestFolder, Path.GetRandomFileName()), ".exe");
            File.WriteAllText(targetFilePath, "I'm the target file");
            return targetFilePath;
        }

        internal string CreateWorkingDirectory()
        {
            string workingDirectoryPath = Path.ChangeExtension(Path.Combine(TestFolder, Path.GetRandomFileName()), null);
            Directory.CreateDirectory(workingDirectoryPath);
            return workingDirectoryPath;
        }

        internal string CreateIconFile()
        {
            string iconFilePath = Path.ChangeExtension(Path.Combine(TestFolder, Path.GetRandomFileName()), ".ico");
            File.WriteAllText(iconFilePath, "I'm the icon file");
            return iconFilePath;
        }
    }
}
