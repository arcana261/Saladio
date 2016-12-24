using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Saladio.Contexts;
using IO.Swagger.Model;

namespace Saladio.Adapters
{
    public class SaladListItemGroup
    {
        public string Name { get; set; }
        public IList<SaladListItem> Items { get; set; }

        public static IList<SaladListItemGroup> GetGroups(SaladioContext context, IList<SavedSalad> savedSalads)
        {
            IList<SaladListItemGroup> result = new List<SaladListItemGroup>();
            result.Add(new SaladListItemGroup(context, savedSalads));

            return result;
        }

        public static IList<SaladListItemGroup> GetGroups(SaladioContext context, IList<ClassicSaladCatagory> catagories)
        {
            return catagories.Select(x => new SaladListItemGroup(context, x)).ToList();
        }

        public SaladListItemGroup(SaladioContext context, IList<SavedSalad> savedSalads)
        {
            Name = "سالادهای من";
            Items = savedSalads.Select(x => new SaladListItem(context, x)).ToList();

            foreach (var item in Items)
            {
                item.Group = this;
            }
        }

        public SaladListItemGroup(SaladioContext context, ClassicSaladCatagory catagory)
        {
            Name = catagory.Name;
            Items = context.GetClassicSaladsByCatagory(catagory).Select(x => new SaladListItem(context, x)).ToList();

            foreach (var item in Items)
            {
                item.Group = this;
            }
        }

        public SaladListItemGroup()
        {

        }
    }
}