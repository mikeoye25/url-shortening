using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using URLShortening.Application.Interfaces;
using URLShortening.Application.ViewModels;

namespace URLShortening.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class UrlShorteningController : ControllerBase
    {
        private readonly IUrlShorteningService UrlShorteningService;

        public UrlShorteningController(IUrlShorteningService urlShorteningService)
        {
            UrlShorteningService = urlShorteningService ?? throw new ArgumentNullException(nameof(urlShorteningService));
        }

        // 1. POST /shorten – Accepts a long URL and returns a shortened URL.
        [EnableRateLimiting("fixed")]
        [HttpPost("shorten")]
        [ProducesResponseType(typeof(ShortenUrlResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShortenUrlResponse>> ShortenUrl([FromBody] ShortenUrlRequest request)
        {
            var response = await UrlShorteningService.ShortenUrl(request);
            return Ok(response);
        }

        // 2. GET /{shortUrl} – Accepts a short URL and redirects to the original long URL.
        [HttpGet("{shortUrl}")]
        [ProducesResponseType(typeof(RetrieveUrlResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<RetrieveUrlResponse>> RetrieveUrl(string shortUrl)
        {
            var response = await UrlShorteningService.RetrieveUrl(shortUrl);
            return Ok(response);
        }
    }
}
