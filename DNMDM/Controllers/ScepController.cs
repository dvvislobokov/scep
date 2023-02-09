using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using DNMDM.Core;
using DNMDM.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace DNMDM.Controllers
{

    public enum ScepOperationType
    {
        GetCACert,
        PKIOperation,
        GetCACaps
    }
    public class ScepOperationRequest
    {
        [Required]
        [FromQuery(Name = "operation")]
        public ScepOperationType Operation { get; set; }
    
        [FromQuery(Name = "message")]
        public string? Message { get; set; }
    }
    
    [Route("/scep")]
    [ApiController]
    public class ScepController : ControllerBase
    {
        private readonly string[] scepOperations = new[] { "GetCACert", "PKIOperation", "GetCACaps" };
        private readonly ILogger<ScepController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICertificatesService _certificatesService;

        public ScepController(ILogger<ScepController> logger, IHttpContextAccessor contextAccessor, ICertificatesService certificatesService)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _certificatesService = certificatesService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Index([FromQuery]ScepOperationRequest request, CancellationToken cancellationToken)
        {
            var body = "";
            using (var str = new StreamReader(_contextAccessor.HttpContext.Request.Body))
            {
                body = await str.ReadToEndAsync();
            }
            switch (request.Operation)
            {
                case ScepOperationType.GetCACaps:
                    return Content(await GetCACaps());
                case ScepOperationType.GetCACert:
                    return GetCaCert();
                case ScepOperationType.PKIOperation:
                    break;
                default:
                    throw new Exception("operation not permitted");
            }
                

            return Ok();
        }
        
        [HttpPost]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> IndexPost(CancellationToken cancellationToken)
        {
            var body = "";
            using (var str = new StreamReader(_contextAccessor.HttpContext.Request.Body))
            {
                str.ReadToEndAsync()
                var s = new MemoryStream();
                await _contextAccessor.HttpContext.Request.Body.CopyToAsync(s);
                SignedCms cms = new SignedCms();
                cms.Decode(s.ToArray());
            }

            return Ok();
        }

        private IActionResult GetCaCert()
        {
            var cert = _certificatesService.GetOrRootCertificate();
            var collection = new X509Certificate2Collection(cert);
            return File(collection.Export(X509ContentType.Cert), "application/x-x509-ca-cert");
        }

        private async Task<string> GetCACaps()
        {
            return @"Renewal
SHA-1
SHA-256
AES
DES3
SCEPStandard
POSTPKIOperation";
        }
    }
}