using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Domain.Entities;

public class Produto
{
    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public decimal Preco { get; private set; }
    public CategoriaProduto Categoria { get; private set; }
    
    private Produto(){}

    public Produto(string nome, decimal preco, CategoriaProduto categoria)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do produto é obrigatório.", nameof(nome));
        if (preco <= 0)
            throw new ArgumentException("Preço deve ser positivo.", nameof(preco));

        
        Nome = nome;
        Preco = preco;
        Categoria = categoria;
    }
}