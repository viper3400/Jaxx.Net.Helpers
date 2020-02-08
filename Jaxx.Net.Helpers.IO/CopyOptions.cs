using System;
using System.Collections.Generic;
using System.Text;

namespace Jaxx.Net.Helpers.IO
{
    public class CopyOptions
    {
        public CopyStrategy CopyStrategy { get; set; } = CopyStrategy.RenameOld;
        public CompareDateOption CompareDateOptions { get; set; } = CompareDateOption.LastWriteTime;
    }
}
