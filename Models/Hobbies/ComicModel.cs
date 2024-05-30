namespace TFGMaui.Models
{
    public class ComicModel : HobbieModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Copyright { get; set; }

        public string Description { get; set; }

        public int PageCount { get; set; }

        public PersonList Creators { get; set; }

        public PersonList Characters { get; set; }

        public string CoverImageUrl { get; set; }

        public string ResourceURI { get; set; }

        public string Format { get; set; }
    }

    public class PersonList
    {
        public int Available { get; set; }

        public sbyte CollectionURI { get; set; }

        public List<PersonC> creators { get; set; }
    }

    public class PersonC
    {
        public string ResourceURI { get; set; }

        public string Name { get; set; }

        public string Role { get; set; }
    }
}
