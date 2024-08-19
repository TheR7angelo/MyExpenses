using AutoMapper;
using MyExpenses.Models.IO.Export.Sql.Tables;
using MyExpenses.Models.Sql.Bases.Tables;

namespace MyExpenses.Models.AutoMapper.Profiles.Exports;

public class THistoryToExportTHistoryProfile : Profile
{
    public THistoryToExportTHistoryProfile()
    {
        CreateMap<THistory, ExportTHistory>()
            .ReverseMap();
    }
}