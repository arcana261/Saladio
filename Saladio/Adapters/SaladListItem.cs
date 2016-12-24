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
using IO.Swagger.Model;
using Saladio.Contexts;

namespace Saladio.Adapters
{
    public class SaladListItem
    {
        public string Name { get; set; }
        public string Ingredients { get; set; }
        public int Callorie { get; set; }
        public int Price { get; set; }
        public SaladListItemGroup Group { get; set; }

        public SaladListItem(SaladioContext context, ClassicSalad salad)
        {
            Name = salad.Name;
            Callorie = (int)salad.Callorie.Value;
            Price = salad.Price.Value;
            Ingredients = salad.Ingredients;
        }

        public SaladListItem(SaladioContext context, SavedSalad salad)
        {
            Name = salad.Name;
            Callorie = (int)salad.Callorie.Value;
            Price = salad.Price.Value;

            StringBuilder ingredientBuilder = new StringBuilder();
            bool first = true;

            using (context.Owner.OpenLoadingFromThread())
            {
                foreach (var item in salad.Ingredients)
                {
                    SaladComponent component = context.GetSaladComponentById(item.SaladComponentId.Value);

                    if (component != null)
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            ingredientBuilder.Append(" " + ",");
                        }

                        ingredientBuilder.Append(component.Name);
                    }
                }

                Ingredients = ingredientBuilder.ToString();
            }
        }
    }
}