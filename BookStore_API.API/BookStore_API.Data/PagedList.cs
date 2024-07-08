﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore_API.Data
{
    public class PagedList<T>
    {
        public Meta Meta { get; private set; }
        public IList<T> Items { get; private set; }

        public PagedList() { }
        public PagedList(List<T> items, int totalCount, int pageNumber, int pageSize)
        {
            this.Meta = new Meta(pageNumber, pageSize, totalCount, (int)Math.Ceiling(totalCount / (double)pageSize));
            this.Items = items;
        }
        public static PagedList<T> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize, string fieldName, bool isAscending = true)
        {

            if (!string.IsNullOrEmpty(fieldName))
            {
                ParameterExpression parameter = Expression.Parameter(source.ElementType, "");

                MemberExpression property = Expression.Property(parameter, fieldName);
                LambdaExpression lambda = Expression.Lambda(property, parameter);

                string methodName = isAscending ? "OrderBy" : "OrderByDescending";

                Expression methodCallExpression = Expression.Call(typeof(Queryable), methodName,
                                      new Type[] { source.ElementType, property.Type },
                                      source.Expression, Expression.Quote(lambda));

                source = source.Provider.CreateQuery<T>(methodCallExpression);
            }

            var totalCount = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, totalCount, pageNumber, pageSize);
        }
    }
}
