using System.ComponentModel.DataAnnotations;
using System.Dynamic;

namespace MyCellarApiCore.Models
{
    public class BaseModel
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime CreateAt { get; set; } = DateTime.Now;

        [Required]
        public DateTime UpdateAt { get; set; } = DateTime.Now;

        [Required]
        public bool Deleted { get; set; }


        public virtual dynamic SelectFields(string fields)
        {
            var fieldsArray = fields.ToLower().Split(",");


            IDictionary<string, object?> expando = new Dictionary<string, object?>();
            var sourceType = this.GetType();

            Console.WriteLine(sourceType);

            foreach (var field in fieldsArray)
            {
                var propertyInfo = sourceType.GetProperty(field.First().ToString().ToUpper() + String.Join("", field.Skip(1)));
                if (propertyInfo != null)
                {
                    expando.Add(field, propertyInfo.GetValue(obj: this));
                }
            }
            

            return expando;
        }
    }
}
