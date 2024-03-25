using dz_13._03.Models;

namespace dz_13._03.ViewModels
{
    public class BookViewModel : ViewModelBase
    {
        private Book Book;
        public BookViewModel(Book book)
        {
            Book = book;
        }
        public string Name
        {
            get { return Book.Name!; }
            set 
            { 
                Book.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public override string ToString()
        {
            return Book.Name!;
        }
        public int? AuthorId
        {
            get { return Book.AuthorId; }
            set
            {
                Book.AuthorId = value;
                OnPropertyChanged(nameof(AuthorId));
            }
        }
    }
}
