using dz_13._03.Models;

namespace dz_13._03.ViewModels
{
    public class AuthorViewModel : ViewModelBase
    {
        private Author Author;
        public AuthorViewModel(Author author)
        {
            Author = author;
        }
        public override string ToString()
        {
            return Author.Name!;
        }
        public string Name
        {
            get { return Author.Name!; }
            set
            {
                Author.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }
}
