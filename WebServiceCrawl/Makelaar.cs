

namespace WebServiceCrawl
{
    /// <summary>
    /// A simple Makelaar object to hold his values 
    /// </summary>
    public class Makelaar
    {
        public int MakelaarId { get; set; }
        public string MakelaarNaam { get; set; }

        #region override Equals(), overload == operator

        public override bool Equals(object other)
        {
            // If parameter is null return false.
            if (other == null)
            {
                return false;
            }

            // If parameter cannot be cast to Makelaar return false.
            var osp = other as Makelaar;
            if (osp == null)
            {
                return false;
            }

            // Return true if the MakelaarId match:
            return (MakelaarId == osp.MakelaarId);
        }

        public bool Equals(Makelaar other)
        {
            if (other == null) return false;
            return (MakelaarId == other.MakelaarId);
        }

        public override int GetHashCode()
        {
            var hash = (MakelaarId + MakelaarNaam).GetHashCode();
            return hash;
        }

        public static bool operator ==(Makelaar m1, Makelaar m2)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(m1, m2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)m1 == null) || ((object)m2 == null))
            {
                return false;
            }

            // Return true if the fields match:
            return m1.Equals(m2);
        }

        public static bool operator !=(Makelaar m1, Makelaar m2)
        {
            return !(m1 == m2);
        }

        #endregion
    }
}
