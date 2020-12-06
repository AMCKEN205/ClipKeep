using System;
using System.Threading.Tasks;
using ClipKeep.Models.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ClipKeep.Models
{
    public sealed class PastedText : IPastedItem
    {

        public PastedText(JObject getResult)
        {
            Content = (string) getResult["PastedContent"];
            DatePasted = DateTime.Parse((string) getResult["DatePasted"]);
            DbId = (string) getResult["id"];
        }

        /// <summary>
        /// Text content of pasted text
        /// </summary>
        public string Content { get; private set; }
        /// <summary>
        /// Date + time the text content was pasted into clipkeep
        /// </summary>
        public DateTime DatePasted { get; private set; }

        /// <summary>
        /// Unique ID of the content item in the DB.
        /// </summary>
        public string DbId { get; private set; }

        /// <summary>
        /// Allows execution of logic specific to the base content object.
        /// E.g. format for display.
        /// </summary>
        public string ContentType { get; } = "Text";
    }
}