namespace GoodHamburger.Web.Services;

public enum ToastTipo { Sucesso, Erro, Aviso }

public record ToastMessage(string Texto, ToastTipo Tipo, Guid Id);

public class ToastService
{
    public event Action<ToastMessage>? OnShow;

    public void Sucesso(string texto) => Mostrar(texto, ToastTipo.Sucesso);
    public void Erro(string texto) => Mostrar(texto, ToastTipo.Erro);
    public void Aviso(string texto) => Mostrar(texto, ToastTipo.Aviso);

    private void Mostrar(string texto, ToastTipo tipo)
        => OnShow?.Invoke(new ToastMessage(texto, tipo, Guid.NewGuid()));
}