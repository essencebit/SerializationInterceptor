using Newtonsoft.Json;
using SerializationInterceptor;
using SerializationInterceptor.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class Program
    {
        public class Test
        {
            [JsonProperty("_prop")]
            public Abstract<Abstract<int>> Prop { get; set; }

            [JsonProperty("_dynamic")]
            public dynamic Dynamic { get; set; }
        }

        public abstract class Abstract<T>
        {
            [JsonPropertyInterceptor("_number")]
            [JsonProperty("_prop")]
            public int Number { get; set; }

            [JsonProperty("_char")]
            public char Char { get; set; }

            [JsonProperty("_x")]
            public T X { get; set; }
        }

        public class Concrete<T> : Abstract<T>
        {
            [JsonProperty("_string")]
            public string String { get; set; }
        }


        public static void Main(string[] args)
        {
            var obj = new Test { Dynamic = new object(), Prop = new Concrete<Abstract<int>> { Number = 13, String = "str", Char = 'c', X = new Concrete<int> { String = "str2", Char = 'd', Number = 14, X=7 } } };
            var @string = NewtonsoftJsonSerializationInterceptor.Serialize(obj);
            var abstractConcreteMap = new AbstractConcreteMap { { typeof(Abstract<Abstract<int>>), typeof(Concrete<Abstract<int>>) },{ typeof(Abstract<int>), typeof(Concrete<int>)} };
            var x = NewtonsoftJsonSerializationInterceptor.Deserialize<Test>(@string, abstractConcreteMap);
        }

        private static object GetObj()
        {
            var self = new X<X<int, float, char>, double, string>();
            self.SelfProp = self;
            self.FirstProp = new X<int, float, char>();
            self.FirstProp.SelfProp = self.FirstProp;
            var obj = new Z
            {
                EnumProp = E.A,
                StructProp = new S
                {
                    EnumerableOfStructSProp = new List<S>
                    {
                        default,
                        new S
                        {
                            EnumerableOfStructSProp = new List<S>(),
                            GenericClassCOfStructSProp = new C<S>()
                        }
                    },
                    GenericClassCOfStructSProp = new C<S>
                    {
                        ArrayOfStructSProp = new S[]
                        {
                            new S
                            {
                                GenericClassCOfStructSProp = new C<S>
                                {
                                    ArrayOfStructSProp = Array.Empty<S>(),
                                    BombProp = Array.Empty<IEnumerable<C<S>[][,,]>>()
                                }
                            },
                            default
                        },
                        BombProp = new ISet<C<S>[][,,]>[]
                        {
                            new HashSet<C<S>[][,,]>
                            {
                                new C<S>[][,,]
                                {
                                    new C<S>[,,]
                                    {
                                        {
                                            {
                                                new C<S>
                                                {
                                                    ArrayOfStructSProp = new S[]
                                                    {
                                                        new S
                                                        {
                                                            GenericClassCOfStructSProp = new C<S>
                                                            {
                                                                ArrayOfStructSProp = Array.Empty<S>(),
                                                                BombProp = Array.Empty<IEnumerable<C<S>[][,,]>>()
                                                            }
                                                        }
                                                    },
                                                    BombProp = new IEnumerable<C<S>[][,,]>[]
                                                    {
                                                        new List<C<S>[][,,]>
                                                        {
                                                            new C<S>[][,,]
                                                            {
                                                                new C<S>[,,]
                                                                {
                                                                    {
                                                                        {
                                                                            new C<S>{},
                                                                            new C<S>{}
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        },
                                                        Enumerable.Empty<C<S>[][,,]>()
                                                    },
                                                    GenericClassXProp = self,
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            null,
                            new HashSet<C<S>[][,,]>
                            {
                                new C<S>[][,,]
                                {
                                    null,
                                    new C<S>[,,]
                                    {
                                        {
                                            {
                                                new C<S>
                                                {
                                                    ArrayOfStructSProp = new S[]
                                                    {
                                                        new S
                                                        {
                                                            GenericClassCOfStructSProp = new C<S>
                                                            {
                                                                ArrayOfStructSProp = Array.Empty<S>(),
                                                                BombProp = Array.Empty<IEnumerable<C<S>[][,,]>>()
                                                            }
                                                        }
                                                    },
                                                    BombProp = new IEnumerable<C<S>[][,,]>[]
                                                    {
                                                        new List<C<S>[][,,]>
                                                        {
                                                            new C<S>[][,,]
                                                            {
                                                                new C<S>[,,]
                                                                {
                                                                    {
                                                                        {
                                                                            new C<S>{},
                                                                            new C<S>{}
                                                                        }
                                                                    },
                                                                    {
                                                                        {
                                                                            null,
                                                                            null
                                                                        }
                                                                    }
                                                                },
                                                                null
                                                            },
                                                            null
                                                        }
                                                    },
                                                    GenericClassXProp = self,
                                                },
                                                null,
                                                null
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                },
                ClassAProp = new A
                {
                    GenericClassAOfAProp = new A<A>
                    {
                        GenericClassAOfGenericClassAOfAProp = new A<A<A>>
                        {
                            GenericClassAOfGenericClassAOfAProp = new A<A<A>>
                            {
                                ClassAProp = new A
                                {
                                    GenericClassAOfAProp = new A<A>
                                    {
                                        GenericClassAOfGenericClassAOfAProp = new A<A<A>>
                                        {
                                            GenericClassAOfGenericClassAOfAProp = new A<A<A>>
                                            {
                                                ClassAProp = new A
                                                {
                                                }
                                            }
                                        },
                                        ClassAProp = new A
                                        {
                                        }
                                    }
                                }
                            }
                        },
                        ClassAProp = new A
                        {
                            GenericClassAOfAProp = new A<A>
                            {
                                GenericClassAOfGenericClassAOfAProp = new A<A<A>>
                                {
                                    ClassAProp = new A
                                    {
                                    }
                                },
                                ClassAProp = new A
                                {
                                }
                            }
                        }
                    }
                }
            };
            return obj;
        }

        class JsonPropertyInterceptorAttribute : InterceptorAttribute
        {
            public JsonPropertyInterceptorAttribute(string interceptorKey)
                : base(interceptorKey, typeof(JsonPropertyAttribute))
            {
            }

            protected override AttributeBuilderParams Intercept(AttributeBuilderParams originalAttributeBuilderParams)
            {
                originalAttributeBuilderParams.ConstructorArgs = new[] { InterceptorId };
                return originalAttributeBuilderParams;
            }
        }

        class Z
        {
            [JsonPropertyInterceptor("enum")]
            [JsonProperty("")]
            public E EnumProp { get; set; }

            [JsonPropertyInterceptor("struct_S")]
            [JsonProperty("13")]
            public S StructProp { get; set; }

            [JsonPropertyInterceptor("class_A")]
            [JsonProperty("")]
            public A ClassAProp { get; set; }
        }

        struct S
        {
            [JsonPropertyInterceptor("enumerable_of_struct_S")]
            [JsonProperty("")]
            public IEnumerable<S> EnumerableOfStructSProp { get; set; }

            [JsonPropertyInterceptor("generic_class_C_of_struct_S")]
            [JsonProperty("")]
            public C<S> GenericClassCOfStructSProp { get; set; }
        }

        class C<T>
        {
            [JsonPropertyInterceptor("array_of_struct_S")]
            [JsonProperty("")]
            public S[] ArrayOfStructSProp { get; set; }

            [JsonPropertyInterceptor("bomb")]
            [JsonProperty("")]
            public IEnumerable<C<S>[][,,]>[] BombProp { get; set; }

            [JsonPropertyInterceptor("generic_class_X")]
            [JsonProperty("")]
            public X<X<int, float, char>, double, string> GenericClassXProp { get; set; }
        }

        class A
        {
            [JsonPropertyInterceptor("generic_class_A_of_A")]
            [JsonProperty("")]
            public A<A> GenericClassAOfAProp { get; set; }
        }

        class A<T>
        {
            [JsonPropertyInterceptor("generic_class_A_of_generic_class_A_of_A")]
            [JsonProperty("")]
            public A<A<A>> GenericClassAOfGenericClassAOfAProp { get; set; }

            [JsonPropertyInterceptor("class_A")]
            [JsonProperty("")]
            public A ClassAProp { get; set; }
        }

        class X<T1, T2, T3>
        {
            [JsonPropertyInterceptor("self")]
            [JsonProperty("")]
            public X<T1, T2, T3> SelfProp { get; set; }

            [JsonPropertyInterceptor("first")]
            [JsonProperty("")]
            public T1 FirstProp { get; set; }

            [JsonPropertyInterceptor("second")]
            [JsonProperty("")]
            public T2 SecondProp { get; set; }

            [JsonPropertyInterceptor("third")]
            [JsonProperty("")]
            public T3 ThirdProp { get; set; }
        }

        enum E : int
        {
            Default = 0,
            A = 1,
            B = 2,
        }
    }
}
