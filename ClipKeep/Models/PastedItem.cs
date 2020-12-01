using System;
using Newtonsoft.Json;

namespace ClipKeep.Models
{
    /// <summary>
    /// Base class for all pasted item types (text or photo atm).
    /// Would make this protected if multiple namespaces existed,
    /// but as this is just a prototype app shouldn't matter too much/shouldn't need to.
    /// </summary>
    public abstract class PastedItem
    {

        protected string _parentUserId;

        protected PastedItem()
        {
            DatePasted = DateTime.Now;
            SetParentUserId();
        }
            
        /// <summary>
        ///  Id of user the pasted item belongs to.
        /// </summary>
        [JsonProperty(PropertyName = "parentUserId")]
        public string ParentUserId
        {
            get { return _parentUserId; }
        }

        /// <summary>
        /// Time the user pasted the item into clipkeep
        /// </summary>
        [JsonProperty(PropertyName = "datePasted")]
        public DateTime DatePasted { get; }
        
        private void SetParentUserId()
        {
            _parentUserId = "Not Implemented";
        }

        /// <summary>
        /// !!! For debugging purposes !!!
        /// </summary>
        /// <returns>
        /// String output of object's properties.
        /// </returns>
        public abstract override string ToString();

    }
}