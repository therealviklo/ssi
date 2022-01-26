using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssi
{
    /// <summary>
    /// En klass som representerar en stenhög.
    /// </summary>
    class StonePile
    {
        public ulong V;

        public StonePile(ulong v)
        {
            V = v;
        }

        /// <summary>
        /// Öka stenhögens värde.
        /// </summary>
        public void Inc()
        {
            V++;
        }

        /// <summary>
        /// Minska stenhögens värde, om nollskilt.
        /// </summary>
        public void Dec()
        {
            if (V > 0)
                V--;
        }
    }
}
