using System;
using System.IO;
using Jaxx.Net.Helpers.IO;
using Xunit;
using Xunit.Abstractions;

namespace Jaxx.Net.Helpers.IO.Tests
{
    public class FileOperationsShould
    {
        private readonly ITestOutputHelper output;
        public FileOperationsShould(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void GetCountentUpFileExtension()
        {
            // clear previous test run
            var tmpPath = Path.Join(Path.GetTempPath(), "counttest");
            if (Directory.Exists(tmpPath)) Directory.Delete(tmpPath, true);

            // init test with new directory and new file
            Directory.CreateDirectory(tmpPath);
            var fileName = "myfile.txt";
            var tmpFilePath = Path.Join(tmpPath, fileName);
            File.WriteAllText(tmpFilePath, "test content");

            // run test
            output.WriteLine($"Start with: {tmpFilePath}");
            for (int i = 1; i <  1001; i++)
            {
                if (i < 1000)
                {
                    var actual = FileOperations.GetCountedUpExtension(tmpFilePath);
                    var expected = tmpFilePath.Remove(tmpFilePath.Length - i.ToString().Length, i.ToString().Length) + i.ToString();
                    output.WriteLine($"i: {i} - {expected} - {actual}");
                    Assert.Equal(actual, expected);
                    File.WriteAllText(actual, "test content");
                    tmpFilePath = actual;
                }
                else if (i > 999)
                {
                    Assert.Throws<NotSupportedException>(() => FileOperations.GetCountedUpExtension(tmpFilePath));
                }
            }
        }

        [Fact]
        public void GetCountetUpFileName()
        {
            // clear previous test run
            var tmpPath = Path.Join(Path.GetTempPath(), "countfilenametest");
            if (Directory.Exists(tmpPath)) Directory.Delete(tmpPath, true);

            // init test with new directory and new file
            Directory.CreateDirectory(tmpPath);
            var fileName = "myfile.txt";
            var tmpFilePath = Path.Join(tmpPath, fileName);
            File.WriteAllText(tmpFilePath, "test content");

            // run test
            output.WriteLine($"Start with: {tmpFilePath}");
            for (int i = 1; i < 101; i++)
            {
                var actual = FileOperations.GetCountedUpFilename(tmpFilePath);
                var expected = tmpFilePath.Remove(tmpFilePath.Length - 4, 4) + i.ToString() + ".txt";
                output.WriteLine($"i: {i} - {expected} - {actual}");
                Assert.Equal(actual, expected);
                File.WriteAllText(actual, "test content");
            }
        }

        [Fact]
        public void RenameLatestFile()
        {
            // clear previous test run
            var tmpPath = Path.Join(Path.GetTempPath(), "renamelatesttest");
            if (Directory.Exists(tmpPath)) Directory.Delete(tmpPath, true);

            // init test with new directory and new file
            Directory.CreateDirectory(tmpPath);
            var fileName = "myfile.txt";
            //create first file (old one)
            var oldFilePath = Path.Join(tmpPath, fileName);
            File.WriteAllText(oldFilePath, "I am the old one!");
            // now create a new one
            var newFilePath = Path.Join(tmpPath, "newfile.txt");
            File.WriteAllText(newFilePath, "I am the new one!");

            // now we try to "move" (rename) the new one, but we expect it to fail
            Assert.Throws<System.IO.IOException>( () => File.Move(newFilePath, oldFilePath));

            FileOperations.Copy(newFilePath, oldFilePath, new CopyOptions { CopyStrategy = CopyStrategy.RenameOld, CompareDateOptions = CompareDateOption.LastWriteTime });
        }
    }
}
