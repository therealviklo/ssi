﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssi
{
    class Dec : ICommand
    {
        public void Run(Dictionary<string, StonePile> env, bool print)
        {
            env["x"].Dec();
        }
    }
}
