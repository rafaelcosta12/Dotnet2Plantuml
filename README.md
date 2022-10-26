# Dotnet2Plantuml
Transforma código C# em Plantuml

# Instalar
Dentro da pasta do projeto, executar o comando:
```
dotnet publish --sc --runtime linux-x64 --output {{saida}}
```

Onde **saida** é onde os arquivos compilados vão ficar.

Para poder acessar o dotnet2plantuml de qualquer lugar, adicione a pasta `{{saida}}` ao PATH.
Execute o comando abaixo e reinie o terminal:
```
echo "export PATH=\$PATH:{{saida}}" >> $HOME/.bashrc
```

# Como usar
O comando **dotnet2plantuml** tem o seguinte formato:

```dotnet2plantuml [options] <files>...```

As opções disponíveis são:
* --classes \<classname\>

Especifica o ponto de partida do diagrama que vai ser gerado. Apenas a classe espceficada e suas depêndencias aparecerão no diagrama. Caso não seja especificado todas as classes encontradas serão mostradas no diagrama.

* --output \<file\> 

Diz ao programa para salvar o conteúdo no arquivo *\<file\>*.

Para gerar o diagrama a partir do conteúdo PlantUml, pode-se usar os sites

* [Plant Text UML Editor](https://www.planttext.com/)

* [PlantUML Web Server](http://www.plantuml.com/plantuml/uml/)

# Exemplos
Supondo que o arquivo *Escola.cs* contem o seguinte conteudo:
```
namespace Escola
{
    public class Pessoa
    {
        public string? Nome { get; set; }
    }

    public class Aluno : Pessoa
    {
        public string? NomeDaMae { get; set; }
        public string? NomeDoPai { get; set; }
    }

    public class Professor : Pessoa
    {
        public List<Aula> Aulas { get; set; } = new();
    }

    public class Turma
    {
        public List<Aluno> Alunos { get; set; } = new();
    }

    public class Aula
    {
        public string? Nome { get; set; }
        public DateTime Horario { get; set; }
        public Professor? Professor { get; set; }
    }
}
```

Rodando o comando:
```
dotnet2plantuml Escola.cs
```

Vai gerar o seguinte conteúdo PlantUML:
```
' Aluno.cs
@startuml
!theme mars
package Escola {
  class Pessoa {
    Nome : string?
  }
}
package Escola {
  class Aluno {
    NomeDaMae : string?
    NomeDoPai : string?
  }
}
package Escola {
  class Professor {
    Aulas : List<Aula>
  }
}
package Escola {
  class Aula {
    Nome : string?
    Horario : DateTime
    Professor : Professor?
  }
}
package Escola {
  class Turma {
    Alunos : List<Aluno>
  }
}
Aluno --|> Pessoa
Professor o-- Aula
Professor --|> Pessoa
Turma o-- Aluno
@enduml
```

Se olhar bem, todos as classes aparecem no conteudo gerado.

![Diagrama Completo](/assets/Diagrama-Escola-Completo.png "Diagrama Escola Completo")

Agora usando a opção *--classes*
```
dotnet2plantuml Aluno.cs --classes Aluno
```

Saída:
```
' Aluno.cs
@startuml
!theme mars
package Escola {
  class Aluno {
    NomeDaMae : string?
    NomeDoPai : string?
  }
}
package Escola {
  class Pessoa {
    Nome : string?
  }
}
Aluno --|> Pessoa
@enduml
```

O conteúdo gerado é reduzido, somente com a classe Aluno e suas depêndencias. Isso pode ser útil em ambientes com uma quantidade de classes muito grande, ou para se analisar somente uma parte do diagrama.

![Diagrama Aluno](/assets//Diagrama-Escola-Aluno.png "Diagrama Escola Visão Aluno")

Isso vale para qualquer classe.

# Obserções
* Classes referênciadas que não foram encontrada a declaração nos arquivos, não serão exibidas como entidades e nem seus relacionamentos.

* Classes declaradas que não tem nenhum conteudo (Propriedades, Campos ou Metodos) não serão mostradas.

* Somente é considerado o relacionamento de agregação classes contidas em uma *List*.

# Ideias de melhorias
* Mostrar modificador de acesso das Propriedades, Campos e Métodos.
