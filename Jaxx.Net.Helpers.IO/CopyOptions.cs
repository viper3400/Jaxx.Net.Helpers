using System;
using System.Collections.Generic;
using System.Text;

namespace Jaxx.Net.Helpers.IO
{
    public class CopyOptions
    {
        /// <summary>
        /// Determines how a existing file in the destination is to be handled during a copy operation. Options are
        /// RenameNew, RenameOld, Overwrite. Default is RenameOld. With RenameOld and RenameNew the source and target
        /// files are checked for their creation date or for their last write date. With RenameOld the older file is 
        /// renamed, with RenameNew the younger file.
        /// </summary>
        public CopyStrategy CopyStrategy { get; set; } = CopyStrategy.RenameOld;
        public CompareDateOption CompareDateOptions { get; set; } = CompareDateOption.LastWriteTime;
    }
}
