﻿using CodeLab.Assets.EFUpdateWrapper;
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
            Book bk = new Book() { ID = 1 };
            EntityUpdateWrapper<Book> wrp = new EntityUpdateWrapper<Book>(bk, context, context.Books);
            wrp.Update<string>(b => b.Author, "New Auther");
            //wrp.Save();
        }
    }

    public partial class BookLibraryEntities : DbContext , IDirectUpdateContext
    {
        public DirectUpdateMode? CurrentSaveOperationMode { get; set; } = null;

        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            var result = base.ValidateEntity(entityEntry, items);
            return this.RemoveEFFalseAlarms(result, entityEntry, items);
        }

    }

}