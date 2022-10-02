using AutoMapper;
using CardStorageService.Data;
using CardStorService.Models.Requests;
using CardStorService.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CardStorService.Controllers
{
    [Authorize]//включаем авторизацию
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        #region Services

        private readonly IClientRepositoryService _clientRepositoryService;
        private readonly ILogger<CardController> _logger;
        private readonly IValidator<CreateClientRequest> _createClientRequestValidator;//добавляем валидацию
        private readonly IMapper _mapper;//добавляем маппинг


        #endregion


        #region Constructors

        public ClientController(
            ILogger<CardController> logger,
            IClientRepositoryService clientRepositoryService,
            IValidator<CreateClientRequest> createClientRequestValidator,
            IMapper mapper)
        {
            _logger = logger;
            _clientRepositoryService = clientRepositoryService;
            _createClientRequestValidator = createClientRequestValidator;
            _mapper = mapper;
        }

        #endregion

        #region Pulbic Methods

        [HttpPost("create")]
        [ProducesResponseType(typeof(CreateClientResponse), StatusCodes.Status200OK)]
        public IActionResult Create([FromBody] CreateClientRequest request)
        {

            //добавляем валидаци
            //Получаем результат валидации из класс authenticationRequest
            ValidationResult validationResult = _createClientRequestValidator.Validate(request);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.ToDictionary());//метод .ToDictionary() - передает описание ошибки

            try
            {
                //var clientId = _clientRepositoryService.Create(new Client
                //{
                //    FirstName = request.FirstName,
                //    Surname = request.Surname,
                //    Patronymic = request.Patronymic
                //});
                var clientId = _clientRepositoryService.Create(_mapper.Map<Client>(request));//добавляем маппинг
                return Ok(new CreateClientResponse
                {
                    ClientId = clientId
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Create client error.");
                return Ok(new CreateCardResponse
                {
                    ErrorCode = 912,
                    ErrorMessage = "Create clinet error."
                });
            }
        }

        #endregion
    }
}
