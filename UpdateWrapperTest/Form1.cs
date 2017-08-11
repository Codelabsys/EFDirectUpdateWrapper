using CodeLab.Assets.EFUpdateWrapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UpdateWrapperTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BookLibraryEntities context = new BookLibraryEntities();
            
            //  not loaded
            Book bk = new Book() { ID = 1 };
            context.PrepareEntityForUpdate(bk, context.Books);
            bk.Author = "redaadad";
            context.SaveChanges(DirectUpdateMode.AllowAll);

            //not loaded more than one prop
            //Book bk = new Book() { ID = 1 };
            //context.PrepareEntityForUpdate(bk, context.Books);
            //bk.Author = "Mohamed";
            //bk.Name = "ahmed";
            //context.SaveChanges(DirectUpdateMode.AllowAll);


            //loaded
            //Book b = context.Books.First(bk => bk.ID == 1);
            //b.Author = "my updsgdsgsdfated aasdasduthor";
            //context.PrepareEntityForUpdate(b, context.Books);
            //context.SaveChanges(DirectUpdateMode.AllowAll);

            //loaded   more than one prop
            //Book b = context.Books.First(bk => bk.ID == 1);
            //b.Author = "My Updsgdsgsdfated Aasdasduthor";
            //b.Name = "adadadad";
            //context.PrepareEntityForUpdate(b, context.Books);
            //context.SaveChanges(DirectUpdateMode.AllowAll);

            //loaded more than one entity
            //Book b = context.Books.First(bk => bk.ID == 1);
            //BookCategory bc = context.BookCategories.First(bb => bb.ID == 1);
            //b.Author = "My Updsgdsgsdfated Aasdasduthordodo";
            //bc.Name = "adadadaddodo";
            //context.PrepareEntityForUpdate(b, context.Books);
            //context.PrepareEntityForUpdate(bc, context.BookCategories);
            //context.SaveChanges(DirectUpdateMode.AllowAll);


            //not loaded more than one entity
            //Book b = new Book() { ID = 1 };
            //BookCategory bc = new BookCategory() { ID = 1 };
            //context.PrepareEntityForUpdate(b, context.Books);
            //context.PrepareEntityForUpdate(bc, context.BookCategories);
            //b.Name = "two entity";
            //bc.Name = "two entity cat";
            //context.SaveChanges(DirectUpdateMode.AllowAll);


            //Edit(update) not loaded while loaded exist
            //Book b = context.Books.First(bk => bk.ID == 1);
            //Book notloaded = new Book() { ID = 1 };

            //b.Author = "My Updsgdsgsdfated Aasdasduthorthor";
            //context.PrepareEntityForUpdate(b, context.Books);

            //notloaded.Author = "My not loaded";
            //context.PrepareEntityForUpdate(notloaded, context.Books);

            //context.SaveChanges(DirectUpdateMode.AllowAll);


            //  Edit(update) loaded while not loaded exist                Loaded always has the priority
            //Book b = context.Books.First(bk => bk.ID == 1);
            //Book notloaded = new Book() { ID = 1 };
            //notloaded.Author = "My not loaded";
            //context.PrepareEntityForUpdate(notloaded, context.Books);

            //b.Author = "My Loaded";
            //context.PrepareEntityForUpdate(b, context.Books);

            //context.SaveChanges(DirectUpdateMode.AllowAll);




            //not loaded with another property                 fail

            //  Book bb = new Book() { ID = 1 };
            //Book bkk = context.Books.First(b => b.ID == 1);


            //context.PrepareEntityForUpdate(bkk, context.Books);
            //bkk.Author = "Mohamedddddeee";
            //context.SaveChanges(DirectUpdateMode.AllowAll);






        }
    }

    public partial class BookLibraryEntities : DbContext, IDirectUpdateContext
    {
        public DirectUpdateMode? CurrentSaveOperationMode { get; set; } = null;

        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            var result = base.ValidateEntity(entityEntry, items);
            return this.RemoveEFFalseAlarms(result, entityEntry);
        }

    }

}
