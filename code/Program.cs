using System;
using System.Collections.Generic;
using System.IO;

public class Livro
{
    public string Titulo { get; set; }
    public string Autor { get; set; }
    public string Genero { get; set; }
    public int Quantidade { get; set; }

    public Livro(string titulo, string autor, string genero, int quantidade)
    {
        Titulo = titulo;
        Autor = autor;
        Genero = genero;
        Quantidade = quantidade;
    }

    public override string ToString()
    {
        return $"{Titulo};{Autor};{Genero};{Quantidade}";
    }

    public static Livro FromString(string linha)
    {
        var partes = linha.Split(';');
        return new Livro(partes[0], partes[1], partes[2], int.Parse(partes[3]));
    }
}

public class Usuario
{
    public string Nome { get; set; }
    public bool EhAdministrador { get; set; }
    public List<Livro> LivrosEmprestados { get; set; }

    public Usuario(string nome, bool ehAdministrador)
    {
        Nome = nome;
        EhAdministrador = ehAdministrador;
        LivrosEmprestados = new List<Livro>();
    }

    public bool PodeEmprestar()
    {
        return LivrosEmprestados.Count < 3;
    }
}

public class Biblioteca
{
    private List<Livro> livros = new List<Livro>();
    private const string caminhoArquivo = "livros.txt";

    public Biblioteca()
    {
        CarregarLivros();
    }

    public void CadastrarLivro(Livro livro)
    {
        livros.Add(livro);
        SalvarLivros();
        Console.WriteLine("Livro cadastrado com sucesso!");
    }

    public void ConsultarCatalogo()
    {
        Console.WriteLine("Catálogo da Biblioteca:");
        foreach (var livro in livros)
        {
            Console.WriteLine($"Título: {livro.Titulo}, Autor: {livro.Autor}, Gênero: {livro.Genero}, Quantidade: {livro.Quantidade}");
        }
    }

    public void EmprestarLivro(string titulo, Usuario usuario)
    {
        var livro = livros.Find(l => l.Titulo.Equals(titulo, StringComparison.OrdinalIgnoreCase));
        if (livro != null && usuario.PodeEmprestar() && livro.Quantidade > 0)
        {
            usuario.LivrosEmprestados.Add(livro);
            livro.Quantidade--;
            SalvarLivros();
            Console.WriteLine($"Livro '{livro.Titulo}' emprestado para {usuario.Nome}.");
        }
        else
        {
            Console.WriteLine("Empréstimo não permitido: limite de livros ou estoque insuficiente.");
        }
    }

    public void DevolverLivro(string titulo, Usuario usuario)
    {
        var livro = usuario.LivrosEmprestados.Find(l => l.Titulo.Equals(titulo, StringComparison.OrdinalIgnoreCase));
        if (livro != null)
        {
            usuario.LivrosEmprestados.Remove(livro);
            livro.Quantidade++;
            SalvarLivros();
            Console.WriteLine($"Livro '{livro.Titulo}' devolvido por {usuario.Nome}.");
        }
        else
        {
            Console.WriteLine("Esse livro não está emprestado por você.");
        }
    }

    private void SalvarLivros()
    {
        using (var writer = new StreamWriter(caminhoArquivo))
        {
            foreach (var livro in livros)
            {
                writer.WriteLine(livro.ToString());
            }
        }
    }

    private void CarregarLivros()
    {
        if (File.Exists(caminhoArquivo))
        {
            var linhas = File.ReadAllLines(caminhoArquivo);
            foreach (var linha in linhas)
            {
                if (!string.IsNullOrWhiteSpace(linha))
                {
                    livros.Add(Livro.FromString(linha));
                }
            }
        }
    }

    public List<Livro> Livros => livros; // Para acesso externo aos livros
}

class ProgramaPrincipal
{
    static void Main()
    {
        Biblioteca biblioteca = new Biblioteca();
        Usuario admin = new Usuario("Admin", true);
        Usuario usuario = new Usuario("Usuario", false);

        while (true)
        {
            Console.WriteLine("Bem-Vindo(a) à biblioteca!");
            Console.WriteLine("\nMenu:");
            Console.WriteLine("1. Cadastrar Livro");
            Console.WriteLine("2. Consultar Catálogo");
            Console.WriteLine("3. Emprestar Livro");
            Console.WriteLine("4. Devolver Livro");
            Console.WriteLine("5. Sair");
            Console.Write("Escolha uma opção: ");

            if (!int.TryParse(Console.ReadLine(), out int opcao))
            {
                Console.WriteLine("Opção inválida. Por favor, tente novamente.");
                continue;
            }

            switch (opcao)
            {
                case 1:
                    if (admin.EhAdministrador)
                    {
                        Console.Write("Título: ");
                        string titulo = Console.ReadLine();
                        Console.Write("Autor: ");
                        string autor = Console.ReadLine();
                        Console.Write("Gênero: ");
                        string genero = Console.ReadLine();
                        Console.Write("Quantidade: ");
                        if (int.TryParse(Console.ReadLine(), out int quantidade))
                        {
                            Livro novoLivro = new Livro(titulo, autor, genero, quantidade);
                            biblioteca.CadastrarLivro(novoLivro);
                        }
                        else
                        {
                            Console.WriteLine("Quantidade inválida.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Acesso negado.");
                    }
                    break;
                case 2:
                    biblioteca.ConsultarCatalogo();
                    break;
                case 3:
                    Console.Write("Título do livro a ser emprestado: ");
                    string tituloEmprestimo = Console.ReadLine();
                    biblioteca.EmprestarLivro(tituloEmprestimo, usuario);
                    break;
                case 4:
                    Console.Write("Título do livro a ser devolvido: ");
                    string tituloDevolucao = Console.ReadLine();
                    biblioteca.DevolverLivro(tituloDevolucao, usuario);
                    break;
                case 5:
                Console.WriteLine("Obrigado e volte sempre!");
                    return;
                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }
        }
    }
}