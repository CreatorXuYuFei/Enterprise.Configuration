using Enterprise.Configuration.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Enterprise.Configuration.Utils
{
    public static class ModelValidator
    {
        /// <summary>实体属性校验</summary>
        public static void Validate(object model)
        {
            var validationContext = new ValidationContext(model);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(model, validationContext, results, true);

            if (!isValid)
            {
                var msg = string.Join("; ", results.Select(r => r.ErrorMessage));
                throw new ConfigValidateException($"配置实体校验失败：{msg}");
            }
        }
    }
}
