using System;
using System.Collections.Generic;
using System.Text;

namespace Jaxx.Net.Helpers.IO
{
    public class CopyOptions
    {
        public CopyStrategy CopyStrategy { get; set; } = CopyStrategy.Stop;
        public CompareDateOption CompareDateOptions { get; set; } = CompareDateOption.LastWriteTime;
    }
}
