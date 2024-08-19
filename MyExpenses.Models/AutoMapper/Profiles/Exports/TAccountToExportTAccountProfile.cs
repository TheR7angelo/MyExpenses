using AutoMapper;
using MyExpenses.Models.IO.Export.Sql.Tables;
using MyExpenses.Models.Sql.Bases.Tables;

namespace MyExpenses.Models.AutoMapper.Profiles.Exports;

public class TAccountToExportTAccountProfile : Profile
{
    public TAccountToExportTAccountProfile()
    {
        CreateMap<TAccount, ExportTAccount>()
            .ReverseMap();
    }
}