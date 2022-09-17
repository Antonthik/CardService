using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CardStorService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        #region Service
        private readonly ILogger<CardController> _logger;
        #endregion

        #region Constructor
        public CardController(ILogger<CardController> logger)
        {
            _logger = logger;
        }
        #endregion

        #region Public Methods

        [HttpGet("getAll")]
        public IActionResult GetByclientId(string clientId)
        {
            _logger.LogInformation("GetByIdClient.......");
            return Ok();
        }
        #endregion
    }
}
