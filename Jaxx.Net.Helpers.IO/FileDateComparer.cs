using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;

namespace Jaxx.Net.Helpers.IO
{
    public class FileDateComparer
    {
        public FileDateComparer(FileInfo file1, FileInfo file2, CompareDateOption compareDateOption)
        {
            Contract.Requires(file1 != null);
            Contract.Requires(file2 != null);

            switch (compareDateOption)
            {
                case CompareDateOption.CreationTime:
                    if (file1.CreationTimeUtc > file2.CreationTimeUtc)
                    {
                        OldFile = file2;
                        NewFile = file1;
                    } 
                    else if (file1.CreationTimeUtc <= file2.CreationTimeUtc)
                    {
                        OldFile = file1;
                        NewFile = file2;
                    }
                    break;
                case CompareDateOption.LastWriteTime:
                    if (file1.LastWriteTimeUtc > file2.LastWriteTimeUtc)
                    {
                        OldFile = file2;
                        NewFile = file1;
                    }
                    else if (file1.LastWriteTimeUtc <= file2.LastWriteTimeUtc)
                    {
                        OldFile = file1;
                        NewFile = file2;
                    }
                    break;
            }
        }

        public FileInfo OldFile { get; private set; }
        public FileInfo NewFile { get; private set; }
    }
}
