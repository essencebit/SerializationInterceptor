using SerializationInterceptor.Tests.Attributes;
using SerializationInterceptor.Tests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xunit;

namespace SerializationInterceptor.Tests
{
    public class SystemTextJsonSerializationTests
    {
        [Fact]
        public void Serialization_NewtonsoftJsonSerializerInvoked_SerializationDoesNotCrash()
        {
            var obj = GetObj();
            SystemTextJsonSerializationInterceptor.Serialize(obj);
        }

        [Fact]
        public async Task AsyncSerialization_NewtonsoftJsonSerializerInvoked_SerializationDoesNotCrash()
        {
            var obj = GetObj();
            await SystemTextJsonSerializationInterceptor.SerializeAsync(obj);
        }

        [Fact]
        public void Deserialization_NewtonsoftJsonSerializerInvoked_DeserializationDoesNotCrash()
        {
            var @string = GetString();
            var map = GetAbstractConcreteMap();
            SystemTextJsonSerializationInterceptor.Deserialize<Root>(@string, map);
        }

        [Fact]
        public async Task AsyncDeserialization_NewtonsoftJsonSerializerInvoked_DeserializationDoesNotCrash()
        {
            var @string = GetString();
            var map = GetAbstractConcreteMap();
            await SystemTextJsonSerializationInterceptor.DeserializeAsync<Root>(@string, map);
        }

        #region test data
        internal static Root GetObj()
        {
            var structs = new S[]
            {
                new S
                {
                    GenericClassCOfStructSProp = new C<S>
                    {
                        ArrayOfStructSProp = Array.Empty<S>(),
                        BombProp = Array.Empty<IEnumerable<C<S>[][]>>()
                    }
                },
                default,
                new S()
            };
            var self = new X<X<int, float, char>, double, string>();
            self.SelfProp = self;
            self.FirstProp = new X<int, float, char>();
            self.FirstProp.SelfProp = self.FirstProp;
            var bomb = new HashSet<C<S>[][]>[]
            {
                new HashSet<C<S>[][]>
                {
                    new C<S>[][]
                    {
                        new C<S>[]
                        {
                            new C<S>
                            {
                            },
                            new C<S>
                            {
                            },
                            null,
                            null
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
                        BombProp = new ISet<C<S>[][]>[]
                        {
                            new HashSet<C<S>[][]>
                            {
                                new C<S>[][]
                                {
                                    new C<S>[]
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
                                                        BombProp = Array.Empty<IEnumerable<C<S>[][]>>()
                                                    }
                                                }
                                            },
                                            BombProp = new IEnumerable<C<S>[][]>[]
                                            {
                                                new List<C<S>[][]>
                                                {
                                                    new C<S>[][]
                                                    {
                                                        new C<S>[]
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
                                            },
                                            GenericClassXProp = self,
                                        }
                                    }
                                }
                            },
                            null,
                            new HashSet<C<S>[][]>
                            {
                                new C<S>[][]
                                {
                                    null,
                                    new C<S>[]
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
                                                        BombProp = Array.Empty<IEnumerable<C<S>[][]>>()
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
                DictProp = new Dictionary<string, IEnumerable<C<int>[][][][]>[]>
                {
                    {
                        "key1",
                        new IEnumerable<C<int>[][][][]>[]
                        {
                            Enumerable.Empty<C<int>[][][][]>()
                        }
                    },
                    {
                        "key2",
                        Array.Empty<IEnumerable<C<int>[][][][]>>()
                    },
                    {
                        "key3",
                        new List<C<int>[][][][]>[]
                        {
                            new List<C<int>[][][][]>
                            {
                            },
                            null,
                            null,
                            new List<C<int>[][][][]>
                            {
                                Array.Empty<C<int>[][][]>(),
                                null,
                                new C<int>[][][][]
                                {
                                    null,
                                    new C<int>[][][]
                                    {
                                        new C<int>[][]
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
                                            },
                                            new C<int>[]
                                            {   
                                                null     
                                            }
                                        },
                                        null,
                                        null,
                                        null,
                                        null
                                    }
                                }
                            }
                        }
                    }
                }
            };
            return obj;
        }

        internal static string GetString()
        {
            return "{\"$id\":\"1\",\"enum\":1,\"struct_S\":{\"enumerable_of_struct_S\":{\"$id\":\"2\",\"$values\":[{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null},{\"enumerable_of_struct_S\":{\"$id\":\"3\",\"$values\":[]},\"generic_class_C_of_struct_S\":{\"$id\":\"4\",\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null}}]},\"generic_class_C_of_struct_S\":{\"$id\":\"5\",\"array_of_struct_S\":[{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":{\"$id\":\"6\",\"array_of_struct_S\":[],\"bomb\":[],\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null}},{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null},{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null}],\"bomb\":[{\"$id\":\"7\",\"$values\":[[[{\"$id\":\"8\",\"array_of_struct_S\":[{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":{\"$id\":\"9\",\"array_of_struct_S\":[],\"bomb\":[],\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null}}],\"bomb\":[{\"$id\":\"10\",\"$values\":[[[{\"$id\":\"11\",\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null},{\"$id\":\"12\",\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":{\"$id\":\"13\",\"number\":1,\"char\":\"x\",\"abstract_class_generic_arg\":{\"$id\":\"14\",\"number\":0,\"char\":\"\\u0000\",\"abstract_class_generic_arg\":{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null}}},\"generic_interface\":{\"$id\":\"15\",\"interface_generic_arg\":{\"$id\":\"16\",\"number\":3,\"char\":\"z\",\"abstract_class_generic_arg\":null},\"string\":\"string2\"}}]]]}],\"generic_class_X\":{\"$id\":\"17\",\"self\":{\"$ref\":\"17\"},\"first\":{\"$id\":\"18\",\"self\":{\"$ref\":\"18\"},\"first\":0,\"second\":0,\"third\":\"\\u0000\"},\"second\":0,\"third\":null},\"generic_abstract_class\":null,\"generic_interface\":null}]]]},null,{\"$id\":\"19\",\"$values\":[[null,[{\"$id\":\"20\",\"array_of_struct_S\":[{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":{\"$id\":\"21\",\"array_of_struct_S\":[],\"bomb\":[],\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null}}],\"bomb\":[{\"$id\":\"22\",\"$values\":[[[{\"$id\":\"23\",\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null},{\"$id\":\"24\",\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null},null,null],null],null]}],\"generic_class_X\":{\"$ref\":\"17\"},\"generic_abstract_class\":null,\"generic_interface\":null},null,null]]]}],\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null}},\"class_A\":{\"$id\":\"25\",\"generic_class_A_of_A\":{\"$id\":\"26\",\"generic_class_A_of_generic_class_A_of_A\":{\"$id\":\"27\",\"generic_class_A_of_generic_class_A_of_A\":{\"$id\":\"28\",\"generic_class_A_of_generic_class_A_of_A\":null,\"class_A\":{\"$id\":\"29\",\"generic_class_A_of_A\":{\"$id\":\"30\",\"generic_class_A_of_generic_class_A_of_A\":{\"$id\":\"31\",\"generic_class_A_of_generic_class_A_of_A\":{\"$id\":\"32\",\"generic_class_A_of_generic_class_A_of_A\":null,\"class_A\":{\"$id\":\"33\",\"generic_class_A_of_A\":null}},\"class_A\":null},\"class_A\":{\"$id\":\"34\",\"generic_class_A_of_A\":null}}}},\"class_A\":null},\"class_A\":{\"$id\":\"35\",\"generic_class_A_of_A\":{\"$id\":\"36\",\"generic_class_A_of_generic_class_A_of_A\":{\"$id\":\"37\",\"generic_class_A_of_generic_class_A_of_A\":null,\"class_A\":{\"$id\":\"38\",\"generic_class_A_of_A\":null}},\"class_A\":{\"$id\":\"39\",\"generic_class_A_of_A\":null}}}}},\"dict\":{\"$id\":\"40\",\"key1\":[{\"$id\":\"41\",\"$values\":[]}],\"key2\":[],\"key3\":[{\"$id\":\"42\",\"$values\":[]},null,null,{\"$id\":\"43\",\"$values\":[[],null,[null,[[[{\"$id\":\"44\",\"array_of_struct_S\":null,\"bomb\":[{\"$ref\":\"22\"}],\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null},null,null,{\"$id\":\"45\",\"array_of_struct_S\":[{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":{\"$ref\":\"6\"}},{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null},{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null}],\"bomb\":[{\"$ref\":\"22\"}],\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null},null],[null]],null,null,null,null]]]}]}}";
        }

        internal static AbstractConcreteMap GetAbstractConcreteMap()
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
        internal class Root
        {
            [JsonPropertyNameInterceptor("enum")]
            [JsonPropertyName("")]
            public E EnumProp { get; set; }

            [JsonPropertyNameInterceptor("struct_S")]
            [JsonPropertyName("")]
            public S StructProp { get; set; }

            [JsonPropertyNameInterceptor("class_A")]
            [JsonPropertyName("")]
            public A ClassAProp { get; set; }

            [JsonPropertyNameInterceptor("dict")]
            [JsonPropertyName("")]
            public IDictionary<string, IEnumerable<C<int>[][][][]>[]> DictProp { get; set; }
        }

        internal struct S
        {
            [JsonPropertyNameInterceptor("enumerable_of_struct_S")]
            [JsonPropertyName("")]
            public IEnumerable<S> EnumerableOfStructSProp { get; set; }

            [JsonPropertyNameInterceptor("generic_class_C_of_struct_S")]
            [JsonPropertyName("")]
            public C<S> GenericClassCOfStructSProp { get; set; }
        }

        internal class C<T>
        {
            [JsonPropertyNameInterceptor("array_of_struct_S")]
            [JsonPropertyName("")]
            public S[] ArrayOfStructSProp { get; set; }

            [JsonPropertyNameInterceptor("bomb")]
            [JsonPropertyName("")]
            public IEnumerable<C<S>[][]>[] BombProp { get; set; }

            [JsonPropertyNameInterceptor("generic_class_X")]
            [JsonPropertyName("")]
            public X<X<int, float, char>, double, string> GenericClassXProp { get; set; }

            [JsonPropertyNameInterceptor("generic_abstract_class")]
            [JsonPropertyName("")]
            public Abstract<Abstract<T>> GenericAbstractClassProp { get; set; }

            [JsonPropertyNameInterceptor("generic_interface")]
            [JsonPropertyName("")]
            public IInterface<Abstract<Abstract<T>>> GenericInterfaceProp { get; set; }
        }

        internal class A
        {
            [JsonPropertyNameInterceptor("generic_class_A_of_A")]
            [JsonPropertyName("")]
            public A<A> GenericClassAOfAProp { get; set; }
        }

        internal class A<T>
        {
            [JsonPropertyNameInterceptor("generic_class_A_of_generic_class_A_of_A")]
            [JsonPropertyName("")]
            public A<A<A>> GenericClassAOfGenericClassAOfAProp { get; set; }

            [JsonPropertyNameInterceptor("class_A")]
            [JsonPropertyName("")]
            public A ClassAProp { get; set; }
        }

        internal class X<T1, T2, T3>
        {
            [JsonPropertyNameInterceptor("self")]
            [JsonPropertyName("")]
            public X<T1, T2, T3> SelfProp { get; set; }

            [JsonPropertyNameInterceptor("first")]
            [JsonPropertyName("")]
            public T1 FirstProp { get; set; }

            [JsonPropertyNameInterceptor("second")]
            [JsonPropertyName("")]
            public T2 SecondProp { get; set; }

            [JsonPropertyNameInterceptor("third")]
            [JsonPropertyName("")]
            public T3 ThirdProp { get; set; }
        }

        internal enum E : int
        {
            Default = 0,
            A = 1,
            B = 2,
        }

        internal abstract class Abstract<T>
        {
            [JsonPropertyNameInterceptor("number")]
            [JsonPropertyName("")]
            public int NumberProp { get; set; }

            [JsonPropertyNameInterceptor("char")]
            [JsonPropertyName("")]
            public char CharProp { get; set; }

            [JsonPropertyNameInterceptor("abstract_class_generic_arg")]
            [JsonPropertyName("")]
            public T AbstractClassGenericArgProp { get; set; }
        }

        internal interface IInterface<T>
        {
            [JsonPropertyNameInterceptor("interface_generic_arg")]
            [JsonPropertyName("")]
            public T InterfaceGenericArgProp { get; set; }

            [JsonPropertyNameInterceptor("string")]
            [JsonPropertyName("")]
            public string StringProp { get; set; }
        }

        internal class Concrete<T> : Abstract<T>, IInterface<T>
        {
            public string StringProp { get; set; }
            public T InterfaceGenericArgProp { get; set; }
        }
        #endregion
    }
}
