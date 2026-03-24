using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_Shared.Metrics
{
    public class Exceptions
    {
        private static readonly Logger _logger = new("Metrics", typeof(Exceptions));

        public static Exception New(string message)
        {
            _logger.Log($"New Exception: {message}");
            return new Exception(message);
        }
    }
}
