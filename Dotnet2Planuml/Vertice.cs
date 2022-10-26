using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dotnet2Plantuml
{
    public class Vertice
    {
        public string ClassName { get; private set; }
        public ClassDeclarationSyntax? Declaration { get; set; }
        public List<PropertyDeclarationSyntax> Properties { get; private set; }
        public List<FieldDeclarationSyntax> Fields { get; private set; }
        public List<MethodDeclarationSyntax> Methods { get; private set; }

        public Vertice(string className, ClassDeclarationSyntax? declaration)
        {
            this.ClassName = className;
            this.Declaration = declaration;
            this.Properties = new List<PropertyDeclarationSyntax>();
            this.Fields = new List<FieldDeclarationSyntax>();
            this.Methods = new List<MethodDeclarationSyntax>();
        }

        public PropertyDeclarationSyntax AddPropertyDeclaration(PropertyDeclarationSyntax declaration)
        {
            Properties.Add(declaration);
            return declaration;
        }

        public FieldDeclarationSyntax AddFieldDeclaration(FieldDeclarationSyntax declaration)
        {
            Fields.Add(declaration);
            return declaration;
        }
        
        public MethodDeclarationSyntax AddMethodDeclaration(MethodDeclarationSyntax declaration)
        {
            Methods.Add(declaration);
            return declaration;
        }

        public string ToPlantUml()
        {
            var text = new List<string>();
            bool inNamespace = (Declaration?.Parent?.Kind() ?? SyntaxKind.None) == SyntaxKind.NamespaceDeclaration;
            
            if (inNamespace)
            {
                text.Add($"package {((NamespaceDeclarationSyntax) Declaration!.Parent!).Name.ToString()} {{");
            }
            
            text.Add($"  class {ClassName} {{");
            foreach (var property in Properties)
            {
                text.Add($"    {property.Identifier.Text} : {property.Type}");
            }
            foreach (var field in Fields)
            {
                foreach (var variables in field.Declaration.Variables)
                {
                    text.Add($"    {variables.Identifier.Text} : {field.Declaration.Type}");
                }
            }
            foreach (var methods in Methods)
            {
                text.Add($"    {methods.Identifier.Text}(...) : {methods.ReturnType}");
            }
            text.Add($"  }}");
            
            if (inNamespace)
                text.Add($"}}");
            
            return string.Join("\n", text);
        }

        public bool IsDeclared => Declaration is not null;
        public bool HasContent => Properties.Any() || Fields.Any() || Methods.Any();
    }
}
