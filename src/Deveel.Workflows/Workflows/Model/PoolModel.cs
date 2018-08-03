using System;
using System.Collections.Generic;

namespace Deveel.Workflows.Model
{
    public sealed class PoolModel
    {
        public PoolModel()
        {
            Lanes = new List<SwimlaneModel>();
        }

        public string Name { get; set; }

        public ICollection<SwimlaneModel> Lanes { get; set; }
    }
}
