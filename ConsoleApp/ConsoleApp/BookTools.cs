namespace DotNetChat
{
    public class BookTools
    {
        [Tool(Name = "book_info", Description = "Provides information about where to buy a particular book and how much it costs. Input should be the title of a book.")]
        public static Task<string> BookInfoAsync(string title)
        {
            switch (title)
            {
                case "Python Cookbook":
                    return Task.FromResult($"'{title}' is available on Amazon.com at a cost of $50.");
                case "Moby Dick":
                    return Task.FromResult($"'{title}' is available on Amazon.com at a cost of $10.");
                case "Love in the Time of Cholera":
                case "The Catcher in the Rye":
                case "The Great Gatsby":
                    return Task.FromResult($"'{title}' is available on Amazon.com at a cost of $15.");
                default:
                    return Task.FromResult($"Unfortunately '{title}' is not available.");
            }
        }

        [Tool(Name = "book_purchase", Description = "Executes a purchase of a book from Amazon.com. Input should be the title of a book.", ReturnDirect = true)]
        //[Tool(Name = "book_purchase", Description = "Executes a purchase of a book from Amazon.com. Input should be the title of a book.")]
        public static Task<string> BookPurchaseAsync(string title)
        {
            return Task.FromResult($"Most excellent news! Your book '{title}' has been ordered from Amazon.com.");
        }

        [Tool(Name = "cancel_book_order", Description = "Cancels an order for a particular book. For example, if the book is no longer wanted. Input should be the title of a book.")]
        public static Task<string> BookOrderCancelAsync(string title)
        {
            return Task.FromResult($"The order for the book '{title}' has been cancelled.");
        }
    }
}
