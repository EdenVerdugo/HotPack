using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO
{
    public static class FileInfoExtensions
    {

        public static string GetMimeType(this FileInfo file)
        {
            return HotPack.Helpers.FileHelper.GetMimeTypeByExtension(file.Extension);
        }
    }
}
