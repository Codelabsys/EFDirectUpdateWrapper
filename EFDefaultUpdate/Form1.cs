using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EFDefaultUpdate
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BookLibraryEntities ctx = new BookLibraryEntities();
            Book b = ctx.Books.First(bk => bk.ID == 1);
            b.Author = "My Updated Author";
            ctx.SaveChanges();

            //=========================


        }

        private void button2_Click(object sender, EventArgs e)
        {
           
            try
            {
                BookLibraryEntities ctx = new BookLibraryEntities();
                Book b = new Book();
                b.ID = 1;
                ctx.Books.Attach(b);
                b.Author = "My Updated Author2";
                ctx.SaveChanges();


            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {

            }
        }
    }
}
