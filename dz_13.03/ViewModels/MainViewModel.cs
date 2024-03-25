using dz_13._03.Commands;
using dz_13._03.Models;
using dz_13._03.Views;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using static System.Reflection.Metadata.BlobBuilder;

namespace dz_13._03.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        public ObservableCollection<AuthorViewModel> AuthorList { get; set; }
        private ObservableCollection<BookViewModel> _books_all;

        private ObservableCollection<BookViewModel> _books;
        public ObservableCollection<BookViewModel> BookList
        {
            get { return _books; }
            set
            {
                _books = value;
                OnPropertyChanged(nameof(BookList));
            }
        }
        public MainViewModel(IQueryable<Author> authors, IQueryable<Book> books)
        {
            AuthorList = new ObservableCollection<AuthorViewModel>(authors.Select(g => new AuthorViewModel(g)));
            _books = new ObservableCollection<BookViewModel>(books.Select(g => new BookViewModel(g)));
            _books_all = new ObservableCollection<BookViewModel>(books.Select(g => new BookViewModel(g)));
        }
        private string name;
        public string AuthorName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged(nameof(AuthorName));
            }
        }
        private int index_selected_author;
        public int Index_selected_author
        {
            get { return index_selected_author; }
            set
            {
                index_selected_author = value;
                FilterBooksByAuthor();
                OnPropertyChanged(nameof(Index_selected_author));
            }
        }
        private int index_selected_book;
        public int Index_selected_book
        {
            get { return index_selected_book; }
            set
            {
                index_selected_book = value;
                OnPropertyChanged(nameof(Index_selected_book));
            }
        }
        private string bookName;
        public string BookName
        {
            get { return bookName; }
            set
            {
                bookName = value;
                OnPropertyChanged(nameof(BookName));
            }
        }
        private int? authorId;
        public int? AuthorId
        {
            get { return authorId; }
            set
            {
                authorId = value;
                OnPropertyChanged(nameof(AuthorId));
            }
        }

        private bool filterByAuthor;
        public bool FilterByAuthor
        {
            get { return filterByAuthor; }
            set
            {
                filterByAuthor = value;
                FilterBooksByAuthor();
                OnPropertyChanged(nameof(FilterByAuthor));
            }
        }
        private void FilterBooksByAuthor()
        {
            BookList = new ObservableCollection<BookViewModel>(_books_all);
            if (FilterByAuthor)
            {
                BookList = new ObservableCollection<BookViewModel>(_books.Where(book => book.AuthorId == Index_selected_author + 1));
            }
            else
            {
                BookList = new ObservableCollection<BookViewModel>(_books_all);
            }
        }

        private DelegateCommand exitApp;

        public ICommand ExitApp
        {
            get
            {
                if (exitApp == null)
                {
                    exitApp = new DelegateCommand(param => Exit(), param => CanExit());
                }
                return exitApp;
            }
        }
        public void Exit()
        {
            System.Windows.Application.Current.Shutdown();
        }
        private bool CanExit()
        {
            return true;
        }

        private DelegateCommand addAuthorcommand;
        public ICommand AddAuthorCommand
        {
            get
            {
                if (addAuthorcommand == null)
                    addAuthorcommand = new DelegateCommand(param => AddAuthorMenu(), param => CanAddAuthorMenu());
                return addAuthorcommand;
            }
        }
        private void AddAuthorMenu()
        {
            var addAuthorWindow = new AddAuthorWindow();
            addAuthorWindow.DataContext = this;
            addAuthorWindow.ShowDialog();
        }
        public bool CanAddAuthorMenu()
        {
            return true;
        }
        private DelegateCommand addAuthor;
        public ICommand AddAuthor
        {
            get
            {
                if (addAuthor == null)
                    addAuthor = new DelegateCommand(param => AddAuthorMethod(), param => CanAddAuthorMethod());
                return addAuthor;
            }
        }
        private void AddAuthorMethod()
        {
            try
            {
                using (var db = new AuthorBookContext())
                {
                    var author = new Author { Name = AuthorName };
                    db.Authors.Add(author);
                    db.SaveChanges();
                    var groupviewmodel = new AuthorViewModel(author);
                    AuthorList.Add(groupviewmodel);
                    System.Windows.Forms.MessageBox.Show("Автор добавлен!");
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
        private bool CanAddAuthorMethod()
        {
            return !AuthorName.IsNullOrEmpty();
        }
        private DelegateCommand deleteAuthor;
        public ICommand DeleteAuthor
        {
            get
            {
                if (deleteAuthor == null)
                    deleteAuthor = new DelegateCommand(param => DeleteAuthorMethod(), param => CanDeleteAuthorMethod());
                return deleteAuthor;
            }
        }
        public void DeleteAuthorMethod()
        {
            try
            {
                var delgroup = AuthorList[Index_selected_author];
                DialogResult result = System.Windows.Forms.MessageBox.Show("Вы действительно желаете удалить автора" + delgroup.Name +
                    " ?", "Удаление", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Cancel)
                    return;
                using (var db = new AuthorBookContext())
                {
                    var query = (from g in db.Authors
                                 where g.Name == delgroup.Name
                                 select g).Single();
                    db.Authors.Remove(query);
                    db.SaveChanges();
                    AuthorList.Remove(delgroup);
                    System.Windows.Forms.MessageBox.Show("Автор удален!");
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
        public bool CanDeleteAuthorMethod()
        {
            return Index_selected_author != -1;
        }
        private DelegateCommand editAuthorMenu;
        public ICommand EditAuthorMenu
        {
            get
            {
                if (editAuthorMenu == null)
                    editAuthorMenu = new DelegateCommand(param => EditAuthorWin(), param => CanEditAuthor());
                return editAuthorMenu;
            }
        }

        private void EditAuthorWin()
        {
            var editAuthorWindow = new EditAuthorWindow();
            editAuthorWindow.DataContext = this;
            editAuthorWindow.ShowDialog();
        }
        private bool CanEditAuthor()
        {
            return true;
        }

        private DelegateCommand editAuthor;
        public ICommand EditAuthor
        {
            get
            {
                if (editAuthor == null)
                    editAuthor = new DelegateCommand(param => EditAuthorMethod(), param => CanEditAuthorMethod());
                return editAuthor;
            }
        }
        public void EditAuthorMethod()
        {
            try
            {
                using (var db = new AuthorBookContext())
                {
                    var updategroup = AuthorList[Index_selected_author];
                    var query = (from g in db.Authors
                                 where g.Name == updategroup.Name
                                 select g).Single();
                    query.Name = AuthorName;
                    db.SaveChanges();
                    AuthorList[Index_selected_author] = new AuthorViewModel(query);
                    System.Windows.Forms.MessageBox.Show("Автор обновлен!");
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
        public bool CanEditAuthorMethod()
        {
            return !AuthorName.IsNullOrEmpty();
        }


        private DelegateCommand addBookWindow;
        public ICommand AddBookWindow
        {
            get
            {
                if (addBookWindow == null)
                    addBookWindow = new DelegateCommand(param => AddBookWin(), param => CanAddBookWin());
                return addBookWindow;
            }
        }
        public void AddBookWin()
        {
            var addBookWindow = new AddBookWindow();
            addBookWindow.DataContext = this;
            addBookWindow.ShowDialog();
        }
        public bool CanAddBookWin()
        {
            return true;
        }

        private DelegateCommand addBook;
        public ICommand AddBook
        {
            get
            {
                if (addBook == null)
                    addBook = new DelegateCommand(param => AddBookMethod(), param => CanAddBookMethod());
                return addBook;
            }
        }
        private void AddBookMethod()
        {
            try
            {
                using (var db = new AuthorBookContext())
                {
                    var book = new Book { Name = BookName};
                    db.Books.Add(book);
                    db.SaveChanges();
                    var groupviewmodel = new BookViewModel(book);
                    _books_all.Add(groupviewmodel);
                    System.Windows.Forms.MessageBox.Show("Книга добавлен!");
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
        private bool CanAddBookMethod()
        {
            return !BookName.IsNullOrEmpty();
        }

        private DelegateCommand deleteBook;
        public ICommand DeleteBook
        {
            get
            {
                if (deleteBook == null)
                    deleteBook = new DelegateCommand(param => DeleteBookMethod(), param => CanDeleteBookMethod());
                return deleteBook;
            }
        }
        public void DeleteBookMethod()
        {
            try
            {
                var delgroup = _books_all[Index_selected_book];
                DialogResult result = System.Windows.Forms.MessageBox.Show("Вы действительно желаете удалить книгу" + delgroup.Name +
                    " ?", "Удаление", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Cancel)
                    return;
                using (var db = new AuthorBookContext())
                {
                    var query = (from g in db.Books
                                 where g.Name == delgroup.Name
                                 select g).Single();
                    db.Books.Remove(query);
                    db.SaveChanges();
                    _books_all.Remove(delgroup);
                    System.Windows.Forms.MessageBox.Show("Книга удалена!");
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
        public bool CanDeleteBookMethod()
        {
            return Index_selected_book != null;
        }

        private DelegateCommand editBook;
        public ICommand EditBookWindow
        {
            get
            {
                if (editBook == null)
                    editBook = new DelegateCommand(param => EditBookMenu(), param => CanEditBookMenu());
                return editBook;
            }
        }
        public void EditBookMenu()
        {
            var editBookWindow = new EditBookWindow();
            editBookWindow.DataContext = this;
            editBookWindow.ShowDialog();
        }
        public bool CanEditBookMenu()
        {
            return true;
        }
        private DelegateCommand editBookWin;
        public ICommand EditBookMethod
        {
            get
            {
                if (editBookWin == null)
                    editBookWin = new DelegateCommand(param => EditBookWin(), param => CanEditBookWin());
                return editBookWin;
            }
        }
        public void EditBookWin()
        {
            try
            {
                using (var db = new AuthorBookContext())
                {
                    var updatebook = BookList[Index_selected_book];
                    var query = (from g in db.Books
                                 where g.Name == updatebook.Name
                                 select g).Single();
                    query.Name = BookName;
                    db.SaveChanges();
                    BookList[Index_selected_book] = new BookViewModel(query);
                    System.Windows.Forms.MessageBox.Show("Книга обновлена!");
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
        public bool CanEditBookWin()
        {
            return !BookName.IsNullOrEmpty();
        }
    }
}
