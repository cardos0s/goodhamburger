using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.Application.Exceptions;

public class RecursoNaoEncontradoException : DomainException
{
    public RecursoNaoEncontradoException(string mensagem) : base(mensagem) { }
}