using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp.Models.Items
{
    public abstract class Item
    {
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public string ImagePath { get; protected set; }

        protected Item(string name, string description, string imagePath)
        {
            Name = name;
            Description = description;
            ImagePath = imagePath;
        }
    }
}
