using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace DNMDM.Core;

public class CertificareUtils
{
    public static string GeneratePrivateKey(int bits = 2048)
    {
        using (var rsa = new RSACryptoServiceProvider(bits))
        {
            return rsa.ToXmlString(true);
        }
    }
    
    public static X509Certificate2 buildSelfSignedServerCertificate(string certificateName)
    {

        X500DistinguishedName distinguishedName = new X500DistinguishedName($"CN={certificateName},OU=Management,O=Vislobokov,C=RU");

        using (RSA rsa = RSA.Create(4096))
        {
            var request = new CertificateRequest(distinguishedName, rsa, HashAlgorithmName.SHA256,RSASignaturePadding.Pkcs1);
            request.CertificateExtensions.Add(
                new X509KeyUsageExtension(X509KeyUsageFlags.CrlSign | X509KeyUsageFlags.KeyCertSign, true));
            request.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
            request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(rsa.ExportSubjectPublicKeyInfo(), false));
            var certificate = request.Create(distinguishedName, X509SignatureGenerator.CreateForRSA(rsa, RSASignaturePadding.Pkcs1), DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddYears(5), new byte[]{1});// request.CreateSelfSigned(new DateTimeOffset(DateTime.UtcNow.AddDays(-1)), new DateTimeOffset(DateTime.UtcNow.AddDays(3650)));
            certificate.FriendlyName = certificateName;
            return certificate;
        }
    }
}