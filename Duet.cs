using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plateform_Launcher
{
    class Duet<K, V>
    {
        public readonly K Key;
        public readonly V Value;

        public Duet(K key, V value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}
