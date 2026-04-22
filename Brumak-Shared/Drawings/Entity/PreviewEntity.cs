using Brumak_Shared.Character.Model;
using Brumak_Shared.Drawings.Skin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_Shared.Drawings.Entity
{
    public class PreviewEntity : IAbstractEntity
    {
        public int Skin { get; set; }
        public string[] SkinHexColors { get; set; } = ["#C8B48C", "#783C28", "#50321E", "#4488CC", "#CC4444"];
        SkinEnum IAbstractEntity.Skin { get => (SkinEnum)this.Skin; set => Skin = (int)value; }
        List<string> IAbstractEntity.SkinHexColors { get => [.. this.SkinHexColors]; set => SkinHexColors = [.. value]; }
    }
}
