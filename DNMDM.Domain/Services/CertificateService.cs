using System.Security.Cryptography.X509Certificates;
using System.Text;
using DNMDM.Core;
using DNMDM.Domain.Abstractions;

namespace DNMDM.Domain.Services;

public class CertificateService : ICertificatesService
{
    private X509Certificate2? _rootCertificate;
    
    public X509Certificate2 GetOrRootCertificate()
    {
        if (_rootCertificate == null)
        {
            var certFile = Directory.GetFiles(Directory.GetCurrentDirectory(), "*Root.crt").FirstOrDefault();
            if (certFile != null)
            {
                var cert = new X509Certificate2(File.ReadAllBytes(certFile));
                _rootCertificate = cert;
                return _rootCertificate;

            }
            _rootCertificate = CertificareUtils.buildSelfSignedServerCertificate("Dynamic Mdm Root");
            StringBuilder builder = new StringBuilder();            

            builder.AppendLine("-----BEGIN CERTIFICATE-----");
            builder.AppendLine(Convert.ToBase64String(_rootCertificate.Export(X509ContentType.Cert), Base64FormattingOptions.InsertLineBreaks));
            builder.AppendLine("-----END CERTIFICATE-----");
            
            File.WriteAllText("Dynamic Mdm Root.crt", builder.ToString());
            return _rootCertificate;
        }

        return _rootCertificate;
    }
}