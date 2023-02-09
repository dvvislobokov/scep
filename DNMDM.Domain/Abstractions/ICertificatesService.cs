using System.Security.Cryptography.X509Certificates;

namespace DNMDM.Domain.Abstractions;

public interface ICertificatesService
{
    X509Certificate2 GetOrRootCertificate();
}