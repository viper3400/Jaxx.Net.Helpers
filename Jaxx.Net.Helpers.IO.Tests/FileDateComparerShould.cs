using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Jaxx.Net.Helpers.IO.Tests
{
    public class FileDateComparerShould
    {
        private readonly ITestOutputHelper output;
        public FileDateComparerShould(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void EvaluateFileAgeByLastWriteTime()
        {
            // clear previous test run
            var tmpPath = Path.Join(Path.GetTempPath(), "Jaxx.Net.Helpers.IO.Tests","EvaluateFileAgeByLastWriteTime");
            if (Directory.Exists(tmpPath)) Directory.Delete(tmpPath, true);

            // init test with new directory and new file
            Directory.CreateDirectory(tmpPath);
            var fileName = "myfile.txt";
            //create first file (old one)
            var oldFilePath = Path.Join(tmpPath, fileName);
            File.WriteAllText(oldFilePath, "I am the old one!");
            // now create a new one (but wait a second)
            System.Threading.Thread.Sleep(1000);
            var newFilePath = Path.Join(tmpPath, "newfile.txt");
            File.WriteAllText(newFilePath, "I am the new one!");

            var file1 = new FileInfo(oldFilePath);
            var file2 = new FileInfo(newFilePath);

            var comparer = new FileDateComparer(file2, file1, CompareDateOption.LastWriteTime);
            output.WriteLine($"TC1: OldFile: {comparer.OldFile}, NewFile: {comparer.NewFile}");
            Assert.Equal(file1, comparer.OldFile);
            Assert.Equal(file2, comparer.NewFile);

            // modify old file (but wait a second)
            System.Threading.Thread.Sleep(1000);
            File.AppendAllText(oldFilePath, "I may be the old one, but i've got an update!");

            // refesh file info
            file1 = new FileInfo(oldFilePath);
            file2 = new FileInfo(newFilePath);

            comparer = new FileDateComparer(file2, file1, CompareDateOption.LastWriteTime);
            output.WriteLine($"TC2: OldFile: {comparer.OldFile}, NewFile: {comparer.NewFile}");
            Assert.Equal(file2, comparer.OldFile);
            Assert.Equal(file1, comparer.NewFile);
        }
        [Fact]
        public void EvaluateFileAgeByCreationTime()
        {
            // clear previous test run
            var tmpPath = Path.Join(Path.GetTempPath(), "Jaxx.Net.Helpers.IO.Tests", "EvaluateFileAgeByLastWriteTime");
            if (Directory.Exists(tmpPath)) Directory.Delete(tmpPath, true);

            // init test with new directory and new file
            Directory.CreateDirectory(tmpPath);
            var fileName = "myfile.txt";
            //create first file (old one)
            var oldFilePath = Path.Join(tmpPath, fileName);
            File.WriteAllText(oldFilePath, "I am the old one!");
            // now create a new one
            System.Threading.Thread.Sleep(1000);
            var newFilePath = Path.Join(tmpPath, "newfile.txt");
            File.WriteAllText(newFilePath, "I am the new one!");

            var file1 = new FileInfo(oldFilePath);
            var file2 = new FileInfo(newFilePath);

            var comparer = new FileDateComparer(file2, file1, CompareDateOption.LastWriteTime);
            Assert.Equal(file1, comparer.OldFile);
            Assert.Equal(file2, comparer.NewFile);
        }
    }
}
