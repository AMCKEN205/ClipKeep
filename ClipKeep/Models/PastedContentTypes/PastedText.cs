using System;
using System.Threading.Tasks;
using ClipKeep.Models.Interfaces;
using Newtonsoft.Json;

namespace ClipKeep.Models
{
    public sealed class PastedText : PastedItem<string>
    {
        public override void Display()
        {
            throw new NotImplementedException();
        }

        public override Task<bool> StoreInDb()
        {
            throw new NotImplementedException();
        }

        public override string ToJson()
        {
            throw new NotImplementedException();
        }
    }
}