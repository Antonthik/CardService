using CardStorService.Models.Requests;
using FluentValidation;

namespace CardStorService.Models.Validators
{
    //класс валидации для AuthenticationReques, на основе интерфейса  AbstractValidator пакета,
    // в качестве параметра указываем класс с плями,которые валидируем
    //Регестрируем в program - раздел Configure FluentValidator
    //Применяем в контролере
    public class AuthenticationRequestValidator : AbstractValidator<AuthenticationRequest>
    {
        //переопределяем конструктор
        public AuthenticationRequestValidator()
        {
            RuleFor(x => x.Login)//указаываем свойство
                .NotNull()//правило валидации
                .Length(5, 255)//правило валидации
                .EmailAddress();//правило валидации


            RuleFor(x => x.Password)
                .NotNull()
                .Length(5, 50);

        }
    }
}

