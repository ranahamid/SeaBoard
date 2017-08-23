using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NSBD.SharepointAutoMapper
{

    public static class SharepointMapperExtensions
    {

        public static string GetSharepointListName(this IEntitySharepointMapper value)
        {



            var attributes = value.GetType().GetCustomAttributes();
            if (attributes.Count() > 0)
            {
                foreach (Attribute attribute in attributes)
                {
                    if (attribute is SharepointListName)
                    {

                        SharepointListName SharepointAttribute = (SharepointListName)attribute;



                        return SharepointAttribute.GetName();
                        

                    }
                }
            }

            return value.GetType().Name;


        }

        public static int GetIdFromEntity(this IEntitySharepointMapper value)
        {



            PropertyInfo propertyInfo = value.GetType().GetProperty("Id");

            string retorno = propertyInfo.GetValue(value).ToString();

            return Convert.ToInt16(retorno);

        }

        public static void ProjectListItemFromEntity<T>(this ListItem value, T Entity) where T :  IEntitySharepointMapper
        {

            
            PropertyInfo[] propertyInfos;
            propertyInfos = typeof(T).GetProperties();
            value = BuildListItem<T>(propertyInfos, value, Entity);

        }
        public static T ProjectToEntity<T>(this ListItemCollection value) where T : new()
        {

            T Entidade = new T();
            PropertyInfo[] propertyInfos;
            propertyInfos = typeof(T).GetProperties();


            if (value.Count > 0)
                Entidade = BuildEntity<T>(propertyInfos, value[0]);


            return Entidade;



        }

        private static List<MapperDictionaryProperty> GetProperties(PropertyInfo[] propertyInfos)
        {
            List<MapperDictionaryProperty> properties = new List<MapperDictionaryProperty>();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                MapperDictionaryProperty property = new MapperDictionaryProperty();
                var attributes = propertyInfo.GetCustomAttributes();
                if (attributes.Count() > 0)
                {
                    foreach (Attribute attribute in attributes)
                    {
                        if (attribute is SharepointFieldName)
                        {

                            SharepointFieldName SharepointAttribute = (SharepointFieldName)attribute;
                            property.NameFieldEntity = propertyInfo.Name;
                            if (propertyInfo.PropertyType == typeof(LookupFieldMapper))
                            {
                                property.TypeFieldEntity = "LookupFieldMapper";
                            }
                            else if (propertyInfo.PropertyType == typeof(ChoiceField))
                            {
                                property.TypeFieldEntity = "ChoiceFieldMapper";
                            }

                            property.NameFieldSharepoint = SharepointAttribute.GetName();
                            properties.Add(property);

                        }
                        else if (!(attribute is IgnorePropertyInSharepoint)){
                        }
                    }
                }
                else
                {
                    property.NameFieldEntity = propertyInfo.Name;
                    property.NameFieldSharepoint = propertyInfo.Name;
                    properties.Add(property);
                }
            }
            return properties;
        }

        private static ListItem BuildListItem<T>(PropertyInfo[] propertyInfos, ListItem listItem, T Item) where T :  IEntitySharepointMapper
        {


            foreach (MapperDictionaryProperty property in GetProperties(propertyInfos))
            {

                try
                {

                    PropertyInfo propertyInfo = Item.GetType().GetProperty(property.NameFieldEntity);

                    if (property.TypeFieldEntity == "LookupFieldMapper")
                    {
                        //FieldLookupValue lookup = (FieldLookupValue)listItem[property.NameFieldSharepoint];
                        //LookupFieldMapper mapperLookup = new LookupFieldMapper()
                        //{
                        //    ID = lookup.LookupId,
                        //    Value = lookup.LookupValue
                        //};
                        if (propertyInfo.GetValue(Item) != null)
                            listItem[property.NameFieldSharepoint] = ((LookupFieldMapper)propertyInfo.GetValue(Item)).ID.Value.ToString() + ";#" + ((LookupFieldMapper)propertyInfo.GetValue(Item)).Value;
                        //lookup.LookupId = ((LookupFieldMapper)propertyInfo.GetValue(Item)).ID.Value;
                        //lookup.LookupValue = ((LookupFieldMapper)propertyInfo.GetValue(Item)).Value;
                    }
                    else
                    {
                        if (propertyInfo.GetValue(Item) != null)
                            listItem[property.NameFieldSharepoint] = propertyInfo.GetValue(Item);

                    }
                }
                catch { }

            }
            return listItem;
        }
        private static T BuildEntity<T>(PropertyInfo[] propertyInfos, ListItem listItem) where T : new()
        {
            T Item = new T();

            foreach (MapperDictionaryProperty property in GetProperties(propertyInfos))
            {

                try
                {

                    PropertyInfo propertyInfo = Item.GetType().GetProperty(property.NameFieldEntity);

                    if (property.TypeFieldEntity == "LookupFieldMapper")
                    {
                        FieldLookupValue lookup = (FieldLookupValue)listItem[property.NameFieldSharepoint];
                        LookupFieldMapper mapperLookup = new LookupFieldMapper()
                        {
                            ID = lookup.LookupId,
                            Value = lookup.LookupValue
                        };
                        propertyInfo.SetValue(Item, mapperLookup, null);
                    }
                    else if (property.TypeFieldEntity == "ChoiceFieldMapper")
                    {
                        List<ChoiceFieldMapper> choices = new List<ChoiceFieldMapper>();
                        FieldChoice allchoices = (FieldChoice)listItem[property.NameFieldSharepoint];
                        foreach (string choice in allchoices.Choices)
                        {
                            if (choice != "Draft"){
                                choices.Add(new ChoiceFieldMapper { Value = choice, Text = choice });
                            }
                        }
                        propertyInfo.SetValue(choices, listItem[property.NameFieldSharepoint], null);
                    }
                    else
                    {
                        propertyInfo.SetValue(Item, listItem[property.NameFieldSharepoint], null);
                    }
                }
                catch { }

            }
            return Item;
        }

    }
}
