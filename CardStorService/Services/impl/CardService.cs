using CardStorServiceProtos;
using Grpc.Core;
using static CardStorServiceProtos.CardService;

namespace CardStorService.Services.impl
{
    //Создаем класс на основе интерфейса сгенерированого автоматом obj\Debug\net6\Protos\CardstorageGrpc- он был сформирован при компиляции
    //Proro\cardstorage.proto - файл,где описывается интерфей для взамодействия клиента и сервиса - из пакета gRPC
    public class CardService: CardServiceBase                                                             
    {
        //По сути это контролер

        #region Services

        private readonly ICardRepositoryService _cardRepositoryService;

        #endregion


        public CardService(ICardRepositoryService cardRepositoryService)
        {
            _cardRepositoryService = cardRepositoryService;
        }

        public override Task<GetByClientIdResponse> GetByClientId(GetByClientIdRequest request, ServerCallContext context)
        {
            var response = new GetByClientIdResponse();

            response.Cards.AddRange(_cardRepositoryService.GetByClientId(request.ClientId.ToString())
                .Select(card => new Card
                {
                    CardNo = card.CardNo,
                    CVV2 = card.CVV2,
                    ExpDate = card.ExpDate.ToShortDateString(),
                    Name = card.Name
                }).ToList());

            return Task.FromResult(response);

        }
    }
}

