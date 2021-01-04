using Postident.Infrastructure.Services.DHL;

namespace Postident.Infrastructure.Interfaces.DHL
{
    public interface IValidationRequestXmlBuilderFactory
    {
        ValidationRequestXmlBuilder CreateInstance();
    }
}