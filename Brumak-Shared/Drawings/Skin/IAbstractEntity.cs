using Brumak_Shared.Character.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_Shared.Drawings.Skin
{
    public interface IAbstractEntity
    {
        SkinEnum Skin { get; set; }
        List<string> SkinHexColors { get; set; }
    }
}
