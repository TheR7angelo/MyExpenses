﻿using AutoMapper;
using MyExpenses.Models.IO.Export.Sql.Tables;
using MyExpenses.Models.Sql.Bases.Tables;

namespace MyExpenses.Models.AutoMapper.Profiles.Exports;

public class TRecursiveFrequencyToExportTRecursiveFrequencyProfile : Profile
{
    public TRecursiveFrequencyToExportTRecursiveFrequencyProfile()
    {
        CreateMap<TRecursiveFrequency, ExportTRecursiveFrequency>()
            .ReverseMap();
    }
}