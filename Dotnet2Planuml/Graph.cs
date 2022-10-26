using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dotnet2Plantuml
{
    public class Graph
    {
        public HashSet<Vertice> Vertices { get; private set; }
        public HashSet<Link> Links { get; private set; }

        public Graph()
        {
            this.Vertices = new HashSet<Vertice>();
            this.Links = new HashSet<Link>();
        }

        public Vertice AddVertice(string className)
        {
            var exists = Vertices.FirstOrDefault(i => i.ClassName == className);
            if (exists is not null) return exists;

            Vertice vertice = new Vertice(className, null);
            Vertices.Add(vertice);
            return vertice;
        }

        public Vertice AddVertice(ClassDeclarationSyntax declaration)
        {
            var exists = Vertices.FirstOrDefault(i => i.ClassName == declaration.Identifier.Text);
            if (exists is not null)
            {
                exists.Declaration = declaration;
                return exists;
            }

            Vertice vertice = new Vertice(declaration.Identifier.Text, declaration);
            Vertices.Add(vertice);
            return vertice;
        }

        public Link AddLink(Vertice a, Vertice b, string relationType)
        {
            var exists = Links.FirstOrDefault(i => i.From.ClassName == a.ClassName && i.To.ClassName == b.ClassName && i.RelationType == relationType);
            if (exists is not null)
            {
                return exists;
            }
            Link link = new Link(a, b, relationType);
            Links.Add(link);
            return link;
        }

        public string AsText()
        {
            var content = new List<string>();
            content.Add("@startuml\n!theme mars");
            foreach (var item in Vertices)
            {
                if (!item.IsDeclared)
                {
                    continue;
                }
                if (!item.HasContent)
                {
                    continue;
                }
                content.Add(item.ToPlantUml());
            }
            foreach (var item in Links)
            {
                if (!item.From.HasContent || !item.From.IsDeclared)
                {
                    continue;
                }
                if (!item.To.HasContent || !item.To.IsDeclared)
                {
                    continue;
                }
                content.Add(item.ToPlantUml());
            }
            content.Add("@enduml");
            return string.Join("\n", content);
        }

        public string AsText(List<string> classes)
        {
            var passeds = new List<Vertice>();
            var content = new List<string>();

            content.Add("@startuml\n!theme mars");
            
            foreach (var item in classes)
            {
                var vertice = Vertices.FirstOrDefault(i => i.ClassName == item);
                if (vertice is null) continue;
                PrintScaffold(vertice, passeds, content);
            }

            content.Add("@enduml");
            content.Add("");
            
            return string.Join("\n", content);
        }

        private void PrintScaffold(Vertice vertice, List<Vertice> passeds, List<string> content)
        {
            if (passeds.Contains(vertice)) return;
            
            if (vertice.HasContent && vertice.IsDeclared)
                content.Add(vertice.ToPlantUml());
            
            passeds.Add(vertice);

            foreach (var item in Links.Where(i => i.From == vertice))
            {
                PrintScaffold(item.To, passeds, content);
                if (item.To.HasContent && item.To.IsDeclared)
                    content.Add(item.ToPlantUml());
            }
        }
    }
}