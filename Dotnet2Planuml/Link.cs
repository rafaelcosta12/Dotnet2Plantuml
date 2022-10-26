namespace Dotnet2Plantuml
{
    public class Link
    {
        public Vertice From { get; private set; }
        public Vertice To { get; private set; }
        public string RelationType { get; set; }

        public Link(Vertice from, Vertice to, string relationType)
        {
            this.From = from;
            this.To = to;
            this.RelationType = relationType;
        }

        public string ToPlantUml()
        {
            return $"{From.ClassName} {RelationType} {To.ClassName}";
        }
    }
}