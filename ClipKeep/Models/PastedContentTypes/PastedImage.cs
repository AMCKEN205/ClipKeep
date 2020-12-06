using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using ClipKeep.Models.Interfaces;
using Newtonsoft.Json.Linq;

namespace ClipKeep.Models
{
    public sealed class PastedImage : IPastedItem
    {
        public PastedImage(JObject getResult)
        {
            ContentType = (string) getResult["PastedContentType"];
            Content = (string) getResult["PastedContent"];
            DatePasted = DateTime.Parse((string) getResult["DatePasted"]);
            DbId = (string) getResult["id"];
        }

        /// <summary>
        /// Allows execution of logic specific to the base content object.
        /// E.g. format for display.
        /// </summary>
        public string ContentType { get; private set; }

        /// <summary>
        /// Image data url of the pasted image's content.
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// Date + time the image content was pasted into ClipKeep
        /// </summary>
        public DateTime DatePasted { get; private set; }

        /// <summary>
        /// The unique ID of the content item in the database.
        /// </summary>
        public string DbId { get; private set; }
    }
}