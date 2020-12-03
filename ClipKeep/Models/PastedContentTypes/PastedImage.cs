using System.Drawing;
using System.Threading.Tasks;
using ClipKeep.Models.Interfaces;

namespace ClipKeep.Models
{
    public sealed class PastedImage : PastedItem<Bitmap>
    {

        public override void Display()
        {
            throw new System.NotImplementedException();
        }

        public override Task<bool> StoreInDb()
        {
            throw new System.NotImplementedException();
        }

        public override string ToJson()
        {
            throw new System.NotImplementedException();
        }
    }
}