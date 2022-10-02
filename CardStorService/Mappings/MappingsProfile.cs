using AutoMapper;
using CardStorageService.Data;
using CardStorService.Models;
using CardStorService.Models.Requests;

namespace CardStorService.Mappings
{
    //Класс привидения от одного класса к другому при сходстве полей
    //Конфигурация маппера
    //Регистрируем в Program
    public class MappingsProfile:Profile
    {
        public MappingsProfile()
        {
            CreateMap<Card, CardDto>();//правила привидения от Card к CardDto
            CreateMap<CreateCardRequest, Card>();
            CreateMap<CreateClientRequest, Client>();
        }

    }
}
