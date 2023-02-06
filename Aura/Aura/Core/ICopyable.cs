using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aura.Core
{
    public interface ICopyable<out T>
    {
        T Copy();
    }
}
