﻿using CardStorageService.Data;
using CardStorService.Models;
using CardStorService.Models.Requests;
using CardStorService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CardStorService.Controllers
{
    [Authorize]//включаем авторизацию
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        #region Service
        private readonly ILogger<CardController> _logger;
        private readonly ICardRepositoryService _cardRepositoryService;

        #endregion

        #region Constructor
        public CardController(ILogger<CardController> logger,
           ICardRepositoryService cardRepositoryService)
        {
            _logger = logger;
            _cardRepositoryService = cardRepositoryService;
        }
        #endregion

        #region Public Methods

        [HttpPost("create")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult Create([FromBody] CreateCardRequest request)
        {
            try
            {
                var cardId = _cardRepositoryService.Create(new Card
                {
                    ClientId = request.ClientId,
                    CardNo = request.CardNo,
                    Name = request.Name,//
                    ExpDate = request.ExpDate,
                    CVV2 = request.CVV2
                });
                return Ok(new CreateCardResponse
                {
                    CardId = cardId.ToString()
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Create card error.");
                return Ok(new CreateCardResponse
                {
                    ErrorCode = 1012,
                    ErrorMessage = "Create card error."
                });
            }
        }

        [HttpGet("get-by-client-id")]
        [ProducesResponseType(typeof(GetCardsResponse), StatusCodes.Status200OK)]
        public IActionResult GetByClientId([FromQuery] string clientId)
        {
            try
            {
                var cards = _cardRepositoryService.GetByClientId(clientId);
                return Ok(new GetCardsResponse
                {
                    Cards = cards.Select(card => new CardDto
                    {
                        CardNo = card.CardNo,
                        CVV2 = card.CVV2,
                        Name = card.Name,
                        ExpDate = card.ExpDate.ToString("MM/yy")
                    }).ToList()
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Get cards error.");
                return Ok(new GetCardsResponse
                {
                    ErrorCode = 1013,
                    ErrorMessage = "Get cards error."
                });
            }
        }
        #endregion
    }
}
