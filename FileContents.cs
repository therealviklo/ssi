using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssi
{
    /// <summary>
    /// En klass som representerar innehållet i en fil. UTF-8 antas som teckenkodning.
    /// </summary>
    class FileContents
    {
        public string Contents { get; private set; }

        public FileContents(string filename)
        {
            Contents = System.IO.File.ReadAllText(filename, Encoding.UTF8);
        }
    }
}
