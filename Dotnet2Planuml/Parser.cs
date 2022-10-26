using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dotnet2Plantuml
{
    public class Parser
    {
        public Graph CurrentGraph { get; private set; } = new Graph();

        public void ParseFile(string fileName)
        {
            if (!fileName.EndsWith(".cs")) return;
            
            string text;
            try
            {
                text = File.ReadAllText(fileName);
            }
            catch (System.Exception)
            {
                throw;
            }
            SyntaxTree tree = CSharpSyntaxTree.ParseText(text);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            Scaffold(root.Members);
        }
        
        private void Scaffold(SyntaxList<MemberDeclarationSyntax> members)
        {
            foreach (var member in members)
            {
                if (member.IsKind(SyntaxKind.NamespaceDeclaration))
                {
                    NamespaceDeclaration(member);
                }

                if (member.IsKind(SyntaxKind.ClassDeclaration))
                {
                    ClassDeclaration(member);
                }
            }
        }

        private void MethodDeclaration(Vertice parent, MemberDeclarationSyntax member)
        {
            parent.AddMethodDeclaration((MethodDeclarationSyntax) member);
        }

        private void PropertyDeclaration(Vertice parent, MemberDeclarationSyntax member)
        {
            var property = parent.AddPropertyDeclaration((PropertyDeclarationSyntax) member);
            FindingRelations(parent, property.Type);
        }

        private void FieldDeclaration(Vertice parent, MemberDeclarationSyntax member)
        {
            var field = parent.AddFieldDeclaration((FieldDeclarationSyntax) member);
            FindingRelations(parent, field.Declaration.Type);
        }

        private void FindingRelations(Vertice parent, TypeSyntax type)
        {
            Vertice vertice;
            string relation = "..";

            if (type.IsKind(SyntaxKind.PredefinedType))
                return;

            if (type.Parent.IsKind(SyntaxKind.VariableDeclaration) || type.Parent.IsKind(SyntaxKind.PropertyDeclaration))
            {
                if (type.IsKind(SyntaxKind.IdentifierName))
                {
                    relation = "*--";
                }
            }

            if (type.Parent.IsKind(SyntaxKind.SimpleBaseType))
            {
                relation = "--|>";
            }

            if (type.Parent.IsKind(SyntaxKind.TypeArgumentList))
            {
                if (type.Parent.Parent.IsKind(SyntaxKind.GenericName))
                {
                    if (((GenericNameSyntax) type.Parent.Parent).Identifier.Text == "List")
                    {
                        relation = "o--";
                    }
                }
            }

            if (type.IsKind(SyntaxKind.GenericName))
            {
                var generic = (GenericNameSyntax) type;
                vertice = CurrentGraph.AddVertice(generic.Identifier.Text);

                CurrentGraph.AddLink(parent, vertice, relation);

                foreach (var item in generic.TypeArgumentList.Arguments)
                {
                    FindingRelations(parent, item);
                }
            }
            else
            {
                vertice = CurrentGraph.AddVertice(type.ToString());
                CurrentGraph.AddLink(parent, vertice, relation);
            }
        }

        private void ClassDeclaration(MemberDeclarationSyntax member)
        {
            var vertice = CurrentGraph.AddVertice((ClassDeclarationSyntax) member);

            ClassScaffold(vertice);

            if (vertice.Declaration!.BaseList is not null)
            {
                foreach (var baseClass in vertice.Declaration.BaseList.Types)
                {
                    FindingRelations(vertice, baseClass.Type);
                }
            }
        }

        private void ClassScaffold(Vertice vertice)
        {
            foreach (var item in vertice.Declaration!.Members)
            {
                if (item.IsKind(SyntaxKind.FieldDeclaration))
                {
                    FieldDeclaration(vertice, item);
                }

                if (item.IsKind(SyntaxKind.PropertyDeclaration))
                {
                    PropertyDeclaration(vertice, item);
                }

                if (item.IsKind(SyntaxKind.MethodDeclaration))
                {
                    MethodDeclaration(vertice, item);
                }
            }
        }

        private void NamespaceDeclaration(MemberDeclarationSyntax member)
        {
            var ns = (NamespaceDeclarationSyntax)member;
            Scaffold(ns.Members);
        }
    }
}
