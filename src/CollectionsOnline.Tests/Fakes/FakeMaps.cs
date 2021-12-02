using IMu;

namespace CollectionsOnline.Tests.Fakes
{
    public static class FakeMaps
    {
        public static Map[] CreateValidPartsMap()
        {
            return new[]
            {
                new Map
                {
                    {
                        "objstatus", new object[]
                        {
                        }
                    }
                },
                new Map
                {
                    {
                        "objstatus", new object[]
                        {
                            new Map
                            {
                                { "StaStatus", "Finished" },
                                {
                                    "location", new Map
                                    {
                                        { "LocLevel1", null },
                                        { "LocLevel2", null },
                                        { "LocLevel4", null },
                                        {
                                            "location", new Map
                                            {
                                                { "LocLevel1", "SCIENCEWORKS (MvCIS)" },
                                                { "LocLevel2", "GROUND LEVEL" },
                                                { "LocLevel4", "GRID B1" },
                                                { "location", null },
                                                { "LocLocationType", "Location" },
                                                { "LocLevel3", "GALLERY 2" }
                                            }
                                        },
                                        { "LocLocationType", "Holder" },
                                        { "LocLevel3", null }
                                    }
                                },
                                {
                                    "event", new Map
                                    {
                                        {
                                            "venname", new object[]
                                            {
                                            }
                                        },
                                        { "EveEventNumber", "318" },
                                        { "EveEventTitle", "The Apple Effect" }
                                    }
                                },
                                { "irn", 30049L }
                            },
                            new Map
                            {
                                { "StaStatus", "On display" },
                                {
                                    "location", new Map
                                    {
                                        { "LocLevel3", null },
                                        { "LocLevel2", null },
                                        { "LocLevel4", null },
                                        { "LocLocationType", "Holder" },
                                        {
                                            "location", new Map
                                            {
                                                { "LocLevel1", "SCIENCEWORKS (MvCIS)" },
                                                { "LocLevel3", "GALLERY 2" },
                                                { "location", null },
                                                { "LocLocationType", "Location" },
                                                { "LocLevel4", "GRID B1" },
                                                { "LocLevel2", "GROUND LEVEL" }
                                            }
                                        },
                                        { "LocLevel1", null }
                                    }
                                },
                                { "irn", 56698L },
                                {
                                    "event", new Map
                                    {
                                        {
                                            "venname", new object[]
                                            {
                                                new Map
                                                {
                                                    { "NamSource", null },
                                                    { "NamBranch", null },
                                                    { "AddPhysStreet", "2 Booker Street" },
                                                    { "NamFullName", "Scienceworks" },
                                                    { "NamOrganisation", "Scienceworks" },
                                                    { "AddPhysCountry", "Australia" },
                                                    { "AddPhysState", "Victoria" },
                                                    { "NamDepartment", null },
                                                    { "AddPhysCity", "Spotswood" },
                                                    { "NamPartyType", "Organisation" },
                                                    { "ColCollaborationName", null },
                                                    {
                                                        "NamOrganisationOtherNames_tab", new object[]
                                                        {
                                                        }
                                                    }
                                                }
                                            }
                                        },
                                        { "EveEventTitle", "Think Ahead" },
                                        { "EveEventNumber", "611" }
                                    }
                                }
                            }
                        }
                    }
                },
                new Map
                {
                    {
                        "objstatus", new object[]
                        {
                            new Map
                            {
                                { "irn", 56699L },
                                {
                                    "event", new Map
                                    {
                                        {
                                            "venname", new object[]
                                            {
                                                new Map
                                                {
                                                    { "AddPhysStreet", "2 Booker Street" },
                                                    { "NamBranch", null },
                                                    { "NamFullName", "Scienceworks" },
                                                    { "NamOrganisation", "Scienceworks" },
                                                    { "AddPhysState", "Victoria" },
                                                    { "AddPhysCountry", "Australia" },
                                                    { "NamSource", null },
                                                    { "NamPartyType", "Organisation" },
                                                    { "AddPhysCity", "Spotswood" },
                                                    {
                                                        "NamOrganisationOtherNames_tab", new object[]
                                                        {
                                                        }
                                                    },
                                                    { "ColCollaborationName", null },
                                                    { "NamDepartment", null }
                                                }
                                            }
                                        },
                                        { "EveEventNumber", "611" },
                                        { "EveEventTitle", "Think Ahead" }
                                    }
                                },
                                {
                                    "location", new Map
                                    {
                                        { "LocLocationType", "Holder" },
                                        {
                                            "location", new Map
                                            {
                                                { "LocLevel4", "GRID B1" },
                                                { "LocLevel2", "GROUND LEVEL" },
                                                { "location", null },
                                                { "LocLocationType", "Location" },
                                                { "LocLevel3", "GALLERY 2" },
                                                { "LocLevel1", "SCIENCEWORKS (MvCIS)" }
                                            }
                                        },
                                        { "LocLevel4", null },
                                        { "LocLevel2", null },
                                        { "LocLevel3", null },
                                        { "LocLevel1", null }
                                    }
                                },
                                { "StaStatus", "On display" }
                            }
                        }
                    }
                },
                new Map
                {
                    {
                        "objstatus", new object[]
                        {
                            new Map
                            {
                                {
                                    "location", new Map
                                    {
                                        { "LocLevel1", null },
                                        { "LocLevel3", null },
                                        { "LocLevel4", null },
                                        { "LocLevel2", null },
                                        { "LocLocationType", "Holder" },
                                        {
                                            "location", new Map
                                            {
                                                { "LocLevel1", "SCIENCEWORKS (MvCIS)" },
                                                { "LocLevel3", "GALLERY 2" },
                                                { "LocLevel4", "GRID B1" },
                                                { "LocLevel2", "GROUND LEVEL" },
                                                { "LocLocationType", "Location" },
                                                { "location", null }
                                            }
                                        }
                                    }
                                },
                                { "StaStatus", "On display" },
                                { "irn", 56700L },
                                {
                                    "event", new Map
                                    {
                                        {
                                            "venname", new object[]
                                            {
                                                new Map
                                                {
                                                    { "NamOrganisation", "Scienceworks" },
                                                    { "AddPhysStreet", "2 Booker Street" },
                                                    { "NamBranch", null },
                                                    { "NamFullName", "Scienceworks" },
                                                    { "AddPhysState", "Victoria" },
                                                    { "AddPhysCountry", "Australia" },
                                                    { "NamSource", null },
                                                    {
                                                        "NamOrganisationOtherNames_tab", new object[]
                                                        {
                                                        }
                                                    },
                                                    { "ColCollaborationName", null },
                                                    { "AddPhysCity", "Spotswood" },
                                                    { "NamPartyType", "Organisation" },
                                                    { "NamDepartment", null }
                                                }
                                            }
                                        },
                                        { "EveEventNumber", "611" },
                                        { "EveEventTitle", "Think Ahead" }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        public static Map[] CreateValidObjectStatusMap()
        {
            return new[]
            {
                new Map
                {
                    {
                        "event", new Map
                        {
                            {
                                "venname", new object[]
                                {
                                }
                            },
                            { "EveEventTitle", "Treasures of the Museum, Victoria, Australia" },
                            { "EveEventNumber", "189" }
                        }
                    },
                    {
                        "location", new Map
                        {
                            {
                                "location", new Map
                                {
                                    { "LocLevel2", "LEVEL 1" },
                                    { "LocLevel4", "GRID D2" },
                                    { "LocLocationType", "Location" },
                                    { "location", null },
                                    { "LocLevel3", "GALLERY 12" },
                                    { "LocLevel1", "MELBOURNE (MvCIS)" }
                                }
                            },
                            { "LocLocationType", "Holder" },
                            { "LocLevel2", null },
                            { "LocLevel4", null },
                            { "LocLevel3", null },
                            { "LocLevel1", null }
                        }
                    },
                    { "StaStatus", "Published" }
                },
                new Map
                {
                    {
                        "event", new Map
                        {
                            {
                                "venname", new object[]
                                {
                                    new Map
                                    {
                                        { "AddPhysStreet", "11 Nicholson Street" },
                                        { "NamFullName", "Melbourne Museum" },
                                        { "NamBranch", null },
                                        { "NamOrganisation", "Melbourne Museum" },
                                        { "AddPhysState", "Victoria" },
                                        { "AddPhysCountry", "Australia" },
                                        { "NamSource", null },
                                        { "NamPartyType", "Organisation" },
                                        { "AddPhysCity", "Melbourne" },
                                        {
                                            "NamOrganisationOtherNames_tab", new object[]
                                            {
                                            }
                                        },
                                        { "ColCollaborationName", null },
                                        { "NamDepartment", null }
                                    }
                                }
                            },
                            { "EveEventTitle", "Galleria (Melbourne Museum)" },
                            { "EveEventNumber", "143" }
                        }
                    },
                    {
                        "location", new Map
                        {
                            { "LocLevel1", null },
                            { "LocLevel3", null },
                            { "LocLevel4", null },
                            { "LocLevel2", null },
                            {
                                "location", new Map
                                {
                                    { "LocLevel1", "MELBOURNE (MvCIS)" },
                                    { "LocLevel3", "GALLERY 12" },
                                    { "LocLocationType", "Location" },
                                    { "location", null },
                                    { "LocLevel2", "LEVEL 1" },
                                    { "LocLevel4", "GRID D2" }
                                }
                            },
                            { "LocLocationType", "Holder" }
                        }
                    },
                    { "StaStatus", "Finished" }
                },
                new Map
                {
                    {
                        "event", new Map
                        {
                            { "EveEventTitle", "PRELIMINARY Museum Inside Out" },
                            { "EveEventNumber", "753" },
                            {
                                "venname", new object[]
                                {
                                    new Map
                                    {
                                        { "NamSource", null },
                                        { "NamFullName", "Melbourne Museum - Touring Hall" },
                                        { "AddPhysStreet", null },
                                        { "NamBranch", null },
                                        { "NamOrganisation", "Melbourne Museum - Touring Hall" },
                                        { "AddPhysCountry", null },
                                        { "AddPhysState", null },
                                        { "NamDepartment", null },
                                        { "NamPartyType", "Organisation" },
                                        { "AddPhysCity", null },
                                        { "ColCollaborationName", null },
                                        {
                                            "NamOrganisationOtherNames_tab", new object[]
                                            {
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    {
                        "location", new Map
                        {
                            { "LocLevel3", null },
                            { "LocLevel4", null },
                            { "LocLevel2", null },
                            { "LocLocationType", "Holder" },
                            {
                                "location", new Map
                                {
                                    { "LocLevel1", "MELBOURNE (MvCIS)" },
                                    { "LocLevel3", "GALLERY 12" },
                                    { "LocLocationType", "Location" },
                                    { "location", null },
                                    { "LocLevel2", "LEVEL 1" },
                                    { "LocLevel4", "GRID D2" }
                                }
                            },
                            { "LocLevel1", null }
                        }
                    },
                    { "StaStatus", "Deselected" }
                },
                new Map
                {
                    {
                        "event", new Map
                        {
                            {
                                "venname", new object[]
                                {
                                }
                            },
                            { "EveEventNumber", "83" },
                            { "EveEventTitle", "Beneath Our Feet" }
                        }
                    },
                    {
                        "location", new Map
                        {
                            { "LocLevel1", null },
                            { "LocLevel2", null },
                            { "LocLevel4", null },
                            {
                                "location", new Map
                                {
                                    { "LocLevel3", "GALLERY 12" },
                                    { "LocLocationType", "Location" },
                                    { "location", null },
                                    { "LocLevel2", "LEVEL 1" },
                                    { "LocLevel4", "GRID D2" },
                                    { "LocLevel1", "MELBOURNE (MvCIS)" }
                                }
                            },
                            { "LocLocationType", "Holder" },
                            { "LocLevel3", null }
                        }
                    },
                    { "StaStatus", "Finished" }
                },
                new Map
                {
                    {
                        "event", new Map
                        {
                            {
                                "venname", new object[]
                                {
                                    new Map
                                    {
                                        { "NamDepartment", null },
                                        { "NamPartyType", "Organisation" },
                                        { "AddPhysCity", null },
                                        {
                                            "NamOrganisationOtherNames_tab", new object[]
                                            {
                                            }
                                        },
                                        { "ColCollaborationName", null },
                                        { "NamSource", null },
                                        { "AddPhysStreet", null },
                                        { "NamFullName", "Melbourne Museum - Australia Gallery" },
                                        { "NamBranch", null },
                                        { "NamOrganisation", "Melbourne Museum - Australia Gallery" },
                                        { "AddPhysCountry", null },
                                        { "AddPhysState", null }
                                    }
                                }
                            },
                            { "EveEventNumber", "1064" },
                            { "EveEventTitle", "Models & Miniatures" }
                        }
                    },
                    {
                        "location", new Map
                        {
                            { "LocLevel2", null },
                            { "LocLevel4", null },
                            { "LocLocationType", "Holder" },
                            {
                                "location", new Map
                                {
                                    { "LocLevel3", "GALLERY 12" },
                                    { "LocLocationType", "Location" },
                                    { "location", null },
                                    { "LocLevel4", "GRID D2" },
                                    { "LocLevel2", "LEVEL 1" },
                                    { "LocLevel1", "MELBOURNE (MvCIS)" }
                                }
                            },
                            { "LocLevel3", null },
                            { "LocLevel1", null }
                        }
                    },
                    { "StaStatus", "On display" }
                }
            };
        }

        public static Map[] CreateIncorrectObjectStatusMap()
        {
            return new[]
            {
                new Map
                {
                    {
                        "event", new Map
                        {
                            { "EveEventTitle", "Galleries of Remembrance (Loan)" },
                            { "EveEventNumber", "865" },
                            {
                                "venname", new object[]
                                {
                                    new Map
                                    {
                                        { "AddPhysState", "Victoria" },
                                        { "AddPhysCountry", null },
                                        { "NamOrganisation", "Shrine of Remembrance" },
                                        { "NamBranch", null },
                                        { "NamFullName", "Shrine of Remembrance" },
                                        { "AddPhysStreet", "Birdwood Avenue" },
                                        { "NamSource", null },
                                        {
                                            "NamOrganisationOtherNames_tab", new object[]
                                            {
                                            }
                                        },
                                        { "ColCollaborationName", null },
                                        { "AddPhysCity", "South Yarra" },
                                        { "NamPartyType", "Organisation" },
                                        { "NamDepartment", null }
                                    }
                                }
                            }
                        }
                    },
                    { "StaStatus", "On display" },
                    {
                        "location", new Map
                        {
                            { "LocLevel3", null },
                            { "LocLevel4", null },
                            { "LocLevel2", "EXHIBITION LOAN" },
                            { "LocLocationType", "Location" },
                            { "location", null },
                            { "LocLevel1", "EXTERNAL (MvCIS)" }
                        }
                    }
                }
            };
        }

        public static Map[] CreateEmptyMapArray()
        {
            return new Map[] { };
        }
    }
}