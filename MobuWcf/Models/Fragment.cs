using System;
using System.Runtime.Serialization;

namespace MobuWcf.Models
{
    /// <summary>
    /// Model used to contain details of a single  fragment. 
    /// </summary>
    [DataContract]
    public class Fragment
    {
        /// <summary>
        /// Details of the APK this fragment belongs to.
        /// </summary>
        [DataMember]
        public PackageDetails PackageDetails;

        /// <summary>
        /// Index of this fragment in the list being downloaded/uploaded.
        /// The size of these fragments is specified when the download/upload is initiated.
        /// </summary>
        [DataMember]
        public int Index;

        /// <summary>
        /// The APK data contained in this fragment.
        /// </summary>
        [DataMember]
        public byte[] Data;

        /// <summary>
        /// Default constructor for fragments
        /// </summary>
        /// <param name="packageDetails"> Details of the APK this fragment belongs to</param>
        /// <param name="index">Index of this fragment in the list being downloaded/uploaded.</param>
        /// <param name="data">The APK data contained in this fragment.</param>
        public Fragment(PackageDetails packageDetails, int index, byte[] data)
        {
            PackageDetails = packageDetails;
            Index = index;
            Data = data;
        }
    }
}