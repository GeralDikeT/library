using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Library.DateBase;

namespace Library
{
    public partial class MainWindow : Window
    {
        int i = 0;
        private DatabaseHelper db;
        public MainWindow()
        {
            InitializeComponent();
            Button_Delete.IsEnabled = false;
            Button_Edit.IsEnabled = false;
            db = new DatabaseHelper();
            LoadBooksFromDatabase();
        }

        private void Button_AddNote_Click(object sender, RoutedEventArgs e)
        {
            if (TextBox_Author.Text != "" &&
                TextBox_Description.Text != "" &&
                TextBox_NameOfBoock.Text != "" &&
                TextBox_PublishedYear.Text != "")
            {
                if (Button_AddNote.Content.ToString() == "Edit")
                {
                    BoockInfo selectedBoockInfo = (BoockInfo)ListBox_Boock.SelectedItem;

                    selectedBoockInfo.TextBlock_NameOfBoock.Text = TextBox_NameOfBoock.Text;
                    selectedBoockInfo.TextBlock_AuthorOfBoock.Text = TextBox_Author.Text;
                    selectedBoockInfo.TextBlock_PublishDate.Text = TextBox_PublishedYear.Text;
                    selectedBoockInfo.TextBlock_Description.Text = TextBox_Description.Text;

                    db.UpdateBook(new Book
                    {
                        Id = Convert.ToInt32(selectedBoockInfo.TextBlock_NumberOfBooock.Text.Split(':')[1].Trim()),
                        Title = TextBox_NameOfBoock.Text,
                        Author = TextBox_Author.Text,
                        Year = Convert.ToInt32(TextBox_PublishedYear.Text),
                        Description = TextBox_Description.Text
                    });

                    Button_AddNote.Content = "Add Book";
                }
                else
                {
                    Book book = new Book
                    {
                        Title = TextBox_NameOfBoock.Text,
                        Author = TextBox_Author.Text,
                        Year = Convert.ToInt32(TextBox_PublishedYear.Text),
                        Description = TextBox_Description.Text
                    };

                    var bookId = db.AddBook(book);

                    BoockInfo newBoockInfo = new BoockInfo
                    {
                        TextBlock_NameOfBoock = { Text = book.Title },
                        TextBlock_AuthorOfBoock = { Text = book.Author },
                        TextBlock_PublishDate = { Text = book.Year.ToString() },
                        TextBlock_Description = { Text = book.Description },
                        TextBlock_NumberOfBooock = { Text = $"Number Of Book: {ListBox_Boock.Items.Count + 1}" }
                    };

                    ListBox_Boock.Items.Add(newBoockInfo);
                }
            }
            else
            {
                MessageBox.Show("Error, Please, Enter all Fields!");
            }

            TextBox_Author.Text = "";
            TextBox_Description.Text = "";
            TextBox_NameOfBoock.Text = "";
            TextBox_PublishedYear.Text = "";
        }



        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            BoockInfo selectedBoockInfo = (BoockInfo)ListBox_Boock.SelectedItem;
            int bookId = Convert.ToInt32(selectedBoockInfo.TextBlock_NumberOfBooock.Text.Split(':')[1].Trim());
            db.DeleteBook(bookId);
            ListBox_Boock.Items.Remove(selectedBoockInfo);
            ReassignBookIds();
        }

        private void ListBox_Boock_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Button_Delete.IsEnabled = true;
            Button_Edit.IsEnabled = true;
        }

        private void Button_Edit_Click(object sender, RoutedEventArgs e)
        {
            ListBox_Boock.IsEnabled = false;
            Button_AddNote.Content = "Edit";

            BoockInfo selectedBoockInfo = (BoockInfo)ListBox_Boock.SelectedItem;
            TextBox_NameOfBoock.Text = selectedBoockInfo.TextBlock_NameOfBoock.Text;
            TextBox_Author.Text = selectedBoockInfo.TextBlock_AuthorOfBoock.Text;
            TextBox_PublishedYear.Text = selectedBoockInfo.TextBlock_PublishDate.Text;
            TextBox_Description.Text = selectedBoockInfo.TextBlock_Description.Text;
        }

        private void ReassignBookIds()
        {
            int id = 1;
            foreach (BoockInfo bookInfo in ListBox_Boock.Items)
            {
                bookInfo.TextBlock_NumberOfBooock.Text = $"Number Of Book: {id++}";
            }
        }
        private void LoadBooksFromDatabase()
        {
            var books = db.GetAllBooks();
            int id = 1;
            foreach (var book in books)
            {
                BoockInfo newBoockInfo = new BoockInfo
                {
                    TextBlock_NameOfBoock = { Text = book.Title },
                    TextBlock_AuthorOfBoock = { Text = book.Author },
                    TextBlock_PublishDate = { Text = book.Year.ToString() },
                    TextBlock_Description = { Text = book.Description },
                    TextBlock_NumberOfBooock = { Text = $"Number Of Book: {id++}" }
                };

                ListBox_Boock.Items.Add(newBoockInfo);
            }
        }

    }
}
