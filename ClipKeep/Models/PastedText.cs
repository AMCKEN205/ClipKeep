using System;
using ClipKeep.Models.Interfaces;
using Newtonsoft.Json;

namespace ClipKeep.Models
{
    public class PastedText : PastedItem<string>, IDisplayable
    {
        public void Display()
        {
            throw new NotImplementedException();
        }

        public override string ToJson()
        {
            throw new NotImplementedException();
        }
    }
}