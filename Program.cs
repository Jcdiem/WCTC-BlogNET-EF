using System;
using NLog.Web;
using System.IO;
using System.Linq;

namespace BlogsConsole
{
    class Program
    {        
        // create static instance of Logger
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
        static void Main(string[] args)
        {
            logger.Info("Program started");
            try
            {
                BloggingContext db = new BloggingContext();
                bool choosingOption = true;
                while (choosingOption)
                {
                    Console.Write("Please choose an option: \n 1. Display Blogs \n 2. Add Blog \n 3. Display Posts \n 4. Add Post \n 5. Quit \n");
                    switch (Convert.ToInt32(Console.ReadLine()))
                    {
                        case 1:
                            // Display all Blogs from the database
                            var blogQuery = db.Blogs.OrderBy(b => b.Name);

                            Console.WriteLine("All blogs in the database:");
                            foreach (var item in blogQuery)
                            {
                                Console.WriteLine(item.Name);
                            }

                            choosingOption = false;
                            break;
                        case 2:
                            // Create and save a new Blog
                            Console.Write("Enter a name for a new Blog: ");
                            string name = Console.ReadLine();

                            //Check to make sure the string is not empty
                            if(name != null && name.Length > 0)
                            {
                                Blog blog = new Blog { Name = name };
                                db.AddBlog(blog);
                                logger.Info("Blog added - {name}", name);

                                choosingOption = false;
                            }
                            else
                            {
                                logger.Error("Name cannot be left blank");
                            }
                            
                            break;
                        case 3:
                            //Display all Posts from the database
                            var postQuery = db.Posts.OrderBy(p => p.Title);

                            Console.WriteLine("All posts in the database:");
                            foreach (var item in postQuery)
                            {
                                Console.WriteLine(item.Title);
                            }

                            choosingOption = false;
                            break;
                        case 4:
                            //Create and save a post

                            //Create the post object
                            Post newPost = new Post();

                            //Print out blogs and their id
                            Console.WriteLine("Gathering blogs, for reference...");
                            blogQuery = db.Blogs.OrderBy(b => b.Name);
                            foreach (var item in blogQuery)
                            {
                                Console.WriteLine("Blog: " + item.Name + " with ID " + item.BlogId);
                            }
                            Console.WriteLine("---End Blog Printing---");
                            
                            //Get the blog
                            Console.Write("Blog ID you would like to post to: ");
                            newPost.Blog = db.Blogs.Where(b => b.BlogId == Convert.ToInt32(Console.ReadLine())).FirstOrDefault();
                            newPost.BlogId = newPost.Blog.BlogId;

                            //Get the title
                            Console.Write("Title of your post: ");
                            newPost.Title = Console.ReadLine();

                            //Get the content
                            Console.WriteLine("Write your post below, then hit enter. \n (For new lines, please use the \\n operator)");
                            newPost.Content = Console.ReadLine();

                            //Check for errors
                            if (newPost.BlogId < 1 || newPost.Content.Length < 1 || newPost.Title.Length < 1)
                            {
                                logger.Error("Error in post creation"); 
                                break;
                            }

                            //Add the content
                            Console.WriteLine("Adding your content...");
                            db.AddPost(newPost);
                            Console.WriteLine("Done adding!");

                            choosingOption = false;
                            break;
                        case 5:
                            //Quit
                            choosingOption = false;
                            break;
                        default:
                            Console.WriteLine("Not understood, please try again.");
                            break;
                    }
                }                               
                
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            logger.Info("Program ended");
        }
    }
}
