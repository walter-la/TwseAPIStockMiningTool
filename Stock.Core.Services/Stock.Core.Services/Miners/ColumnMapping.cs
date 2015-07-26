using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq.Expressions;
using System.Globalization;
using Stock.Core.Services.Interfaces;

namespace Stock.Core.Services.Miners
{
    /// <summary>
    /// 欄位對應的表示類別。
    /// </summary>
    public class ColumnMapping : IColumnMapping
    {
        private static readonly RuntimeTypeHandle _byteTypeHandle = typeof(byte).TypeHandle;
        private static readonly RuntimeTypeHandle _intTypeHandle = typeof(int).TypeHandle;
        private static readonly RuntimeTypeHandle _floatTypeHandle = typeof(float).TypeHandle;
        private static readonly RuntimeTypeHandle _decimalTypeHandle = typeof(decimal).TypeHandle;
        private static readonly RuntimeTypeHandle _dateTimeTypeHandle = typeof(DateTime).TypeHandle;

        public PropertyInfo Property { get; private set; }
        public Func<string, object> Transformation { get; private set; }

        /// <summary>
        /// 建立實體屬性與轉換方法的物件。
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="property"></param>
        /// <param name="transformation"></param>
        /// <returns></returns>
        public static IColumnMapping Create<TEntity>(Expression<Func<TEntity, object>> property, Func<string, object> transformation = null)
        {
            return new ColumnMapping()
            {
                Property = GetPropertyInfo(property),
                Transformation = GetTransformation<TEntity>(property, transformation)
            };
        }
        /// <summary>
        /// 基礎資料型別轉換方法。
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="property"></param>
        /// <param name="transformation"></param>
        /// <returns></returns>
        private static Func<string, object> GetTransformation<TEntity>(Expression<Func<TEntity, object>> property, Func<string, object> transformation)
        {
            if (transformation != null)
                return transformation;

            var typeHandle = GetPropertyTypeHandle<TEntity>(property);
            if (typeHandle.Equals(_intTypeHandle))
            {
                return (s) =>
                {
                    int value;
                    int.TryParse(s, System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture, out value);
                    return value;
                };
            }
            else if (typeHandle.Equals(_floatTypeHandle))
            {
                return (s) =>
                {
                    decimal value;
                    decimal.TryParse(s, System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture, out value);
                    return (float)value;
                };
            }
            else if (typeHandle.Equals(_dateTimeTypeHandle))
            {
                return (s) =>
                {
                    DateTime value;
                    DateTime.TryParse(s, out value);
                    return value;
                };
            }
            else if (typeHandle.Equals(_byteTypeHandle))
            {
                return (s) =>
                {
                    byte value;
                    byte.TryParse(s, System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture, out value);
                    return value;
                };
            }
            else if (typeHandle.Equals(_decimalTypeHandle))
            {
                return (s) =>
                {
                    decimal value;
                    decimal.TryParse(s, System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture, out value);
                    return value;
                };
            }
            else
            {
                return s => s;
            }
        }
        /// <summary>
        /// 取得實體屬性。
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        private static PropertyInfo GetPropertyInfo<TEntity>(Expression<Func<TEntity, object>> property)
        {
            var propertyName = GetPropertyName<TEntity>(property);
            return typeof(TEntity).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        }
        /// <summary>
        /// 取得屬性名稱。
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        private static string GetPropertyName<TEntity>(Expression<Func<TEntity, object>> property)
        {
            return GetMemberExpression<TEntity>(property).Member.Name;
        }
        /// <summary>
        /// 取得屬性型別。
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        private static RuntimeTypeHandle GetPropertyTypeHandle<TEntity>(Expression<Func<TEntity, object>> property)
        {
            return ((PropertyInfo)GetMemberExpression<TEntity>(property).Member).PropertyType.TypeHandle;
        }
        /// <summary>
        /// 取得成員表示式。
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        private static MemberExpression GetMemberExpression<TEntity>(Expression<Func<TEntity, object>> property)
        {
            var exp = (LambdaExpression)property;

            //exp.Body has 2 possible types
            //If the property type is native, then exp.Body == typeof(MemberExpression)
            //If the property type is not native, then exp.Body == typeof(UnaryExpression) in which 
            //case we can get the MemberExpression from its Operand property
            var mExp = (exp.Body.NodeType == ExpressionType.MemberAccess) ?
                (MemberExpression)exp.Body :
                (MemberExpression)((UnaryExpression)exp.Body).Operand;
            return mExp;
        }
    }
}
