using Newtonsoft.Json;
using SerializationInterceptor;
using SerializationInterceptor.Attributes;
using SerializationInterceptor.Enums;
using System;
using System.Collections.Generic;

namespace Test
{
    public class Program
    {
        [JsonPropertyInterceptor(null)]
        public struct Test
        {
            public int I { set { } }
            public IDictionary<Test,int?> Prop { get; set; }
        }

        public static void Main(string[] args)
        {
            var obj = new Test
            {
                Prop = new Dictionary<Test,int?>
                {
                    { new Test{},null },
                }
            };
            var objCloneType = TypeCloneFactory.CloneType(Operation.Serialization, obj.GetType());
            var def = TypeDefinitionSerializer.Serialize(objCloneType);
            var objClone = PureAutoMapper.Map(obj, obj.GetType(), objCloneType);
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
                                                        }
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
