using Newtonsoft.Json;
using SerializationInterceptor.Tests.Attributes;
using SerializationInterceptor.Tests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
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
        }

        [Fact]
        public async Task AsyncSerialization_NewtonsoftJsonSerializerInvoked_SerializationDoesNotCrash()
        {
            var obj = GetObj();
            await NewtonsoftJsonSerializationInterceptor.SerializeAsync(obj);
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
                DictProp = new Dictionary<IEnumerable<C<int>[][,,][,,,][]>[], A<int>>
                {
                    {
                        new IEnumerable<C<int>[][,,][,,,][]>[]
                        {
                            Enumerable.Empty<C<int>[][,,][,,,][]>()
                        },
                        new A<int>
                        {
                            GenericClassAOfGenericClassAOfAProp = new A<A<A>>()
                        }
                    },
                    {
                        Array.Empty<IEnumerable<C<int>[][,,][,,,][]>>(),
                        null
                    },
                    {
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
                        },
                        new A<int>
                        {
                            GenericClassAOfGenericClassAOfAProp = new A<A<A>>
                            {
                                GenericClassAOfGenericClassAOfAProp = new A<A<A>>()
                            },
                            ClassAProp = new A
                            {
                                GenericClassAOfAProp = new A<A>()
                            }
                        }
                    }
                }
            };
            return obj;
        }
        #endregion

        #region types
        class Root
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
            public ICollection<KeyValuePair<IEnumerable<C<int>[][,,][,,,][]>[], A<int>>> DictProp { get; set; }
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
        #endregion
    }
}
