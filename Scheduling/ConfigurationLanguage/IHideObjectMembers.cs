using System;
using System.ComponentModel;

namespace Simple.Scheduler
{
    /// <summary>
    /// Helper interface, that hides the unwanted members of the object type from intellisense.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IHideObjectMembers
    {
        /// <summary>
        /// Gets the type of the particular instance.
        /// </summary>
        /// <returns>The <see cref="Type"/> for the instance.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        Type GetType();

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        int GetHashCode();

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> 
        /// is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> 
        /// to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> 
        /// is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool Equals(object obj);
    }
}