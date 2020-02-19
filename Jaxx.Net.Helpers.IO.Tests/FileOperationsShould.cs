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
        private XunitLogger<FileOperations> logger;
        public FileOperationsShould(ITestOutputHelper output)
        {
            this.output = output;
            logger = new XunitLogger<FileOperations>(output);
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
                    var actual = new FileOperations(logger).GetCountedUpExtension(tmpFilePath);
                    var expected = tmpFilePath.Remove(tmpFilePath.Length - i.ToString().Length, i.ToString().Length) + i.ToString();
                    output.WriteLine($"i: {i} - {expected} - {actual}");
                    Assert.Equal(actual, expected);
                    File.WriteAllText(actual, "test content");
                    tmpFilePath = actual;
                }
                else if (i > 999)
                {
                    Assert.Throws<NotSupportedException>(() => new FileOperations(logger).GetCountedUpExtension(tmpFilePath));
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
                var actual = new FileOperations(logger).GetCountedUpFilename(tmpFilePath);
                var expected = tmpFilePath.Remove(tmpFilePath.Length - 4, 4) + i.ToString() + ".txt";
                output.WriteLine($"i: {i} - {expected} - {actual}");
                Assert.Equal(actual, expected);
                File.WriteAllText(actual, "test content");
            }
        }

        [Fact]
        public void RenameOldFile()
        {
            // clear previous test run
            var tmpSourcePath = Path.Join(Path.GetTempPath(), "Jaxx.Net.Helpers.IO.Tests", "RenameOldFileSource");
            if (Directory.Exists(tmpSourcePath)) Directory.Delete(tmpSourcePath, true);

            var tmpDestPath = Path.Join(Path.GetTempPath(), "Jaxx.Net.Helpers.IO.Tests", "RenameOldFileDest");
            if (Directory.Exists(tmpDestPath)) Directory.Delete(tmpDestPath, true);

            // init test with new directory and new file
            Directory.CreateDirectory(tmpSourcePath);
            Directory.CreateDirectory(tmpDestPath);
            var fileName = "myfile.txt";
            //create first file (old one) in destination
            var oldFilePath = Path.Join(tmpDestPath, fileName);
            File.WriteAllText(oldFilePath, "I am the old one!");
            // now create a new one in sourcePath (but wait a little to assure that file date has changed)
            System.Threading.Thread.Sleep(2000);
            var newFilePath = Path.Join(tmpSourcePath, "myfile.txt");
            File.WriteAllText(newFilePath, "I am the new one!");

            // now we try to "move" (rename) the new one, but we expect it to fail
            Assert.Throws<System.IO.IOException>( () => File.Move(newFilePath, oldFilePath));

            new FileOperations(logger).Copy(newFilePath, oldFilePath, new CopyOptions { CopyStrategy = CopyStrategy.RenameOld, CompareDateOptions = CompareDateOption.LastWriteTime });

            // We expect new content in the old file path
            var actual = File.ReadAllText(oldFilePath);
            Assert.Equal("I am the new one!", actual);

            // We expect a renamed file
            var expectedRenamedFile = Path.Join(tmpDestPath, "myfile.tx1");
            Assert.True(File.Exists(expectedRenamedFile));

            // We expect the old content
            var expectedContentFromRenamedFile = File.ReadAllText(expectedRenamedFile);
            Assert.Equal("I am the old one!", expectedContentFromRenamedFile);
        }

        [Fact]
        public void RenameOldFile2()
        {
            // clear previous test run
            var tmpSourcePath = Path.Join(Path.GetTempPath(), "Jaxx.Net.Helpers.IO.Tests", "RenameOldFileSource2");
            if (Directory.Exists(tmpSourcePath)) Directory.Delete(tmpSourcePath, true);

            var tmpDestPath = Path.Join(Path.GetTempPath(), "Jaxx.Net.Helpers.IO.Tests", "RenameOldFileDest2");
            if (Directory.Exists(tmpDestPath)) Directory.Delete(tmpDestPath, true);

            // init test with new directory and new file
            Directory.CreateDirectory(tmpSourcePath);
            Directory.CreateDirectory(tmpDestPath);
            var fileName = "myfile.txt";
            //create first file (old one) in destination
            var oldFilePath = Path.Join(tmpSourcePath, fileName);
            File.WriteAllText(oldFilePath, "I am the source, i am the old!");
            // now create a new one in sourcePath (but wait a little to assure that file date has changed)
            System.Threading.Thread.Sleep(2000);
            var newFilePath = Path.Join(tmpDestPath, "myfile.txt");
            File.WriteAllText(newFilePath, "I am the the existing, i am new!");

            // now we try to "move" (rename) the new one, but we expect it to fail
            Assert.Throws<System.IO.IOException>(() => File.Move(newFilePath, oldFilePath));

            new FileOperations(logger).Copy(oldFilePath, newFilePath, new CopyOptions { CopyStrategy = CopyStrategy.RenameOld, CompareDateOptions = CompareDateOption.LastWriteTime });

            // We expect new content in the old file path
            var actual = File.ReadAllText(Path.Join(tmpDestPath, "myfile.txt"));
            Assert.Equal("I am the the existing, i am new!", actual);

            // We expect a renamed file
            var expectedRenamedFile = Path.Join(tmpDestPath, "myfile.tx1");
            Assert.True(File.Exists(expectedRenamedFile));

            // We expect the old content
            var expectedContentFromRenamedFile = File.ReadAllText(expectedRenamedFile);
            Assert.Equal("I am the source, i am the old!", expectedContentFromRenamedFile);
        }

        [Fact]
        public void RenameOldFile3()
        {
            // clear previous test run
            var tmpSourcePath = Path.Join(Path.GetTempPath(), "Jaxx.Net.Helpers.IO.Tests", "RenameOldFileSource3");
            if (Directory.Exists(tmpSourcePath)) Directory.Delete(tmpSourcePath, true);

            var tmpDestPath = Path.Join(Path.GetTempPath(), "Jaxx.Net.Helpers.IO.Tests", "RenameOldFileDest3");
            if (Directory.Exists(tmpDestPath)) Directory.Delete(tmpDestPath, true);

            // init test with new directory and new file
            Directory.CreateDirectory(tmpSourcePath);
            Directory.CreateDirectory(tmpDestPath);
            var fileName = "myfile.txt";
            //create first file (old one) in destination
            var destFile = Path.Join(tmpDestPath, fileName);
            File.WriteAllText(destFile, "I am the existing, i am the old!");
            // now create a new one in sourcePath (but wait a little to assure that file date has changed)
            System.Threading.Thread.Sleep(2000);
            var sourceFile = Path.Join(tmpSourcePath, "myfile.txt");
            File.WriteAllText(sourceFile, "I am the source, i am the new!");

            // now we try to "move" (rename) the new one, but we expect it to fail
            Assert.Throws<System.IO.IOException>(() => File.Move(sourceFile, destFile));

            new FileOperations(logger).Copy(sourceFile, destFile, new CopyOptions { CopyStrategy = CopyStrategy.RenameOld, CompareDateOptions = CompareDateOption.LastWriteTime });

            // We expect new content in the old file path
            var actual = File.ReadAllText(Path.Join(tmpDestPath, "myfile.txt"));
            Assert.Equal("I am the source, i am the new!", actual);

            // We expect a renamed file
            var expectedRenamedFile = Path.Join(tmpDestPath, "myfile.tx1");
            Assert.True(File.Exists(expectedRenamedFile));

            // We expect the old content
            var expectedContentFromRenamedFile = File.ReadAllText(expectedRenamedFile);
            Assert.Equal("I am the existing, i am the old!", expectedContentFromRenamedFile);
        }

        [Fact]
        public void RenameNewFile()
        {
            // clear previous test run
            var tmpSourcePath = Path.Join(Path.GetTempPath(), "Jaxx.Net.Helpers.IO.Tests", "RenameNewFileSource");
            if (Directory.Exists(tmpSourcePath)) Directory.Delete(tmpSourcePath, true);

            var tmpDestPath = Path.Join(Path.GetTempPath(), "Jaxx.Net.Helpers.IO.Tests", "RenameNewFileDest");
            if (Directory.Exists(tmpDestPath)) Directory.Delete(tmpDestPath, true);

            // init test with new directory and new file
            Directory.CreateDirectory(tmpSourcePath);
            Directory.CreateDirectory(tmpDestPath);

            // init test with new directory and new file
            Directory.CreateDirectory(tmpDestPath);
            var fileName = "myfile.txt";
            //create first file (old one)
            var oldFilePath = Path.Join(tmpSourcePath, fileName);
            File.WriteAllText(oldFilePath, "I am the old one!");
            // now create a new one (but wait a little to assure that file date has changed)
            System.Threading.Thread.Sleep(2000);
            var newFilePath = Path.Join(tmpSourcePath, "newfile.txt");
            File.WriteAllText(newFilePath, "I am the new one!");

            // now we try to "move" (rename) the new one, but we expect it to fail
            Assert.Throws<System.IO.IOException>(() => File.Move(newFilePath, oldFilePath));

            new FileOperations(logger).Copy(newFilePath, oldFilePath, new CopyOptions { CopyStrategy = CopyStrategy.RenameNew, CompareDateOptions = CompareDateOption.LastWriteTime });

            // We expect old content in the old file path
            var actual = File.ReadAllText(oldFilePath);
            Assert.Equal("I am the old one!", actual);

            // We expect a renamed file
            var expectedRenamedFile = Path.Join(tmpSourcePath, "myfile.tx1");
            Assert.True(File.Exists(expectedRenamedFile));

            // We expect the new content
            var expectedContentFromRenamedFile = File.ReadAllText(expectedRenamedFile);
            Assert.Equal("I am the new one!", expectedContentFromRenamedFile);
        }
    }
}
