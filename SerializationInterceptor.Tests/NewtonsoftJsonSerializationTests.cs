using Newtonsoft.Json;
using SerializationInterceptor.Tests.Attributes;
using SerializationInterceptor.Tests.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SerializationInterceptor.Tests
{
    public class NewtonsoftJsonSerializationTests
    {
        [Fact]
        public void Serialization_NewtonsoftJsonSerializerInvoked_SerializationDoesNotCrash()
        {
            var obj = GetObj();
            NewtonsoftJsonSerializationInterceptor.Serialize(obj);
            NewtonsoftJsonSerializationInterceptor.Serialize(obj, obj.GetType());
            NewtonsoftJsonSerializationInterceptor.Serialize(obj, new MemoryStream());
            NewtonsoftJsonSerializationInterceptor.Serialize(obj, obj.GetType(), new MemoryStream());
        }

        [Fact]
        public async Task AsyncSerialization_NewtonsoftJsonSerializerInvoked_SerializationDoesNotCrash()
        {
            var obj = GetObj();
            await NewtonsoftJsonSerializationInterceptor.SerializeAsync(obj);
            await NewtonsoftJsonSerializationInterceptor.SerializeAsync(obj, obj.GetType());
            await NewtonsoftJsonSerializationInterceptor.SerializeAsync(obj, new MemoryStream());
            await NewtonsoftJsonSerializationInterceptor.SerializeAsync(obj, obj.GetType(), new MemoryStream());
        }

        [Fact]
        public void Deserialization_NewtonsoftJsonSerializerInvoked_DeserializationDoesNotCrash()
        {
            var @string = GetString();
            var map = GetAbstractConcreteMap();
            NewtonsoftJsonSerializationInterceptor.Deserialize<Root>(@string, map);
            NewtonsoftJsonSerializationInterceptor.Deserialize(@string, typeof(Root), map);
            NewtonsoftJsonSerializationInterceptor.Deserialize<Root>(new MemoryStream(Encoding.UTF8.GetBytes(@string)), map);
            NewtonsoftJsonSerializationInterceptor.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(@string)), typeof(Root), map);
        }

        [Fact]
        public async Task AsyncDeserialization_NewtonsoftJsonSerializerInvoked_DeserializationDoesNotCrash()
        {
            var @string = GetString();
            var map = GetAbstractConcreteMap();
            await NewtonsoftJsonSerializationInterceptor.DeserializeAsync<Root>(@string, map);
            await NewtonsoftJsonSerializationInterceptor.DeserializeAsync(@string, typeof(Root), map);
            await NewtonsoftJsonSerializationInterceptor.DeserializeAsync<Root>(new MemoryStream(Encoding.UTF8.GetBytes(@string)), map);
            await NewtonsoftJsonSerializationInterceptor.DeserializeAsync(new MemoryStream(Encoding.UTF8.GetBytes(@string)), typeof(Root), map);
        }

        #region test data
        private static Root GetObj()
        {
            var structs = new S[]
            {
                new S
                {
                    GenericClassCOfStructSProp = new C<S>
                    {
                        ArrayOfStructSProp = Array.Empty<S>(),
                        BombProp = Array.Empty<IEnumerable<C<S>[][,,]>>()
                    }
                },
                default,
                new S()
            };
            var self = new X<X<int, float, char>, double, string>();
            self.SelfProp = self;
            self.FirstProp = new X<int, float, char>();
            self.FirstProp.SelfProp = self.FirstProp;
            var bomb = new HashSet<C<S>[][,,]>[]
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
                                    },
                                    new C<S>
                                    {
                                    }
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
            };
            var obj = new Root
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
                        ArrayOfStructSProp = structs,
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
                                                                            new C<S>
                                                                            {
                                                                            },
                                                                            new C<S>
                                                                            {
                                                                                GenericAbstractClassProp = new Concrete<Abstract<S>>
                                                                                {
                                                                                    CharProp = 'x',
                                                                                    NumberProp = 1,
                                                                                    StringProp = "string1",
                                                                                    AbstractClassGenericArgProp = new Concrete<S>
                                                                                    {
                                                                                        AbstractClassGenericArgProp = new S()
                                                                                    }
                                                                                },
                                                                                GenericInterfaceProp = new Concrete<Abstract<Abstract<S>>>
                                                                                {
                                                                                    CharProp = 'y',
                                                                                    NumberProp = 2,
                                                                                    StringProp = "string2",
                                                                                    InterfaceGenericArgProp = new Concrete<Abstract<S>>
                                                                                    {
                                                                                        CharProp = 'z',
                                                                                        NumberProp = 3,
                                                                                        StringProp = "string3",
                                                                                        InterfaceGenericArgProp = new Concrete<S>
                                                                                        {
                                                                                            AbstractClassGenericArgProp = new S()
                                                                                        }
                                                                                    },
                                                                                }
                                                                            }
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
                                                    BombProp = bomb,
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
                },
                DictProp = new Dictionary<string, IEnumerable<C<int>[][,,][,,,][]>[]>
                {
                    {
                        "key1",
                        new IEnumerable<C<int>[][,,][,,,][]>[]
                        {
                            Enumerable.Empty<C<int>[][,,][,,,][]>()
                        }
                    },
                    {
                        "key2",
                        Array.Empty<IEnumerable<C<int>[][,,][,,,][]>>()
                    },
                    {
                        "key3",
                        new List<C<int>[][,,][,,,][]>[]
                        {
                            new List<C<int>[][,,][,,,][]>
                            {
                            },
                            null,
                            null,
                            new List<C<int>[][,,][,,,][]>
                            {
                                Array.Empty<C<int>[,,][,,,][]>(),
                                null,
                                new C<int>[][,,][,,,][]
                                {
                                    null,
                                    new C<int>[,,][,,,][]
                                    {
                                        {
                                            {
                                                null,
                                                new C<int>[,,,][]
                                                {
                                                    {
                                                        {
                                                            {
                                                                new C<int>[]
                                                                {
                                                                    new C<int>
                                                                    {
                                                                        BombProp = bomb
                                                                    },
                                                                    null,
                                                                    null,
                                                                    new C<int>
                                                                    {
                                                                        BombProp = bomb,
                                                                        ArrayOfStructSProp =structs
                                                                    },
                                                                    null,
                                                                }
                                                            }
                                                        }
                                                    },
                                                    {
                                                        {
                                                            {
                                                                null
                                                            }
                                                        }
                                                    }

                                                },
                                                null
                                            },
                                            {
                                                null,
                                                null,
                                                null
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            return obj;
        }

        private static string GetString()
        {
            return "{\"enum\":1,\"struct_S\":{\"enumerable_of_struct_S\":[{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null},{\"enumerable_of_struct_S\":[],\"generic_class_C_of_struct_S\":{\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null}}],\"generic_class_C_of_struct_S\":{\"array_of_struct_S\":[{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":{\"array_of_struct_S\":[],\"bomb\":[],\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null}},{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null},{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null}],\"bomb\":[[[[[[{\"array_of_struct_S\":[{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":{\"array_of_struct_S\":[],\"bomb\":[],\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null}}],\"bomb\":[[[[[[{\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null},{\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":{\"number\":49,\"char\":\"x\",\"abstract_class_generic_arg\":{\"number\":0,\"char\":\"\\u0000\",\"abstract_class_generic_arg\":{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null}}},\"generic_interface\":{\"interface_generic_arg\":{\"number\":51,\"char\":\"z\",\"abstract_class_generic_arg\":null},\"string\":\"string2\"}}]]]]]],\"generic_class_X\":{\"first\":{\"first\":0,\"second\":0.0,\"third\":\"\\u0000\"},\"second\":0.0,\"third\":null},\"generic_abstract_class\":null,\"generic_interface\":null}]]]]],null,[[null,[[[{\"array_of_struct_S\":[{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":{\"array_of_struct_S\":[],\"bomb\":[],\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null}}],\"bomb\":[[[[[[{\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null},{\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null}]],[[null,null]]],null],null]],\"generic_class_X\":{\"first\":{\"first\":0,\"second\":0.0,\"third\":\"\\u0000\"},\"second\":0.0,\"third\":null},\"generic_abstract_class\":null,\"generic_interface\":null},null,null]]]]]],\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null}},\"class_A\":{\"generic_class_A_of_A\":{\"generic_class_A_of_generic_class_A_of_A\":{\"generic_class_A_of_generic_class_A_of_A\":{\"generic_class_A_of_generic_class_A_of_A\":null,\"class_A\":{\"generic_class_A_of_A\":{\"generic_class_A_of_generic_class_A_of_A\":{\"generic_class_A_of_generic_class_A_of_A\":{\"generic_class_A_of_generic_class_A_of_A\":null,\"class_A\":{\"generic_class_A_of_A\":null}},\"class_A\":null},\"class_A\":{\"generic_class_A_of_A\":null}}}},\"class_A\":null},\"class_A\":{\"generic_class_A_of_A\":{\"generic_class_A_of_generic_class_A_of_A\":{\"generic_class_A_of_generic_class_A_of_A\":null,\"class_A\":{\"generic_class_A_of_A\":null}},\"class_A\":{\"generic_class_A_of_A\":null}}}}},\"dict\":{\"key1\":[[]],\"key2\":[],\"key3\":[[],null,null,[[],null,[null,[[[null,[[[[[{\"array_of_struct_S\":null,\"bomb\":[[[[[[{\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null},{\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null}]],[[null,null]]],null],null]],\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null},null,null,{\"array_of_struct_S\":[{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":{\"array_of_struct_S\":[],\"bomb\":[],\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null}},{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null},{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null}],\"bomb\":[[[[[[{\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null},{\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null}]],[[null,null]]],null],null]],\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null},null]]]],[[[null]]]],null],[null,null,null]]]]]]}}";
        }

        private static AbstractConcreteMap GetAbstractConcreteMap()
        {
            return new AbstractConcreteMap
            {
                { typeof(Abstract<Abstract<S>>), typeof(Concrete<Abstract<S>>) },
                { typeof(Abstract<S>), typeof(Concrete<S>) },
                { typeof(IInterface<Abstract<Abstract<S>>>), typeof(Concrete<Abstract<Abstract<S>>>) },
            };
        }
        #endregion

        #region types
        private class Root
        {
            [JsonPropertyInterceptor("enum")]
            [JsonProperty("")]
            public E EnumProp { get; set; }

            [JsonPropertyInterceptor("struct_S")]
            [JsonProperty("")]
            public S StructProp { get; set; }

            [JsonPropertyInterceptor("class_A")]
            [JsonProperty("")]
            public A ClassAProp { get; set; }

            [JsonPropertyInterceptor("dict")]
            [JsonProperty("")]
            public IDictionary<string, IEnumerable<C<int>[][,,][,,,][]>[]> DictProp { get; set; }
        }

        private struct S
        {
            [JsonPropertyInterceptor("enumerable_of_struct_S")]
            [JsonProperty("")]
            public IEnumerable<S> EnumerableOfStructSProp { get; set; }

            [JsonPropertyInterceptor("generic_class_C_of_struct_S")]
            [JsonProperty("")]
            public C<S> GenericClassCOfStructSProp { get; set; }
        }

        private class C<T>
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

            [JsonPropertyInterceptor("generic_abstract_class")]
            [JsonProperty("")]
            public Abstract<Abstract<T>> GenericAbstractClassProp { get; set; }

            [JsonPropertyInterceptor("generic_interface")]
            [JsonProperty("")]
            public IInterface<Abstract<Abstract<T>>> GenericInterfaceProp { get; set; }
        }

        private class A
        {
            [JsonPropertyInterceptor("generic_class_A_of_A")]
            [JsonProperty("")]
            public A<A> GenericClassAOfAProp { get; set; }
        }

        private class A<T>
        {
            [JsonPropertyInterceptor("generic_class_A_of_generic_class_A_of_A")]
            [JsonProperty("")]
            public A<A<A>> GenericClassAOfGenericClassAOfAProp { get; set; }

            [JsonPropertyInterceptor("class_A")]
            [JsonProperty("")]
            public A ClassAProp { get; set; }
        }

        private class X<T1, T2, T3>
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

        private enum E : int
        {
            Default = 0,
            A = 1,
            B = 2,
        }

        private abstract class Abstract<T>
        {
            [JsonPropertyInterceptor("number")]
            [JsonProperty("")]
            public int NumberProp { get; set; }

            [JsonPropertyInterceptor("char")]
            [JsonProperty("")]
            public char CharProp { get; set; }

            [JsonPropertyInterceptor("abstract_class_generic_arg")]
            [JsonProperty("")]
            public T AbstractClassGenericArgProp { get; set; }
        }

        private interface IInterface<T>
        {
            [JsonPropertyInterceptor("interface_generic_arg")]
            [JsonProperty("")]
            public T InterfaceGenericArgProp { get; set; }

            [JsonPropertyInterceptor("string")]
            [JsonProperty("")]
            public string StringProp { get; set; }
        }

        private class Concrete<T> : Abstract<T>, IInterface<T>
        {
            public string StringProp { get; set; }
            public T InterfaceGenericArgProp { get; set; }
        }
        #endregion
    }
}
