using System;
using Newtonsoft.Json;

namespace ClipKeep.Models
{
    /// <summary>
    /// Base class for all pasted item types (text or photo atm).
    /// Would make this protected if multiple assemblies existed,
    /// but as this is just a prototype app shouldn't matter too much/shouldn't need to.
    /// </summary>
    public abstract class PastedItem<T>
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
        public string ParentUserId
        {
            get { return _parentUserId; }
        }

        /// <summary>
        /// The actual pasted text/image
        /// </summary>
        public T Content { get; set; }

        /// <summary>
        /// Time the user pasted the item into clipkeep
        /// </summary>
        public DateTime DatePasted { get; }
        
        private void SetParentUserId()
        {
            _parentUserId = "Not Implemented";
        }

        public abstract string ToJson();


    }
}