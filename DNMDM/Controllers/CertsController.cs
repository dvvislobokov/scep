using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DNMDM.Core;
using DNMDM.Domain.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DNMDM.Controllers
{
    [Route("/api/v1/certs")]
    [ApiController]
    public class CertsController : ControllerBase
    {
        private readonly ICertificatesService _certificatesService;
        public CertsController(ICertificatesService certificatesService)
        {
            _certificatesService = certificatesService;
        }


        [HttpGet("root")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateRootCertificate(string certname)
        {
            var cert = _certificatesService.GetOrRootCertificate();
            return File(cert.RawData, "application/pkix-cert",
                $"{cert.FriendlyName}.crt");
        }
    }
}
