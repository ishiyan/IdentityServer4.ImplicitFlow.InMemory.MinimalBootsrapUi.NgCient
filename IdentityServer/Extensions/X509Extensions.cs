using System.Security.Cryptography.X509Certificates;

namespace IdentityServer.Extensions
{
    public static class X509Extensions
    {
        public static X509Certificate2 GetX509Certificate(this string thumbprint)
        {
            using (X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                certStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
                certStore.Close();
                return certCollection.Count > 0 ? certCollection[0] : null;
            }
        }
    }
}
