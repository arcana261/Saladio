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
using System.Globalization;
using IO.Swagger.Api;
using Saladio.Utility;
using Saladio.Activities;
using IO.Swagger.Model;
using Saladio.Config;

namespace Saladio.Contexts
{
    public class SaladioContext : IDisposable
    {
        private SharedActivity mOwner;
        private static IList<SaladComponentGroup> mGroups;
        private static IList<ClassicSaladCatagory> mClassicSaladCatagories;
        private static IDictionary<int, IList<ClassicSalad>> mClassicSalads;
        private static IDictionary<int, SaladComponent> mSaladComponentById;
        private static IList<SavedSalad> mSavedSalads;
        private static IList<DeliverySchedule> mDeliveryHours;
        private static IList<string> mDeliveryAddresses;
        private static IDictionary<int, IDictionary<int, IList<Order>>> mOrderSchedules; //[year][month]
        private static IDictionary<string, BodyNeed> mUserBodyNeeds;

        public SaladioContext()
        {
        }

        public SaladioContext(SharedActivity owner)
        {
            mOwner = owner;
        }

        public SharedActivity Owner
        {
            get
            {
                return mOwner;
            }
        }

        public IList<SaladBodyNeedGroup> GetCustomSaladBodyNeedComparison(IList<PickedSaladComponent> components)
        {
            try
            {
                using (mOwner.OpenLoadingFromThread())
                {
                    SaladBodyNeedGroup food = new SaladBodyNeedGroup()
                    {
                        Name = "food",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_group_food)
                    };

                    SaladBodyNeedGroup irons = new SaladBodyNeedGroup()
                    {
                        Name = "irons",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_group_irons)
                    };

                    SaladBodyNeedGroup vitamins = new SaladBodyNeedGroup()
                    {
                        Name = "vitamins",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_group_vitamins)
                    };

                    IDictionary<string, BodyNeed> saladBodyNeeds = GetCustomSaladBodyNeeds(components);
                    IDictionary<string, BodyNeed> userNeeds = GetUserBodyNeeds();

                    var foodItems = (new string[] { "energy", "protein", "carbohydrat", "fibre", "fat", "moisture", "sugar" });
                    var ironItems = (new string[] { "calcium", "iron", "magnesium", "phosphorus", "zinc", "copper", "manganese" });
                    var vitaminItems = (new string[] { "vitamin_a", "vitamin_b1", "vitamin_b2", "vitamin_b3", "vitamin_b6", "vitamin_b12", "vitamin_c", "vitamin_d", "vitamin_e", "vitamin_k" });

                    var q = (new SaladBodyNeedGroup[] { food, irons, vitamins }).Zip(new string[][] { foodItems, ironItems, vitaminItems }, (group, items) => new
                    {
                        group = group,
                        items = items
                    });

                    IList<SaladBodyNeedGroup> result = new List<SaladBodyNeedGroup>();

                    foreach (var item in q)
                    {
                        foreach (var x in item.items)
                        {
                            if (saladBodyNeeds.ContainsKey(x) || userNeeds.ContainsKey(x))
                            {
                                if (!saladBodyNeeds.ContainsKey(x))
                                {
                                    item.group.Items.Add(new SaladBodyNeedItem()
                                    {
                                        Name = userNeeds[x].Name,
                                        FriendlyName = userNeeds[x].FriendlyName,
                                        Required = userNeeds[x].Value,
                                        Provided = null,
                                        Unit = userNeeds[x].Unit
                                    });
                                }
                                else if (!userNeeds.ContainsKey(x))
                                {
                                    item.group.Items.Add(new SaladBodyNeedItem()
                                    {
                                        Name = saladBodyNeeds[x].Name,
                                        FriendlyName = saladBodyNeeds[x].FriendlyName,
                                        Required = null,
                                        Provided = saladBodyNeeds[x].Value,
                                        Unit = saladBodyNeeds[x].Unit
                                    });
                                }
                                else
                                {
                                    item.group.Items.Add(new SaladBodyNeedItem()
                                    {
                                        Name = userNeeds[x].Name,
                                        FriendlyName = userNeeds[x].FriendlyName,
                                        Required = userNeeds[x].Value,
                                        Provided = saladBodyNeeds[x].Value,
                                        Unit = saladBodyNeeds[x].Unit
                                    });
                                }
                            }
                        }
                    }

                    result.Add(food);
                    result.Add(irons);
                    result.Add(vitamins);

                    return result;
                }
            }
            catch(Exception e)
            {
                mOwner.ShowMessageDialogForExceptionFromThread(e);
                return new List<SaladBodyNeedGroup>();
            }
        }

        public IDictionary<string, BodyNeed> GetCustomSaladBodyNeeds(IList<PickedSaladComponent> components)
        {
            try
            {
                using (mOwner.OpenLoadingFromThread())
                {
                    IDictionary<string, BodyNeed> result = new Dictionary<string, BodyNeed>();

                    string unitMicroGram = mOwner.Resources.GetString(Resource.String.body_need_unit_micro_gram);
                    string unitMiliGram = mOwner.Resources.GetString(Resource.String.body_need_unit_mili_gram);
                    string unitGram = mOwner.Resources.GetString(Resource.String.body_need_unit_gram);
                    string unitIu = mOwner.Resources.GetString(Resource.String.body_need_unit_iu);
                    string unitEnergy = mOwner.Resources.GetString(Resource.String.body_need_unit_energy);

                    result["protein"] = new BodyNeed()
                    {
                        Name = "protein",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_protein),
                        Value = 0,
                        Unit = unitGram
                    };

                    result["fat"] = new BodyNeed()
                    {
                        Name = "fat",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_fat),
                        Value = 0,
                        Unit = unitGram
                    };

                    result["carbohydrat"] = new BodyNeed()
                    {
                        Name = "carbohydrat",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_carbohydrat),
                        Value = 0,
                        Unit = unitGram
                    };

                    result["energy"] = new BodyNeed()
                    {
                        Name = "energy",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_callorie),
                        Value = 0,
                        Unit = unitEnergy
                    };

                    result["moisture"] = new BodyNeed()
                    {
                        Name = "moisture",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_water),
                        Value = 0,
                        Unit = unitGram
                    };

                    result["sugar"] = new BodyNeed()
                    {
                        Name = "sugar",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_sugar),
                        Value = 0,
                        Unit = unitGram
                    };

                    result["fibre"] = new BodyNeed()
                    {
                        Name = "fibre",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_fiber),
                        Value = 0,
                        Unit = unitGram
                    };

                    result["calcium"] = new BodyNeed()
                    {
                        Name = "calcium",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_calcium),
                        Value = 0,
                        Unit = unitMiliGram
                    };

                    result["iron"] = new BodyNeed()
                    {
                        Name = "iron",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_iron),
                        Value = 0,
                        Unit = unitMiliGram
                    };

                    result["magnesium"] = new BodyNeed()
                    {
                        Name = "magnesium",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_magnesium),
                        Value = 0,
                        Unit = unitMiliGram
                    };

                    result["phosphorus"] = new BodyNeed()
                    {
                        Name = "phosphorus",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_phosphorus),
                        Value = 0,
                        Unit = unitMiliGram
                    };

                    result["zinc"] = new BodyNeed()
                    {
                        Name = "zinc",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_zinc),
                        Value = 0,
                        Unit = unitMiliGram
                    };

                    result["copper"] = new BodyNeed()
                    {
                        Name = "copper",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_copper),
                        Value = 0,
                        Unit = unitMiliGram
                    };

                    result["manganese"] = new BodyNeed()
                    {
                        Name = "manganese",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_manganese),
                        Value = 0,
                        Unit = unitMiliGram
                    };

                    result["vitamin_e"] = new BodyNeed()
                    {
                        Name = "vitamin_e",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_vitamin_e),
                        Value = 0,
                        Unit = unitMiliGram
                    };

                    result["vitamin_d"] = new BodyNeed()
                    {
                        Name = "vitamin_d",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_vitamin_d),
                        Value = 0,
                        Unit = unitIu
                    };

                    result["vitamin_c"] = new BodyNeed()
                    {
                        Name = "vitamin_c",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_vitamin_c),
                        Value = 0,
                        Unit = unitMiliGram
                    };

                    result["vitamin_b1"] = new BodyNeed()
                    {
                        Name = "vitamin_b1",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_vitamin_b1),
                        Value = 0,
                        Unit = unitMiliGram
                    };

                    result["vitamin_b2"] = new BodyNeed()
                    {
                        Name = "vitamin_b2",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_vitamin_b2),
                        Value = 0,
                        Unit = unitMiliGram
                    };

                    result["vitamin_b3"] = new BodyNeed()
                    {
                        Name = "vitamin_b3",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_vitamin_b3),
                        Value = 0,
                        Unit = unitMiliGram
                    };

                    result["vitamin_b6"] = new BodyNeed()
                    {
                        Name = "vitamin_b6",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_vitamin_b6),
                        Value = 0,
                        Unit = unitMiliGram
                    };

                    result["vitamin_b12"] = new BodyNeed()
                    {
                        Name = "vitamin_b12",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_vitamin_b12),
                        Value = 0,
                        Unit = unitMicroGram
                    };

                    result["vitamin_k"] = new BodyNeed()
                    {
                        Name = "vitamin_k",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_vitamin_k),
                        Value = 0,
                        Unit = unitMicroGram
                    };

                    result["vitamin_a"] = new BodyNeed()
                    {
                        Name = "vitamin_a",
                        FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_vitamin_a),
                        Value = 0,
                        Unit = unitMicroGram
                    };

                    foreach (var item in components)
                    {
                        SaladComponent salad = GetSaladComponentById(item.SaladComponentId.Value);

                        result["protein"].Value += salad.Protein.Value * item.Quantity.Value;
                        result["fat"].Value += salad.Fat.Value * item.Quantity.Value;
                        result["carbohydrat"].Value += salad.Carbohydrat.Value * item.Quantity.Value;
                        result["energy"].Value += salad.Energy.Value * item.Quantity.Value;
                        result["moisture"].Value += salad.Moisture.Value * item.Quantity.Value;
                        result["sugar"].Value += salad.Sugar.Value * item.Quantity.Value;
                        result["fibre"].Value += salad.Fibre.Value * item.Quantity.Value;
                        result["calcium"].Value += salad.Calcium.Value * item.Quantity.Value;
                        result["iron"].Value += salad.Iron.Value * item.Quantity.Value;
                        result["magnesium"].Value += salad.Magnesium.Value * item.Quantity.Value;
                        result["phosphorus"].Value += salad.Phosphorus.Value * item.Quantity.Value;
                        result["zinc"].Value += salad.Zinc.Value * item.Quantity.Value;
                        result["copper"].Value += salad.Copper.Value * item.Quantity.Value;
                        result["manganese"].Value += salad.Manganese.Value * item.Quantity.Value;
                        result["vitamin_e"].Value += salad.VitaminE.Value * item.Quantity.Value;
                        result["vitamin_d"].Value += salad.VitaminD.Value * item.Quantity.Value;
                        result["vitamin_c"].Value += salad.VitaminC.Value * item.Quantity.Value;
                        result["vitamin_b1"].Value += salad.Thiamin.Value * item.Quantity.Value;
                        result["vitamin_b2"].Value += salad.VitaminB2.Value * item.Quantity.Value;
                        result["vitamin_b3"].Value += salad.VitaminB3.Value * item.Quantity.Value;
                        result["vitamin_b6"].Value += salad.VitaminB6.Value * item.Quantity.Value;
                        result["vitamin_b12"].Value += salad.VitaminB12.Value * item.Quantity.Value;
                        result["vitamin_k"].Value += salad.VitaminK.Value * item.Quantity.Value;
                        result["vitamin_a"].Value += salad.VitaminA.Value * item.Quantity.Value;
                    }

                    return result;
                }
            }
            catch(Exception e)
            {
                mOwner.ShowMessageDialogForExceptionFromThread(e);
                return new Dictionary<string, BodyNeed>();
            }
        }

        public IDictionary<string, BodyNeed> GetUserBodyNeeds()
        {
            if (mUserBodyNeeds == null)
            {
                try
                {
                    using (mOwner.OpenLoadingFromThread())
                    {
                        UsersApi api = new UsersApi(SharedConfig.AuthorizedApiConfig);
                        Dictionary<string, BodyNeed> items = new Dictionary<string, BodyNeed>();

                        var res = api.GetCurrentUserNeeds();
                        
                        string unitMicroGram = mOwner.Resources.GetString(Resource.String.body_need_unit_micro_gram);
                        string unitMiliGram = mOwner.Resources.GetString(Resource.String.body_need_unit_mili_gram);
                        string unitGram = mOwner.Resources.GetString(Resource.String.body_need_unit_gram);
                        string unitIu = mOwner.Resources.GetString(Resource.String.body_need_unit_iu);
                        string unitEnergy = mOwner.Resources.GetString(Resource.String.body_need_unit_energy);

                        if (res.Energy.HasValue)
                        {
                            items["energy"] = new BodyNeed()
                            {
                                Name = "energy",
                                FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_callorie),
                                Value = res.Energy.Value,
                                Unit = unitEnergy
                            };
                        }

                        if (res.VitaminA.HasValue)
                        {
                            items["vitamin_a"] = new BodyNeed()
                            {
                                Name = "vitamin_a",
                                FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_vitamin_a),
                                Value = res.VitaminA.Value,
                                Unit = unitMicroGram
                            };
                        }

                        if (res.VitaminC.HasValue)
                        {
                            items["vitamin_c"] = new BodyNeed()
                            {
                                Name = "vitamin_c",
                                FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_vitamin_c),
                                Value = res.VitaminC.Value,
                                Unit = unitMiliGram
                            };
                        }

                        if (res.VitaminD.HasValue)
                        {
                            items["vitamin_d"] = new BodyNeed()
                            {
                                Name = "vitamin_d",
                                FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_vitamin_d),
                                Value = res.VitaminD.Value,
                                Unit = unitIu
                            };
                        }

                        if (res.VitaminE.HasValue)
                        {
                            items["vitamin_e"] = new BodyNeed()
                            {
                                Name = "vitamin_e",
                                FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_vitamin_e),
                                Value = res.VitaminE.Value,
                                Unit = unitMiliGram
                            };
                        }

                        if (res.VitaminK.HasValue)
                        {
                            items["vitamin_k"] = new BodyNeed()
                            {
                                Name = "vitamin_k",
                                FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_vitamin_k),
                                Value = res.VitaminK.Value,
                                Unit = unitMicroGram
                            };
                        }

                        if (res.VitaminB1.HasValue)
                        {
                            items["vitamin_b1"] = new BodyNeed()
                            {
                                Name = "vitamin_b1",
                                FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_vitamin_b1),
                                Value = res.VitaminB1.Value,
                                Unit = unitMiliGram
                            };
                        }

                        if (res.VitaminB2.HasValue)
                        {
                            items["vitamin_b2"] = new BodyNeed()
                            {
                                Name = "vitamin_b2",
                                FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_vitamin_b2),
                                Value = res.VitaminB2.Value,
                                Unit = unitMiliGram
                            };
                        }

                        if (res.VitaminB3.HasValue)
                        {
                            items["vitmain_b3"] = new BodyNeed()
                            {
                                Name = "vitamin_b3",
                                FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_vitamin_b3),
                                Value = res.VitaminB3.Value,
                                Unit = unitMiliGram
                            };
                        }

                        if (res.VitaminB6.HasValue)
                        {
                            items["vitamin_b6"] = new BodyNeed()
                            {
                                Name = "vitamin_b6",
                                FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_vitamin_b6),
                                Value = res.VitaminB6.Value,
                                Unit = unitMiliGram
                            };
                        }

                        if (res.VitaminB12.HasValue)
                        {
                            items["vitamin_b12"] = new BodyNeed()
                            {
                                Name = "vitamin_b12",
                                FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_vitamin_b12),
                                Value = res.VitaminB12.Value,
                                Unit = unitMicroGram
                            };
                        }

                        if (res.Calcium.HasValue)
                        {
                            items["calcium"] = new BodyNeed()
                            {
                                Name = "calcium",
                                FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_calcium),
                                Value = res.Calcium.Value,
                                Unit = unitMiliGram
                            };
                        }

                        if (res.Copper.HasValue)
                        {
                            items["copper"] = new BodyNeed()
                            {
                                Name = "copper",
                                FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_copper),
                                Value = res.Copper.Value,
                                Unit = unitMiliGram
                            };
                        }

                        if (res.Iron.HasValue)
                        {
                            items["iron"] = new BodyNeed()
                            {
                                Name = "iron",
                                FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_iron),
                                Value = res.Iron.Value,
                                Unit = unitMiliGram
                            };
                        }

                        if (res.Magnesium.HasValue)
                        {
                            items["magnesium"] = new BodyNeed()
                            {
                                Name = "magnesium",
                                FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_magnesium),
                                Value = res.Magnesium.Value,
                                Unit = unitMiliGram
                            };
                        }

                        if (res.Phosphorus.HasValue)
                        {
                            items["phosphorus"] = new BodyNeed()
                            {
                                Name = "phosphorus",
                                FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_phosphorus),
                                Value = res.Phosphorus.Value,
                                Unit = unitMiliGram
                            };
                        }

                        if (res.Zinc.HasValue)
                        {
                            items["zinc"] = new BodyNeed()
                            {
                                Name = "zinc",
                                FriendlyName = mOwner.Resources.GetString(Resource.String.body_need_zinc),
                                Value = res.Zinc.Value,
                                Unit = unitMiliGram
                            };
                        }

                        mUserBodyNeeds = items;
                    }
                }
                catch (Exception e)
                {
                    mOwner.ShowMessageDialogForExceptionFromThread(e);
                    return new Dictionary<string, BodyNeed>();
                }
            }

            return mUserBodyNeeds;
        }

        public IList<ClassicSalad> GetClassicSaladsByCatagory(ClassicSaladCatagory catagory)
        {
            return GetClassicSaladsByCatagory(catagory.Id.Value);
        }

        public IList<ClassicSalad> GetClassicSaladsByCatagory(int catagoryId)
        {
            if (mClassicSalads == null)
            {
                mClassicSalads = new Dictionary<int, IList<ClassicSalad>>();
            }

            if (!mClassicSalads.ContainsKey(catagoryId))
            {
                try
                {
                    using (mOwner.OpenLoadingFromThread())
                    {
                        ClassicSaladsApi api = new ClassicSaladsApi(SharedConfig.AuthorizedApiConfig);
                        IList<ClassicSalad> salads = new List<ClassicSalad>();

                        PagedApiHelper.FetchAll((start, length) =>
                        {
                            var res = api.GetClassicSaladsByCatagory(catagoryId, length, start);

                            foreach (var item in res.Data)
                            {
                                salads.Add(item);
                            }

                            return res.RecordsFiltered.Value;
                        });

                        mClassicSalads[catagoryId] = salads;
                    }
                }
                catch(Exception e)
                {
                    mOwner.ShowMessageDialogForExceptionFromThread(e);
                    return new List<ClassicSalad>();
                }
            }

            return mClassicSalads[catagoryId];
        }

        public IList<ClassicSaladCatagory> ClassicSaladCatagories
        {
            get
            {
                if (mClassicSaladCatagories == null)
                {
                    try
                    {
                        using (mOwner.OpenLoadingFromThread())
                        {
                            ClassicSaladCatagoriesApi catagoryApi = new ClassicSaladCatagoriesApi(SharedConfig.AuthorizedApiConfig);
                            List<ClassicSaladCatagory> items = new List<ClassicSaladCatagory>();

                            PagedApiHelper.FetchAll((start, length) =>
                            {
                                var res = catagoryApi.GetClassicSaladCatagories(length, start);

                                foreach (var item in res.Data)
                                {
                                    items.Add(item);
                                }

                                return res.RecordsFiltered.Value;
                            });

                            mClassicSaladCatagories = items;
                        }
                    }
                    catch(Exception e)
                    {
                        mOwner.ShowMessageDialogForExceptionFromThread(e);
                        return new List<ClassicSaladCatagory>();
                    }
                }

                return mClassicSaladCatagories;
            }
        }

        public SaladComponent GetSaladComponentById(int saladComponentId)
        {
            if (mSaladComponentById == null)
            {
                mSaladComponentById = new Dictionary<int, SaladComponent>();
            }

            if (!mSaladComponentById.ContainsKey(saladComponentId))
            {
                try
                {
                    SaladComponentsApi api = new SaladComponentsApi(SharedConfig.AuthorizedApiConfig);
                    var res = api.GetSaladComponentById(saladComponentId);

                    mSaladComponentById[saladComponentId] = res;
                    return res;
                }
                catch(Exception e)
                {
                    mOwner.ShowMessageDialogForExceptionFromThread(e);
                    return null;
                }
            }

            return mSaladComponentById[saladComponentId];
        }

        public IList<SaladComponentGroup> SaladComponentGroups
        {
            get
            {
                if (mGroups == null)
                {
                    try
                    {
                        using (var handle = mOwner.OpenLoadingFromThread())
                        {
                            SaladComponetGroupsApi api = new SaladComponetGroupsApi(SharedConfig.AuthorizedApiConfig);
                            List<SaladComponentGroup> groups = new List<SaladComponentGroup>();
                            mSaladComponentById = new Dictionary<int, SaladComponent>();

                            PagedApiHelper.FetchAll((start, length) =>
                            {
                                var res = api.GetSaladComponentGroup(length, start);
                                foreach (var item in res.Data)
                                {
                                    groups.Add(item);

                                    foreach (var component in item.Items)
                                    {
                                        mSaladComponentById[component.Id.Value] = component;
                                    }
                                }

                                return res.RecordsFiltered.Value;
                            });

                            mGroups = groups;
                        }
                    }
                    catch(Exception e)
                    {
                        mOwner.ShowMessageDialogForExceptionFromThread(e);
                        return new List<SaladComponentGroup>();
                    }
                }

                return mGroups;
            }
        }

        public void ClearSavedSaladsCache()
        {
            mSavedSalads = null;
        }

        public IList<SavedSalad> SavedSalads
        {
            get
            {
                if (mSavedSalads == null)
                {
                    try
                    {
                        using (mOwner.OpenLoadingFromThread())
                        {
                            SavedSaladsApi api = new SavedSaladsApi(SharedConfig.AuthorizedApiConfig);
                            List<SavedSalad> items = new List<SavedSalad>();

                            PagedApiHelper.FetchAll((start, length) =>
                            {
                                var res = api.GetSavedSaladsByMe(length, start);

                                foreach (var item in res.Data)
                                {
                                    items.Add(item);
                                }

                                return res.RecordsFiltered.Value;
                            });

                            mSavedSalads = items;
                        }
                    }
                    catch(Exception e)
                    {
                        mOwner.ShowMessageDialogForExceptionFromThread(e);
                        return new List<SavedSalad>();
                    }
                }

                return mSavedSalads;
            }
        }

        public IList<DeliverySchedule> DeliveryHours
        {
            get
            {
                if (mDeliveryHours == null)
                {
                    try
                    {
                        using (var handle = mOwner.OpenLoadingFromThread())
                        {
                            DeliverySchedulesApi api = new DeliverySchedulesApi(SharedConfig.AuthorizedApiConfig);
                            List<DeliverySchedule> items = new List<DeliverySchedule>();

                            PagedApiHelper.FetchAll((start, length) =>
                            {
                                var res = api.GetDeliverySchedules(length, null, start);
                                foreach (var item in res.Data)
                                {
                                    items.Add(item);
                                }

                                return res.RecordsFiltered.Value;
                            });

                            mDeliveryHours = items;
                        }
                    }
                    catch(Exception e)
                    {
                        mOwner.ShowMessageDialogForExceptionFromThread(e);
                        return new List<DeliverySchedule>();
                    }
                }

                return mDeliveryHours;
            }
        }

        public IList<string> DeliveryAddresses
        {
            get
            {
                if (mDeliveryAddresses == null)
                {
                    try
                    {
                        using (mOwner.OpenLoadingFromThread())
                        {
                            UsersApi api = new UsersApi(SharedConfig.AuthorizedApiConfig);
                            var res = api.GetCurrentUser();
                            mDeliveryAddresses = res.Addresses;
                        }
                    }
                    catch(Exception e)
                    {
                        mOwner.ShowMessageDialogForExceptionFromThread(e);
                        return new List<string>();
                    }
                }

                return mDeliveryAddresses;
            }
        }

        public IList<GroupedOrderSchedule> GetGroupedOrderSchedules(int year, int month)
        {
            IList<Order> orders = GetOrderSchedules(year, month);
            ILookup<int, DeliverySchedule> deliverySchedules = DeliveryHours.ToLookup(x => x.Id.Value);

            PersianCalendar persianCalendar = new PersianCalendar();
            int daysOfMonth = persianCalendar.GetDaysInMonth(year, month);

            List<GroupedOrderSchedule> result = new List<GroupedOrderSchedule>();

            for (int i = 1; i <= daysOfMonth; i++)
            {
                result.Add(new GroupedOrderSchedule()
                {
                    LaunchOrders = new List<Order>(),
                    DinnerOrders = new List<Order>(),
                    Year = year,
                    Month = month,
                    Day = i
                });
            }

            foreach (var item in orders)
            {
                DeliverySchedule schedule = deliverySchedules[item.DeliveryScheduleId.Value].FirstOrDefault();

                if (schedule.Catagory == DeliverySchedule.CatagoryEnum.Launch)
                {
                    result[item.DeliveryDate.Day.Value - 1].LaunchOrders.Add(item);
                }
                else
                {
                    result[item.DeliveryDate.Day.Value - 1].DinnerOrders.Add(item);
                }
            }

            return result;
        }

        public IList<Order> GetOrderSchedules(int year, int month)
        {
            if (mOrderSchedules == null)
            {
                mOrderSchedules = new Dictionary<int, IDictionary<int, IList<Order>>>();
            }

            if (!mOrderSchedules.ContainsKey(year))
            {
                mOrderSchedules[year] = new Dictionary<int, IList<Order>>();
            }

            IDictionary<int, IList<Order>> yearSchedules = mOrderSchedules[year];

            if (!yearSchedules.ContainsKey(month))
            {
                try
                {
                    using (mOwner.OpenLoadingFromThread())
                    {
                        OrdersApi api = new OrdersApi(SharedConfig.AuthorizedApiConfig);
                        IList<Order> items = new List<Order>();

                        PagedApiHelper.FetchAll((start, length) =>
                        {
                            var res = api.GetOrdersByMe(length, start, year, month, 1, year, month, 31);

                            foreach (var item in res.Data)
                            {
                                items.Add(item);
                            }

                            return res.RecordsFiltered.Value;
                        });

                        yearSchedules[month] = items;
                    }
                }
                catch(Exception e)
                {
                    mOwner.ShowMessageDialogForExceptionFromThread(e);
                    return new List<Order>();
                }
            }

            return yearSchedules[month];
        }

        public void ClearOrderScheduleCache(int year, int month)
        {
            if (mOrderSchedules != null && mOrderSchedules.ContainsKey(year))
            {
                mOrderSchedules[year].Remove(month);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SaladioContext() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}