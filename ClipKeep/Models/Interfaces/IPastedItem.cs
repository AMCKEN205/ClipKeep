using System;
using ClipKeep.Models.Interfaces;

namespace ClipKeep.Models
{
    /// <summary>
    /// Base class for all pasted item types (text or photo atm).
    /// Would make this protected if multiple assemblies existed,
    /// but as this is just a prototype app shouldn't matter too much/shouldn't need to.
    /// </summary>
    public interface IPastedItem
    {
        /// <summary>
        /// Time the user pasted the item into clipkeep.
        /// </summary>
        DateTime DatePasted { get; }

        /// <summary>
        /// An indication of what the base object is for HTML formatting/display.
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// The actual content of the pasted item.
        /// </summary>
        string Content { get; }

        /// <summary>
        /// The unique ID of the content item in the database.
        /// </summary>
        string DbId { get; }
    }
}