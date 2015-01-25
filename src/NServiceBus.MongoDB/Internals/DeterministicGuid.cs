namespace NServiceBus.MongoDB.Internals
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Security.Cryptography;
    using System.Text;

    internal static class DeterministicGuid
    {
        public static Guid Create(params object[] data)
        {
            Contract.Requires(data != null);

            // use MD5 hash to get a 16-byte hash of the string
            using (var provider = new MD5CryptoServiceProvider())
            {
                var inputBytes = Encoding.Default.GetBytes(string.Concat(data));

                //// TODO: provide extension method to normalize byte array to 16 length
                var hashBytes = provider.ComputeHash(inputBytes);

                // generate a GUID from the hash:
                Contract.Assume(hashBytes.Length == 16);
                return new Guid(hashBytes);
            }
        }
    }
}
