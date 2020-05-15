using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Simplex.Core.Util
{
    public static class Logger
    {
        public static readonly log4net.ILog Default = log4net.LogManager.GetLogger(Assembly.GetExecutingAssembly(), "Simplex");
    }
}
