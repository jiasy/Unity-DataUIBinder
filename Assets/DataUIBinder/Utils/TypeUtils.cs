using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

namespace DataUIBinder {
    public class TypeUtils {
        public static object getObjectByNameSpaceAndClassName(string nameSpace_,string fullClassName_, object[] parameters_ = null) {
            return getObjectByClassName(nameSpace_ + "." + fullClassName_,parameters_);
        }
        public static object getObjectByClassName(string fullClassName_, object[] parameters_ = null) {
            object _obj;
            Type _type = Type.GetType(fullClassName_);
            if(parameters_ != null) {
                //_obj = System.Activator.CreateInstance(_type,parameters_);//也可以创建对象
                _obj = _type.Assembly.CreateInstance(fullClassName_, true, System.Reflection.BindingFlags.Default, null, parameters_, null, null);
            } else {
                _obj = _type.Assembly.CreateInstance(fullClassName_);
            }
            return _obj;
        }
    }
    /*
    高效的相同属性的类对象，复制对象
        public class Student{
            public int Id { get; set; }
            public string Name { get; set; } 
            public int Age { get; set; } 
        }
        public class StudentSecond{
            public int Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; } 
        }
    */
    //StudentSecond ss= TransExpV2<Student, StudentSecond>.Trans(s);
    public static class TransExpV2<TIn, TOut> {

        private static readonly Func<TIn, TOut> cache = GetFunc();
        private static Func<TIn, TOut> GetFunc() {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
            List<MemberBinding> memberBindingList = new List<MemberBinding>();

            foreach(var item in typeof(TOut).GetProperties()) {
                if(!item.CanWrite) continue;

                MemberExpression property = Expression.Property(parameterExpression, typeof(TIn).GetProperty(item.Name));
                MemberBinding memberBinding = Expression.Bind(item, property);
                memberBindingList.Add(memberBinding);
            }

            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
            Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression });

            return lambda.Compile();
        }

        public static TOut Trans(TIn tIn) {
            return cache(tIn);
        }

    }

}