using Microsoft.SharePoint.Client;
using NSBD.SharepointAutoMapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NSBD.SharepointAutoMapper
{    
    public static class SharepointMapper
    {
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
                            else if (propertyInfo.PropertyType == typeof(ChoiceFieldMapper))
                            {
                                property.TypeFieldEntity = "ChoiceFieldMapper";
                            }

                            property.NameFieldSharepoint = SharepointAttribute.GetName();
                            properties.Add(property);

                        }
                        else if (!(attribute is IgnorePropertyInSharepoint))
                        {


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

        public static List<T> ProjectToListEntity<T>(this ListItemCollection value) where T : new()
        {
            List<T> ListEntidade = new List<T>();
            PropertyInfo[] propertyInfos;
            propertyInfos = typeof(T).GetProperties();
            foreach (ListItem listItem in value)
            {
                ListEntidade.Add(BuildEntity<T>(propertyInfos, listItem));
            }

            return ListEntidade;
        }
        public static T ProjectToEntity<T>(this ListItem listItem) where T : new()
        {
            PropertyInfo[] propertyInfos;
            propertyInfos = typeof(T).GetProperties();
            return BuildEntity<T>(propertyInfos, listItem); 
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

                        if (lookup != null)
                        {
                            LookupFieldMapper mapperLookup = new LookupFieldMapper()
                            {
                                ID = lookup.LookupId,
                                Value = lookup.LookupValue
                            };
                            propertyInfo.SetValue(Item, mapperLookup, null);
                        }
                    }
                    else if (property.TypeFieldEntity == "ChoiceFieldMapper")
                    {
                        ChoiceFieldMapper choice = new ChoiceFieldMapper();
                        FieldChoice allchoices = (FieldChoice)listItem[property.NameFieldSharepoint];
                        foreach (string c in allchoices.Choices)
                        {
                            if (c != "Draft"){
                                choice = new ChoiceFieldMapper { Value = c, Text = c };
                            }
                            break;
                        }
                        propertyInfo.SetValue(choice, listItem[property.NameFieldSharepoint], null);
                    }
                    else
                    { 
                        propertyInfo.SetValue(Item, listItem[property.NameFieldSharepoint], null);
                    }
                }
                catch (Exception ex){
                
                }

            }
            return Item;
        }
        private static ConfigurationStore _configuration = new ConfigurationStore();

        public static IConfiguration Configuration
        {
            get { return (IConfiguration)_configuration; }
        }     
        public static IMappingExpression CreateMap<T>(String ListName) where T : class
        {
            return Configuration.CreateMap<T>(ListName);
        }        
    }
}
