namespace GoodHamburger.Domain.Exceptions;

public class PedidoInvalidoException : DomainException
{
    public PedidoInvalidoException(string message): base(message){}
}