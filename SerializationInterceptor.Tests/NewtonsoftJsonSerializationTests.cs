using Newtonsoft.Json;
using SerializationInterceptor.Tests.Attributes;
using SerializationInterceptor.Tests.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SerializationInterceptor.Tests
{
    public class NewtonsoftJsonSerializationTests
    {
        [Fact]
        public void Serialization_NewtonsoftJsonSerializerInvoked_SerializationOccursCorrectly()
        {
            var obj = new Z
            {
                EnumProp = E.A,
                StructProp = new S
                {
                    EnumerableOfStructSProp = new List<S>
                    {
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
                                    ArrayOfStructSProp = new S[]
                                    {
                                    },
                                    BombProp = new IEnumerable<C<S>[]>[0]
                                }
                            }
                        },
                        BombProp = new IEnumerable<C<S>[]>[]
                        {
                            new List<C<S>[]>
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
                                                    ArrayOfStructSProp = new S[]
                                                    {
                                                    },
                                                    BombProp = new IEnumerable<C<S>[]>[0]
                                                }
                                            }
                                        },
                                        BombProp = new IEnumerable<C<S>[]>[]
                                        {
                                            new List<C<S>[]>
                                            {
                                                new C<S>[]
                                                {
                                                }
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

            var actual = NewtonsoftJsonSerializationInterceptor.Serialize(obj);
            var expected = actual;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task AsyncSerialization_NewtonsoftJsonSerializerInvoked_SerializationOccursCorrectly()
        {
            var obj = new Z
            {
                EnumProp = E.A,
                StructProp = new S
                {
                    EnumerableOfStructSProp = new List<S>
                    {
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
                                    ArrayOfStructSProp = new S[]
                                    {
                                    },
                                    BombProp = new IEnumerable<C<S>[]>[0]
                                }
                            }
                        },
                        BombProp = new IEnumerable<C<S>[]>[]
                        {
                            new List<C<S>[]>
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
                                                    ArrayOfStructSProp = new S[]
                                                    {
                                                    },
                                                    BombProp = new IEnumerable<C<S>[]>[0]
                                                }
                                            }
                                        },
                                        BombProp = new IEnumerable<C<S>[]>[]
                                        {
                                            new List<C<S>[]>
                                            {
                                                new C<S>[]
                                                {
                                                }
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

            var actual = await NewtonsoftJsonSerializationInterceptor.SerializeAsync(obj);
            var expected = actual;
            Assert.Equal(expected, actual);
        }

        #region types
        class Z
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
            public IEnumerable<C<S>[]>[] BombProp { get; set; }

            [JsonPropertyInterceptor("generic_class_X")]
            [JsonProperty("")]
            public X<int, string, double> GenericClassXProp { get; set; }
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
            public X<T1, T2, T3> Self { get; set; }

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
