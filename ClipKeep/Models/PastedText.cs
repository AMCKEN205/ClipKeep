using System;
using Newtonsoft.Json;

namespace ClipKeep.Models
{
    public class PastedText : PastedItem
    {

        public PastedText(string content) : base()
        {
            Content = content;
        }

        [JsonProperty(PropertyName = "content")]
        public string Content { get; }

        /// <summary>
        ///  !!! For debugging purposes !!!
        /// </summary>
        /// <returns>
        /// String output of object's properties.
        /// </returns>
        public override string ToString()
        {
            string pastedItemToString = $"Parent User ID: {ParentUserId} Date Pasted: {DatePasted}";

            return pastedItemToString;
        }
    }
}